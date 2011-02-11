using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core.IO;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Search;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ProtoBuf;
using System.Runtime.InteropServices;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    [ProtoContract]
    public class AlbumDetailsViewModel : ViewModelBase
    {
        private readonly IZuneDatabaseReader _dbReader;
        private readonly IViewLocator _locator;
        private readonly IZuneAudioFileRetriever _fileRetriever;
        private readonly SharedModel _sharedModel;
        private bool _isDownloadingDetails;
        private DbAlbum _zuneAlbumMetaData;
        private WebAlbum _webAlbumMetaData;
        private LinkStatus _linkStatus;

        public AlbumDetailsViewModel(IZuneDatabaseReader dbReader,
                                     IViewLocator locator,
                                     IZuneAudioFileRetriever fileRetriever,
                                     SharedModel sharedModel)
        {
            _dbReader = dbReader;
            _locator = locator;
            _fileRetriever = fileRetriever;
            _sharedModel = sharedModel;
            _sharedModel.DbAlbum = ZuneAlbumMetaData;

            LinkCommand = new RelayCommand(LinkAlbum);
            DelinkCommand = new RelayCommand(DelinkAlbum);
            RefreshCommand = new RelayCommand(RefreshAlbum);
        }

        public AlbumDetailsViewModel()
        {
            //used for design-time purposes
        }

        #region View Bindings

        public string AlbumTitle
        {
            get { return ZuneAlbumMetaData.Title; }
        }

        public string AlbumArtist
        {
            get { return ZuneAlbumMetaData.Artist; }
        }

        public DateTime DateAdded
        {
            get { return ZuneAlbumMetaData.DateAdded; }
        }

        public RelayCommand MoreInfoCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand DelinkCommand { get; private set; }

        [ProtoMember(1)]
        public DbAlbum ZuneAlbumMetaData
        {
            get { return _zuneAlbumMetaData; }
            set
            {
                _zuneAlbumMetaData = value;
                RaisePropertyChanged(() => ZuneAlbumMetaData);
            }
        }

        [ProtoMember(2)]
        public WebAlbum WebAlbumMetaData
        {
            get { return _webAlbumMetaData; }
            set
            {
                _webAlbumMetaData = value;
                RaisePropertyChanged(() => WebAlbumMetaData);
            }
        }

        [ProtoMember(3)]
        public LinkStatus LinkStatus
        {
            get { return _linkStatus; }
            set
            {
                _linkStatus = value;
                RaisePropertyChanged(() => LinkStatus);
                RaisePropertyChanged(() => CanLink);
                RaisePropertyChanged(() => CanDelink);
            }
        }

        public bool IsDownloadingDetails
        {
            get { return _isDownloadingDetails; }
            set
            {
                _isDownloadingDetails = value;
                RaisePropertyChanged(() => IsDownloadingDetails);
            }
        }

        public bool CanLink
        {
            get { return true; }
        }

        public bool CanDelink
        {
            get { return _linkStatus != LinkStatus.Unlinked && _linkStatus != LinkStatus.Unknown; }
        }

        public bool CanRefresh   
        {
            get { return SharedMethods.CheckIfZuneSoftwareIsRunning(); }
        }

        #endregion

        public void LinkAlbum()
        {
            try
            {
                ZuneAlbumMetaData = _dbReader.GetAlbum(ZuneAlbumMetaData.MediaId);
                _sharedModel.DbAlbum = ZuneAlbumMetaData;

                IEnumerable<string> filePaths = ZuneAlbumMetaData.Tracks.Select(x => x.FilePath);
                _sharedModel.SongsFromFile = _fileRetriever.GetContainers(filePaths);

                var searchVm = _locator.SwitchToView<SearchView,SearchViewModel>();
                searchVm.Search(ZuneAlbumMetaData.Artist, ZuneAlbumMetaData.Title);
            }
            catch (COMException)
            {
                Messenger.Default.Send<ErrorMessage, ApplicationViewModel>(new ErrorMessage(ErrorMode.Error,
                    "Could not find album, you may need to refresh the database."));
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<ErrorMessage, ApplicationViewModel>(new ErrorMessage(ErrorMode.Error, ex.Message));
                //if we hit an error on any track in the albums then just fail and dont read anymore
            }
        }

        public void DelinkAlbum()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            //TODO: fix bug where application crashes when removing an album that is currently playing

            var filePaths = _dbReader.GetTracksForAlbum(ZuneAlbumMetaData.MediaId).Select(x => x.FilePath);

            var containers = _fileRetriever.GetContainers(filePaths);

            foreach (var container in containers)
            {
                container.RemoveZuneAttribute("WM/WMContentID");
                container.RemoveZuneAttribute("WM/WMCollectionID");
                container.RemoveZuneAttribute("WM/WMCollectionGroupID");
                container.RemoveZuneAttribute("ZuneCollectionID");
                container.RemoveZuneAttribute("WM/UniqueFileIdentifier");
                container.RemoveZuneAttribute("ZuneCollectionID");
                container.RemoveZuneAttribute("ZuneUserEditedFields");
                container.RemoveZuneAttribute(ZuneIds.Album);
                container.RemoveZuneAttribute(ZuneIds.Artist);
                container.RemoveZuneAttribute(ZuneIds.Track);
                container.WriteToFile();
            }

            Mouse.OverrideCursor = null;

            Messenger.Default.Send<ErrorMessage, ApplicationViewModel>(new ErrorMessage(ErrorMode.Warning,
                                                    "Album should now be de-linked. You may need to " +
                                                    "remove then re-add the album for the changes to take effect."));

            //force a refresh on the album to see if the de-link worked
            //this probably wont work because the zunedatabase does not correctly change the albums
            //details when delinking, but does when linking
            RefreshAlbum();
        }

        public void RefreshAlbum()
        {
            LinkStatus = LinkStatus.Unknown;
            IsDownloadingDetails = true;

            try
            {
                DbAlbum albumMetaData = _dbReader.GetAlbum(ZuneAlbumMetaData.MediaId);

                if (albumMetaData != null)
                {
                    ZuneAlbumMetaData = albumMetaData;
                    GetAlbumDetailsFromWebsite();
                }
                else
                {
                    LinkStatus = LinkStatus.Unlinked;
                    IsDownloadingDetails = false;
                }
            }
            catch (COMException)
            {
                Messenger.Default.Send<ErrorMessage, ApplicationViewModel>(new ErrorMessage(ErrorMode.Error,
                    "Could not find album, you may need to refresh the database."));
            }
        }

        public event Action DownloadCompleted = delegate { };

        public void GetAlbumDetailsFromWebsite()
        {
            Guid albumMediaId = ZuneAlbumMetaData.AlbumMediaId;

            if (albumMediaId != Guid.Empty)
            {
                var url = String.Concat(Urls.Album, albumMediaId);
                AlbumDetailsDownloader.DownloadAsync(url, album =>
                {
                    if (album != null)
                    {                              
                        LinkStatus = SharedMethods.GetAlbumLinkStatus(album.Title, album.Artist,
                                ZuneAlbumMetaData.Title, ZuneAlbumMetaData.Artist);
                        WebAlbumMetaData = album;
                    }
                    else
                    {
                        LinkStatus = LinkStatus.Unlinked;
                    }

                    IsDownloadingDetails = false;
                    DownloadCompleted.Invoke();
                });
            }
            else
            {
                LinkStatus = LinkStatus.Unlinked;
                IsDownloadingDetails = false;
                DownloadCompleted.Invoke();
            }

        }
    }
}