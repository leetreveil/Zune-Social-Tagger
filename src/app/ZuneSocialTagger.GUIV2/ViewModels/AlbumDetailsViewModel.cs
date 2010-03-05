using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Caliburn.Core;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;
using Album = ZuneSocialTagger.GUIV2.Models.Album;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class AlbumDetailsViewModel : PropertyChangedBase
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;
        private readonly IZuneDatabaseReader _dbReader;
        private Album _album;

        public AlbumDetailsViewModel(Album album, IUnityContainer container,
                                         IZuneWizardModel model,
                                         IZuneDatabaseReader dbReader)
        {
            _container = container;
            _model = model;
            _dbReader = dbReader;
            this.Album = album;
        }


        public Album Album
        {
            get { return _album; }
            set
            {
                _album = value;
                NotifyOfPropertyChange(() => this.Album);
            }
        }

        public void LinkAlbum()
        {
            var doesAlbumExist = _dbReader.DoesAlbumExist(this.Album.ZuneAlbumMetaData.MediaId);

            if (!doesAlbumExist)
            {
                ShowCouldNotFindAlbumError();
            }
            else
            {
                AlbumDetails albumDetails = this.Album.ZuneAlbumMetaData;

                _model.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel
                {
                    Artist = albumDetails.AlbumArtist,
                    Title = albumDetails.AlbumTitle,
                    ArtworkUrl = albumDetails.ArtworkUrl,
                    SongCount = albumDetails.TrackCount.ToString(),
                    Year = albumDetails.ReleaseYear.ToString()
                };

                IEnumerable<Track> tracksForAlbum = _dbReader.GetTracksForAlbum(albumDetails.MediaId);

                _model.Rows.Clear();

                foreach (var track in tracksForAlbum)
                    _model.Rows.Add(new DetailRow(track.FilePath, ZuneTagContainerFactory.GetContainer(track.FilePath)));

                var searchViewModel = _container.Resolve<SearchViewModel>();

                searchViewModel.SearchHeader.SearchBar.SearchText = albumDetails.AlbumArtist + " " +
                                                       albumDetails.AlbumTitle;

                searchViewModel.SearchHeader.AlbumDetails = _model.AlbumDetailsFromFile;

                _model.CurrentPage = searchViewModel;
            }
        }


        public void DelinkAlbum()
        {
            var doesAlbumExist = _dbReader.DoesAlbumExist(this.Album.ZuneAlbumMetaData.MediaId);

            if (!doesAlbumExist)
                ShowCouldNotFindAlbumError();

            else
            {
                Mouse.OverrideCursor = Cursors.Wait;

                //TODO: fix bug where application crashes when removing an album that is currently playing

                var tracksForAlbum = _dbReader.GetTracksForAlbum(this.Album.ZuneAlbumMetaData.MediaId).ToList();

                _dbReader.RemoveAlbumFromDatabase(this.Album.ZuneAlbumMetaData.MediaId);

                //Thread.Sleep(5000);

                foreach (var track in tracksForAlbum)
                {
                    var container = ZuneTagContainerFactory.GetContainer(track.FilePath);

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

                    container.WriteToFile(track.FilePath);
                }


                foreach (var track in tracksForAlbum)
                    _dbReader.AddTrackToDatabase(track.FilePath);

                Mouse.OverrideCursor = null;

                //TODO: change this to an information message box because it is not an error
                ZuneMessageBox.Show("Album should now be de-linked. You may need to " +
                                    "remove then re-add the album for the changes to take effect.", ErrorMode.Warning);
            }
        }

        public void RefreshAlbum()
        {
            var doesAlbumExist = _dbReader.DoesAlbumExist(this.Album.ZuneAlbumMetaData.MediaId);

            if (!doesAlbumExist)
                ShowCouldNotFindAlbumError();
            else
            {
                var newAlbum = Album.GetNewAlbum(_dbReader.GetAlbum(this.Album.ZuneAlbumMetaData.MediaId));

                string url = String.Concat("http://catalog.zune.net/v3.0/en-US/music/album/",
                                           newAlbum.ZuneAlbumMetaData.AlbumMediaId);

                var downloader = new AlbumDetailsDownloader(url);

                downloader.DownloadCompleted += alb =>
                                                    {
                                                        newAlbum.LinkStatus = Album.GetAlbumLinkStatus(alb.AlbumTitle,
                                                                                                       alb.AlbumArtist,
                                                                                                       newAlbum.ZuneAlbumMetaData);

                                                        newAlbum.WebAlbumMetaData  = new AlbumDetails{AlbumArtist = alb.AlbumArtist,
                                                            AlbumTitle = alb.AlbumTitle,ArtworkUrl = alb.ArtworkUrl};

                                                        this.Album = newAlbum;
                                                    };

                downloader.DownloadAsync();
            }
        }


        private void ShowCouldNotFindAlbumError()
        {
            ZuneMessageBox.Show("Could not find album, you may need to refresh the database.", ErrorMode.Error);
        }


    }
}