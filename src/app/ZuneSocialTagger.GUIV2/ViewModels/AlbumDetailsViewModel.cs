using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Caliburn.Core;
using Caliburn.PresentationFramework;
using Microsoft.Practices.ServiceLocation;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Views;
using Album = ZuneSocialTagger.Core.ZuneDatabase.Album;
using Track = ZuneSocialTagger.Core.ZuneDatabase.Track;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class AlbumDetailsViewModel : PropertyChangedBase
    {
        private readonly IServiceLocator _locator;
        private readonly IZuneWizardModel _model;
        private readonly IZuneDbAdapter _dbReader;
        private Album _zuneAlbumMetaData;
        private Album _webAlbumMetaData;
        private LinkStatus _linkStatus;

        public AlbumDetailsViewModel(IServiceLocator locator, IZuneWizardModel model, IZuneDbAdapter dbAdapter)
        {
            _locator = locator;
            _model = model;
            _dbReader = dbAdapter;
        }

        private AlbumDetailsViewModel()
        {
            //used for serialization purposes
        }

        public Album ZuneAlbumMetaData
        {
            get { return _zuneAlbumMetaData; }
            set
            {
                _zuneAlbumMetaData = value;
                NotifyOfPropertyChange(() => this.ZuneAlbumMetaData);
            }
        }

        public Album WebAlbumMetaData
        {
            get { return _webAlbumMetaData; }
            set
            {
                _webAlbumMetaData = value;
                NotifyOfPropertyChange(() => this.WebAlbumMetaData);
            }
        }

        public LinkStatus LinkStatus
        {
            get { return _linkStatus; }
            set
            {
                _linkStatus = value;
                NotifyOfPropertyChange(() => this.LinkStatus);
            }
        }

        public void LinkAlbum()
        {
            if (!DoesAlbumExistInDbAndDisplayError()) return;

            Album albumDetails = this.ZuneAlbumMetaData;

            IEnumerable<Track> tracksForAlbum = _dbReader.GetTracksForAlbum(albumDetails.MediaId);

            _model.Rows = new BindableCollection<DetailRow>();

            foreach (var track in tracksForAlbum)
            {
                try
                {
                    //TODO: do we really want to not be able to link an album if just one track cant be read?
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(track.FilePath);

                    _model.Rows.Add(new DetailRow(track.FilePath, container));
                }
                catch (AudioFileReadException ex)
                {
                    ZuneMessageBox.Show(ex.Message, ErrorMode.Error);
                    return;
                }
            }

            _model.SearchHeader.SearchBar.SearchText = albumDetails.AlbumArtist + " " + albumDetails.AlbumTitle;

            _model.SearchHeader.AlbumDetails = new ExpandedAlbumDetailsViewModel
            {
                Artist = albumDetails.AlbumArtist,
                Title = albumDetails.AlbumTitle,
                ArtworkUrl = albumDetails.ArtworkUrl,
                SongCount = albumDetails.TrackCount.ToString(),
                Year = albumDetails.ReleaseYear.ToString()
            };

            _model.CurrentPage = _locator.GetInstance<SearchViewModel>();
        }

        public void DelinkAlbum()
        {
            if (!DoesAlbumExistInDbAndDisplayError()) return;

            Mouse.OverrideCursor = Cursors.Wait;

            //TODO: fix bug where application crashes when removing an album that is currently playing

            var tracksForAlbum = _dbReader.GetTracksForAlbum(this.ZuneAlbumMetaData.MediaId).ToList();

            //_dbReader.RemoveAlbumFromDatabase(this.ZuneAlbumMetaData.MediaId);

            foreach (var track in tracksForAlbum)
            {
                try
                {
                    IZuneTagContainer container = ZuneTagContainerFactory.GetContainer(track.FilePath);

                    _model.Rows.Add(new DetailRow(track.FilePath, container));

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
                catch (AudioFileReadException ex)
                {
                    ZuneMessageBox.Show(ex.Message, ErrorMode.Error);
                    return;
                }

            }

            //foreach (var track in tracksForAlbum)
            //    _dbReader.AddTrackToDatabase(track.FilePath);

            Mouse.OverrideCursor = null;

            //TODO: change this to an information message box because it is not an error
            ZuneMessageBox.Show("Album should now be de-linked. You may need to " +
                                "remove then re-add the album for the changes to take effect.", ErrorMode.Warning);
        }

        public void RefreshAlbum()
        {
            if (!DoesAlbumExistInDbAndDisplayError()) return;

            ThreadPool.QueueUserWorkItem(_ =>
             {
                 Album albumMetaData = _dbReader.GetAlbum(this.ZuneAlbumMetaData.MediaId).ZuneAlbumMetaData;

                 this.LinkStatus = LinkStatus.Unknown;


                 string url = String.Concat(Urls.Album, albumMetaData.AlbumMediaId);

                 if (albumMetaData.AlbumMediaId != Guid.Empty)
                 {
                     var downloader = new AlbumDetailsDownloader(url);

                     downloader.DownloadCompleted += alb =>
                         {
                             this.LinkStatus = SharedMethods.GetAlbumLinkStatus(alb.AlbumTitle,
                                                                                alb.AlbumArtist,
                                                                                albumMetaData.AlbumTitle,
                                                                                albumMetaData.AlbumArtist);

                             this.WebAlbumMetaData = new Album
                                                         {
                                                             AlbumArtist = alb.AlbumArtist,
                                                             AlbumTitle = alb.AlbumTitle,
                                                             ArtworkUrl = alb.ArtworkUrl
                                                         };
                         };

                     downloader.DownloadAsync();
                 }
                 else
                 {
                     this.LinkStatus = LinkStatus.Unlinked;
                 }
             });
        }

        private bool DoesAlbumExistInDbAndDisplayError()
        {
            bool doesAlbumExist = _dbReader.DoesAlbumExist(this.ZuneAlbumMetaData.MediaId);

            if (!doesAlbumExist)
            {
                SharedMethods.ShowCouldNotFindAlbumError();
                return false;
            }

            return true;
        }
    }
}