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
using ZuneSocialTagger.GUI.ViewsViewModels.Details;
using ZuneSocialTagger.GUI.ViewsViewModels.MoreInfo;
using ZuneSocialTagger.GUI.ViewsViewModels.Search;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    public class AlbumDetailsViewModel : ViewModelBase
    {

        private readonly IZuneDatabaseReader _dbReader;
        private readonly IViewModelLocator _locator;
        private readonly ExpandedAlbumDetailsViewModel _albumDetailsFromFile;
        private readonly IZuneAudioFileRetriever _fileRetriever;
        private bool _isDownloadingDetails;

        private DbAlbum _zuneAlbumMetaData;
        private WebAlbum _webAlbumMetaData;
        private LinkStatus _linkStatus;

        public event Action AlbumDetailsDownloaded = delegate { };

        public AlbumDetailsViewModel(IZuneDatabaseReader dbReader,
                                     IViewModelLocator locator,
                                     [File]ExpandedAlbumDetailsViewModel albumDetailsFromFile,
                                     IZuneAudioFileRetriever fileRetriever)
        {
            _dbReader = dbReader;
            _locator = locator;
            _albumDetailsFromFile = albumDetailsFromFile;
            _fileRetriever = fileRetriever;

            this.DelinkCommand = new RelayCommand(DelinkAlbum);
            this.LinkCommand = new RelayCommand(LinkAlbum);
            this.RefreshCommand = new RelayCommand(RefreshAlbum);
            this.MoreInfoCommand = new RelayCommand(ShowMoreInfo);
        }

        public AlbumDetailsViewModel()
        {
            //used for serialization purposes
        }

        #region View Bindings

        public string AlbumTitle
        {
            get { return this.ZuneAlbumMetaData.Title; }
        }

        public string AlbumArtist
        {
            get { return this.ZuneAlbumMetaData.Artist; }
        }

        public DateTime DateAdded
        {
            get { return this.ZuneAlbumMetaData.DateAdded; }
        }

        public RelayCommand MoreInfoCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand DelinkCommand { get; private set; }

        public DbAlbum ZuneAlbumMetaData
        {
            get { return _zuneAlbumMetaData; }
            set
            {
                _zuneAlbumMetaData = value;
                RaisePropertyChanged(() => this.ZuneAlbumMetaData);
            }
        }

        public WebAlbum WebAlbumMetaData
        {
            get { return _webAlbumMetaData; }
            set
            {
                _webAlbumMetaData = value;
                RaisePropertyChanged(() => this.WebAlbumMetaData);
            }
        }

        public LinkStatus LinkStatus
        {
            get { return _linkStatus; }
            set
            {
                _linkStatus = value;
                RaisePropertyChanged(() => this.LinkStatus);
                RaisePropertyChanged(() => this.CanDelink);
                RaisePropertyChanged(() => this.CanLink);
                RaisePropertyChanged(() => this.CanShowMoreInfo);
            }
        }

        public bool IsDownloadingDetails
        {
            get { return _isDownloadingDetails; }
            set
            {
                _isDownloadingDetails = value;
                RaisePropertyChanged(() => this.IsDownloadingDetails);
            }
        }

        public bool CanDelink
        {
            get { return _linkStatus != LinkStatus.Unlinked && _linkStatus != LinkStatus.Unknown; }
        }

        public bool CanLink
        {
            get { return true; }
        }

        public bool CanShowMoreInfo
        {
            get { return _linkStatus != LinkStatus.Unlinked && _linkStatus != LinkStatus.Unknown; }
        }

        #endregion

        private void ShowMoreInfo()
        {
            var moreInfoViewModel = _locator.SwitchToViewModel<MoreInfoViewModel>();
            moreInfoViewModel.SetAlbumDetails(this);
        }

        public void LinkAlbum()
        {
            DbAlbum zuneAlbumMetaData = this.ZuneAlbumMetaData;

            DoesAlbumExistInDbAndDisplayError(zuneAlbumMetaData);
            SharedMethods.SetAlbumDetails(_albumDetailsFromFile, zuneAlbumMetaData);

            try
            {
                IEnumerable<string> filePaths = _dbReader.GetTracksForAlbum(zuneAlbumMetaData.MediaId).Select(x => x.FilePath);
                _fileRetriever.GetContainers(filePaths);
                var searchVm = _locator.SwitchToViewModel<SearchViewModel>();
                searchVm.Search(zuneAlbumMetaData.Title, zuneAlbumMetaData.Artist);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<ErrorMessage, ApplicationViewModel>(new ErrorMessage(ErrorMode.Error, ex.Message));
                return;  //if we hit an error on any track in the albums then just fail and dont read anymore
            }
        }

        public void DelinkAlbum()
        {
            DoesAlbumExistInDbAndDisplayError(this.ZuneAlbumMetaData);

            Mouse.OverrideCursor = Cursors.Wait;

            //TODO: fix bug where application crashes when removing an album that is currently playing

            var filePaths = _dbReader.GetTracksForAlbum(this.ZuneAlbumMetaData.MediaId).Select(x=> x.FilePath);

            _fileRetriever.GetContainers(filePaths);

            foreach (var container in _fileRetriever.Containers)
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

            Messenger.Default.Send<ErrorMessage,ApplicationViewModel>(new ErrorMessage(ErrorMode.Warning,
                                                    "Album should now be de-linked. You may need to " +
                                                    "remove then re-add the album for the changes to take effect."));

            //force a refresh on the album to see if the de-link worked
            //this probably wont work because the zunedatabase does not correctly change the albums
            //details when delinking, but does when linking
            RefreshAlbum();
        }

        public void RefreshAlbum()
        {
            this.LinkStatus = LinkStatus.Unknown;
            this.IsDownloadingDetails = true;

            DoesAlbumExistInDbAndDisplayError(this.ZuneAlbumMetaData);

            DbAlbum albumMetaData = _dbReader.GetAlbum(this.ZuneAlbumMetaData.MediaId);

            if (albumMetaData != null)
            {
                this.ZuneAlbumMetaData = albumMetaData;
                GetAlbumDetailsFromWebsite();
            }
            else
            {
                this.LinkStatus = LinkStatus.Unlinked;
                this.IsDownloadingDetails = false;
            }
        }

        public void GetAlbumDetailsFromWebsite()
        {
            Guid albumMediaId = this.ZuneAlbumMetaData.AlbumMediaId;

            if (albumMediaId != Guid.Empty)
            {
                    var downloader = new AlbumDetailsDownloader(String.Concat(Urls.Album, albumMediaId));

                    downloader.DownloadCompleted += (dledAlbum, state) =>
                    {
                        if (state == DownloadState.Success)
                        {
                            if (dledAlbum == null)
                            {
                                this.LinkStatus = LinkStatus.Unavailable;
                            }
                            else
                            {
                                this.LinkStatus = SharedMethods.GetAlbumLinkStatus(dledAlbum.Title, dledAlbum.Artist,
                                    this.ZuneAlbumMetaData.Title, this.ZuneAlbumMetaData.Artist);
                            }

                            this.WebAlbumMetaData = dledAlbum;

                        }
                        else
                            this.LinkStatus = LinkStatus.Unavailable;

                        this.IsDownloadingDetails = false;
                        AlbumDetailsDownloaded.Invoke();
                    };

                    downloader.DownloadAsync();
                    this.IsDownloadingDetails = true;
            }
            else
            {
                this.LinkStatus = LinkStatus.Unlinked;
                this.IsDownloadingDetails = false;
            }

        }

        private void DoesAlbumExistInDbAndDisplayError(DbAlbum selectedAlbum)
        {
            if (!_dbReader.DoesAlbumExist(selectedAlbum.MediaId))
            {
                Messenger.Default.Send<ErrorMessage,ApplicationViewModel>(new ErrorMessage(ErrorMode.Error,
                    "Could not find album, you may need to refresh the database."));
            }
        }
    }
}