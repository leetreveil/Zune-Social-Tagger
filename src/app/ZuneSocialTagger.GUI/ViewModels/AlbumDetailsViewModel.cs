using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class AlbumDetailsViewModel : ViewModelBaseExtended
    {
        private readonly IZuneDatabaseReader _dbReader;
        private readonly ZuneWizardModel _model;
        private Album _zuneAlbumMetaData;
        private Album _webAlbumMetaData;
        private LinkStatus _linkStatus;

        public AlbumDetailsViewModel(IZuneDatabaseReader dbReader, ZuneWizardModel model)
        {
            _dbReader = dbReader;
            _model = model;

            this.DelinkCommand = new RelayCommand(DelinkAlbum);
            this.LinkCommand = new RelayCommand(LinkAlbum);
            this.RefreshCommand = new RelayCommand(RefreshAlbum);
        }

        public AlbumDetailsViewModel()
        {
            //used for serialization purposes
        }

        [XmlIgnore]
        public RelayCommand RefreshCommand { get; private set; }
        [XmlIgnore]
        public RelayCommand LinkCommand { get; private set; }
        [XmlIgnore]
        public RelayCommand DelinkCommand { get; private set; }

        public Album ZuneAlbumMetaData
        {
            get { return _zuneAlbumMetaData; }
            set
            {
                _zuneAlbumMetaData = value;
                RaisePropertyChanged(() => this.ZuneAlbumMetaData);
            }
        }

        public Album WebAlbumMetaData
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
            }
        }

        [XmlIgnore]
        public bool CanDelink
        {
            get 
            {
                return _linkStatus != LinkStatus.Unlinked && _linkStatus != LinkStatus.Unknown;
            }
        }

        [XmlIgnore]
        public bool CanLink
        {
            get 
            {
                return true;
            }
        }

        public void LinkAlbum()
        {
            var albumDetails = this.ZuneAlbumMetaData;

            if (!DoesAlbumExistInDbAndDisplayError(albumDetails)) return;

            IEnumerable<Track> tracksForAlbum = _dbReader.GetTracksForAlbum(albumDetails.MediaId);

            var selectedAlbum = new SelectedAlbum();

            foreach (var track in tracksForAlbum)
            {
                var zuneTagContainer = SharedMethods.GetContainer(track.FilePath);

                if (zuneTagContainer != null)
                    selectedAlbum.Tracks.Add(new Song(track.FilePath, zuneTagContainer));
                else
                    return;
            }

            selectedAlbum.ZuneAlbumMetaData = new ExpandedAlbumDetailsViewModel
                                                    {
                                                        Artist = albumDetails.Artist,
                                                        Title = albumDetails.Title,
                                                        ArtworkUrl = albumDetails.ArtworkUrl,
                                                        SongCount = albumDetails.TrackCount.ToString(),
                                                        Year = albumDetails.ReleaseYear.ToString()
                                                    };

            selectedAlbum.AlbumDetails = this;

            _model.SelectedAlbum = selectedAlbum;
            _model.SearchText = albumDetails.Title + " " + albumDetails.Artist;

            //tell the application to switch to the search view
            Messenger.Default.Send(typeof(SearchViewModel));
            
        }

        public void DelinkAlbum()
        {
            if (!DoesAlbumExistInDbAndDisplayError(this.ZuneAlbumMetaData)) return;

            Mouse.OverrideCursor = Cursors.Wait;

            //TODO: fix bug where application crashes when removing an album that is currently playing

            List<Track> tracksForAlbum = _dbReader.GetTracksForAlbum(this.ZuneAlbumMetaData.MediaId).ToList();


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

            Messenger.Default.Send(new ErrorMessage(ErrorMode.Warning,
                                                    "Album should now be de-linked. You may need to " +
                                                    "remove then re-add the album for the changes to take effect."));

            //force a refresh on the album to see if the de-link worked
            //this probably wont work because the zunedatabase does not correctly change the albums
            //details when delinking, but does when linking
            RefreshAlbum();
        }

        public void RefreshAlbum()
        {
            if (!DoesAlbumExistInDbAndDisplayError(this.ZuneAlbumMetaData)) return;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Album albumMetaData = _dbReader.GetAlbum(this.ZuneAlbumMetaData.MediaId);

                this.LinkStatus = LinkStatus.Unknown;

                if (albumMetaData.AlbumMediaId != Guid.Empty)
                {
                    var downloader = new AlbumDetailsDownloader(String.Concat(Urls.Album, albumMetaData.AlbumMediaId));

                    downloader.DownloadCompleted += (alb, state) =>
                    {
                        if (state == DownloadState.Success)
                        {
                            if (alb != null)
                            {
                                this.LinkStatus = SharedMethods.GetAlbumLinkStatus(alb.Title,
                                                            alb.Artist,
                                                            albumMetaData.Title,
                                                            albumMetaData.Artist);

                                this.WebAlbumMetaData = new Album
                                {
                                    Artist = alb.Artist,
                                    Title = alb.Title,
                                    ArtworkUrl = alb.ArtworkUrl
                                };
                            }
                            else
                            {
                                this.LinkStatus = LinkStatus.Unavailable;
                            }
                        }
                        else
                        {
                            this.LinkStatus = LinkStatus.Unavailable;
                            Messenger.Default.Send(new ErrorMessage(ErrorMode.Error,"Failed to communicate with zune server"));
                        }

                    };

                    downloader.DownloadAsync();
                }
                else
                {
                    this.LinkStatus = LinkStatus.Unlinked;
                }
            });
        }

        private bool DoesAlbumExistInDbAndDisplayError(Album selectedAlbum)
        {
            bool doesAlbumExist = _dbReader.DoesAlbumExist(selectedAlbum.MediaId);

            if (!doesAlbumExist)
            {
                Messenger.Default.Send(new ErrorMessage(ErrorMode.Error,
                                                        "Could not find album, you may need to refresh the database."));
                return false;
            }

            return true;
        }
    }
}