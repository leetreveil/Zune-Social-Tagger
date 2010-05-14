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
using ZuneSocialTagger.GUI.ViewsViewModels.MoreInfo;
using ZuneSocialTagger.GUI.ViewsViewModels.Search;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList
{
    [Serializable]
    public class AlbumDetailsViewModel : ViewModelBase
    {
        [NonSerialized]
        private readonly IZuneDatabaseReader _dbReader;

        [NonSerialized]
        private readonly ApplicationViewModel _avm;

        private DbAlbum _zuneAlbumMetaData;
        private WebAlbum _webAlbumMetaData;
        private LinkStatus _linkStatus;

        [NonSerialized]
        private RelayCommand _refreshCommand;
        [NonSerialized]
        private RelayCommand _delinkCommand;
        [NonSerialized]
        private RelayCommand _linkCommand;
        [NonSerialized]
        private RelayCommand _moreInfoCommand;

        [field: NonSerialized]
        public event Action AlbumDetailsDownloaded = delegate { };

        public AlbumDetailsViewModel(IZuneDatabaseReader dbReader,
                                        ApplicationViewModel avm)
        {
            _dbReader = dbReader;
            _avm = avm;

            this.DelinkCommand = new RelayCommand(DelinkAlbum);
            this.LinkCommand = new RelayCommand(LinkAlbum);
            this.RefreshCommand = new RelayCommand(RefreshAlbum);
            this.MoreInfoCommand = new RelayCommand(ShowMoreInfo);
        }

        public AlbumDetailsViewModel()
        {
            //used for serialization purposes
        }

        public RelayCommand MoreInfoCommand
        {
            get { return _moreInfoCommand; }
            private set { _moreInfoCommand = value; }
        }

        public RelayCommand RefreshCommand
        {
            get { return _refreshCommand; }
            private set { _refreshCommand = value; }
        }

        public RelayCommand LinkCommand
        {
            get { return _linkCommand; }
            private set { _linkCommand = value; }
        }

        public RelayCommand DelinkCommand
        {
            get { return _delinkCommand; }
            private set { _delinkCommand = value; }
        }

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

        private void ShowMoreInfo()
        {
            Messenger.Default.Send<Type,ApplicationViewModel>(typeof(MoreInfoViewModel));
            Messenger.Default.Send<AlbumDetailsViewModel,MoreInfoViewModel>(this);
        }

        public void LinkAlbum()
        {
            _avm.SongsFromFile = new List<Song>();

            var albumDetails = this.ZuneAlbumMetaData;

            DoesAlbumExistInDbAndDisplayError(albumDetails);

            IEnumerable<DbTrack> tracksForAlbum = _dbReader.GetTracksForAlbum(albumDetails.MediaId);

            foreach (DbTrack track in tracksForAlbum)
            {
                var zuneTagContainer = SharedMethods.GetContainer(track.FilePath);

                if (zuneTagContainer != null)
                    _avm.SongsFromFile.Add(new Song(track.FilePath, zuneTagContainer));
                else
                    return;
            }

            _avm.AlbumDetailsFromFile = SharedMethods.GetAlbumDetailsFrom(albumDetails);

            //tell the application to switch to the search view
            _avm.SwitchToView(typeof(SearchViewModel));
            //send the search text to the search view model after it has been constructed
            Messenger.Default.Send<string, SearchViewModel>(albumDetails.Title + " " + albumDetails.Artist);
        }

        public void DelinkAlbum()
        {
            DoesAlbumExistInDbAndDisplayError(this.ZuneAlbumMetaData);

            Mouse.OverrideCursor = Cursors.Wait;

            //TODO: fix bug where application crashes when removing an album that is currently playing

            List<DbTrack> tracksForAlbum = _dbReader.GetTracksForAlbum(this.ZuneAlbumMetaData.MediaId).ToList();

            foreach (var track in tracksForAlbum)
            {
                IZuneTagContainer zuneTagContainer = SharedMethods.GetContainer(track.FilePath);

                if (zuneTagContainer != null)
                {
                    zuneTagContainer.RemoveZuneAttribute("WM/WMContentID");
                    zuneTagContainer.RemoveZuneAttribute("WM/WMCollectionID");
                    zuneTagContainer.RemoveZuneAttribute("WM/WMCollectionGroupID");
                    zuneTagContainer.RemoveZuneAttribute("ZuneCollectionID");
                    zuneTagContainer.RemoveZuneAttribute("WM/UniqueFileIdentifier");
                    zuneTagContainer.RemoveZuneAttribute("ZuneCollectionID");
                    zuneTagContainer.RemoveZuneAttribute("ZuneUserEditedFields");
                    zuneTagContainer.RemoveZuneAttribute(ZuneIds.Album);
                    zuneTagContainer.RemoveZuneAttribute(ZuneIds.Artist);
                    zuneTagContainer.RemoveZuneAttribute(ZuneIds.Track);

                    zuneTagContainer.WriteToFile(track.FilePath);
                }
                else
                {
                    return;
                }
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
            DoesAlbumExistInDbAndDisplayError(this.ZuneAlbumMetaData);

            DbAlbum albumMetaData = _dbReader.GetAlbum(this.ZuneAlbumMetaData.MediaId);
            this.ZuneAlbumMetaData = albumMetaData;
            this.LinkStatus = LinkStatus.Unknown;
            this.WebAlbumMetaData = null;
            GetAlbumDetailsFromWebsite();
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

                    AlbumDetailsDownloaded.Invoke();
                };

                downloader.DownloadAsync();
            }
            else
                this.LinkStatus = LinkStatus.Unlinked;
        }

        private void DoesAlbumExistInDbAndDisplayError(DbAlbum selectedAlbum)
        {
            //if (!SharedMethods.CheckIfZuneSoftwareIsRunning())
            //{
            //    Messenger.Default.Send<ErrorMessage,ApplicationViewModel>(new ErrorMessage(ErrorMode.Warning, 
            //        "Any albums you link / delink will not show their changes until the zune software is running."));
            //}
            if (!_dbReader.DoesAlbumExist(selectedAlbum.MediaId))
            {
                Messenger.Default.Send<ErrorMessage,ApplicationViewModel>(new ErrorMessage(ErrorMode.Error,
                    "Could not find album, you may need to refresh the database."));
            }
        }
    }
}