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
        private readonly IViewLocator _locator;
        private readonly ExpandedAlbumDetailsViewModel _albumDetailsFromFile;
        private readonly IZuneAudioFileRetriever _fileRetriever;
        private readonly SharedModel _sharedModel;
        private bool _isDownloadingDetails;
        private DbAlbum _zuneAlbumMetaData;
        private WebAlbum _webAlbumMetaData;
        private LinkStatus _linkStatus;

        public AlbumDetailsViewModel(IZuneDatabaseReader dbReader,
                                     IViewLocator locator,
                                     [File]ExpandedAlbumDetailsViewModel albumDetailsFromFile,
                                     IZuneAudioFileRetriever fileRetriever,
                                     SharedModel sharedModel)
        {
            _dbReader = dbReader;
            _locator = locator;
            _albumDetailsFromFile = albumDetailsFromFile;
            _fileRetriever = fileRetriever;
            _sharedModel = sharedModel;

            this.LinkCommand = new RelayCommand(LinkAlbum);
            this.RefreshCommand = new RelayCommand(RefreshAlbum);
            this.MoreInfoCommand = new RelayCommand(ShowMoreInfo);
        }

        public AlbumDetailsViewModel()
        {
            //used for design-time purposes
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

        public bool CanLink
        {
            get { return true; }
        }

        public bool CanShowMoreInfo
        {
            get { return _linkStatus != LinkStatus.Unlinked && _linkStatus != LinkStatus.Unknown; }
        }

        public bool CanRefresh   
        {
            get { return SharedMethods.CheckIfZuneSoftwareIsRunning(); }
        }

        #endregion

        private void ShowMoreInfo()
        {
            var moreInfoViewModel = _locator.SwitchToView<MoreInfoView,MoreInfoViewModel>();
            moreInfoViewModel.SetAlbumDetails(this);
        }

        public void LinkAlbum()
        {
            DbAlbum zuneAlbumMetaData = this.ZuneAlbumMetaData;

            DoesAlbumExistInDbAndDisplayError(zuneAlbumMetaData);
            //set the expanded album details view mod that is used throughout the app
            SetAlbumDetails(_albumDetailsFromFile, zuneAlbumMetaData);

            try
            {
                IEnumerable<string> filePaths = zuneAlbumMetaData.Tracks.Select(x => x.FilePath);
                _sharedModel.SongsFromFile = _fileRetriever.GetContainers(filePaths);
                _sharedModel.AlbumDetailsFromFile = _albumDetailsFromFile;
                var searchVm = _locator.SwitchToView<SearchView,SearchViewModel>();
                searchVm.Search(zuneAlbumMetaData.Artist, zuneAlbumMetaData.Title);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<ErrorMessage, ApplicationViewModel>(new ErrorMessage(ErrorMode.Error, ex.Message));
                return;  //if we hit an error on any track in the albums then just fail and dont read anymore
            }
        }

        public void RefreshAlbum()
        {
            this.LinkStatus = LinkStatus.Unknown;
            this.IsDownloadingDetails = true;

            var exists = DoesAlbumExistInDbAndDisplayError(this.ZuneAlbumMetaData);

            if (exists)
            {
                DbAlbum albumMetaData = _dbReader.GetAlbum(this.ZuneAlbumMetaData.MediaId);

                if (albumMetaData != null)
                {
                    this.ZuneAlbumMetaData = albumMetaData;
                    GetAlbumDetailsFromWebsite(null);
                }
                else
                {
                    this.LinkStatus = LinkStatus.Unlinked;
                    this.IsDownloadingDetails = false;
                }
            }
        }

        public void GetAlbumDetailsFromWebsite(Action callback)
        {
            Guid albumMediaId = this.ZuneAlbumMetaData.AlbumMediaId;

            if (albumMediaId != Guid.Empty)
            {
                var url = String.Concat(Urls.Album, albumMediaId);
                AlbumDetailsDownloader.DownloadAsync(url, album =>
                {
                    if (album != null)
                    {                              
                        this.LinkStatus = SharedMethods.GetAlbumLinkStatus(album.Title, album.Artist,
                                this.ZuneAlbumMetaData.Title, this.ZuneAlbumMetaData.Artist);
                        this.WebAlbumMetaData = album;
                    }
                    else
                    {
                        this.LinkStatus = LinkStatus.Unavailable;
                    }

                    this.IsDownloadingDetails = false;

                    if (callback != null)
                        callback.Invoke();
                });
            }
            else
            {
                this.LinkStatus = LinkStatus.Unlinked;
                this.IsDownloadingDetails = false;
            }

        }

        private static void SetAlbumDetails(ExpandedAlbumDetailsViewModel details, DbAlbum album)
        {
            details.Artist = album.Artist;
            details.Title = album.Title;
            details.ArtworkUrl = album.ArtworkUrl;
            details.SongCount = album.TrackCount.ToString();
            details.Year = album.ReleaseYear;
        }

        private bool DoesAlbumExistInDbAndDisplayError(DbAlbum selectedAlbum)
        {
            if (!_dbReader.DoesAlbumExist(selectedAlbum.MediaId))
            {
                Messenger.Default.Send<ErrorMessage,ApplicationViewModel>(new ErrorMessage(ErrorMode.Error,
                    "Could not find album, you may need to refresh the database."));

                return false;
            }

            return true;
        }
    }
}