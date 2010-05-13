using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using ZuneSocialTagger.GUI.Properties;
using System.Linq;
using ZuneSocialTagger.GUI.ViewsViewModels.Application;
using ZuneSocialTagger.GUI.ViewsViewModels.Search;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;
using ZuneSocialTagger.GUI.ViewsViewModels.Success;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Details
{
    public class DetailsViewModel : ViewModelBase
    {
        public DetailsViewModel()
        {
            this.AlbumDetailsFromWebsite = ApplicationViewModel.AlbumDetailsFromWeb;
            this.AlbumDetailsFromFile = ApplicationViewModel.AlbumDetailsFromFile;

            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.SaveCommand = new RelayCommand(Save);

            this.Rows = new ObservableCollection<object>();
            PopulateRows();
        }

        private void PopulateRows()
        {
            var tracksFromFile = ApplicationViewModel.SongsFromFile;

            //get lists of tracks by discNumer
            var tracksByDiscNumber =
                tracksFromFile.Select(x => x.MetaData.DiscNumber).Distinct().Select(
                    number => tracksFromFile.Where(x => x.MetaData.DiscNumber == number));

            if (tracksByDiscNumber.Count() == 1)
                tracksByDiscNumber.First().ForEach(AddRow);
            else
            {
                foreach (IEnumerable<Song> discTracks in tracksByDiscNumber)
                {
                    AddHeader(discTracks.First()); //add header for each disc before adding the tracks
                    discTracks.ForEach(AddRow);
                }
            }
        }

        private void AddHeader(Song track)
        {
            this.Rows.Add(new DiscHeader
                              {
                                  DiscNumber = "Disc " +
                                               SharedMethods.DiscNumberConverter(track.MetaData.DiscNumber)
                              });
        }

        private void AddRow(Song track)
        {
            var detailRow = new DetailRow();

            detailRow.SongDetails = new TrackWithTrackNum
                                        {
                                            TrackNumber = SharedMethods.TrackNumberConverter(track.MetaData.TrackNumber),
                                            TrackTitle = track.MetaData.Title
                                        };

            foreach (WebTrack webTrack in ApplicationViewModel.SongsFromWebsite)
            {
                detailRow.AvailableZuneTracks.Add(new TrackWithTrackNum
                                                      {
                                                          TrackNumber = webTrack.TrackNumber,
                                                          TrackTitle = webTrack.Title
                                                      });
            }

            detailRow.MatchTheSelectedSongToTheAvailableSongs();

            this.Rows.Add(detailRow);
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; private set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; private set; }
        public RelayCommand MoveBackCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public ObservableCollection<object> Rows { get; private set; }

        public bool UpdateAlbumInfo
        {
            get { return Settings.Default.UpdateAlbumInfo; }
            set
            {
                if (value != UpdateAlbumInfo)
                {
                    Settings.Default.UpdateAlbumInfo = value;
                }
            }
        }

        private void Save()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var uaeExceptions = new List<UnauthorizedAccessException>();

            foreach (var row in ApplicationViewModel.SongsFromFile)
            {
                try
                {
                    //if (row.SelectedSong != null)
                    //{
                    //    var container = row.Container;

                    //    if (row.SelectedSong.HasAllZuneIds)
                    //    {
                    //        container.RemoveZuneAttribute("WM/WMContentID");
                    //        container.RemoveZuneAttribute("WM/WMCollectionID");
                    //        container.RemoveZuneAttribute("WM/WMCollectionGroupID");
                    //        container.RemoveZuneAttribute("ZuneCollectionID");
                    //        container.RemoveZuneAttribute("WM/UniqueFileIdentifier");

                    //        container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Album, row.SelectedSong.AlbumMediaId));
                    //        container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Artist, row.SelectedSong.ArtistMediaId));
                    //        container.AddZuneAttribute(new ZuneAttribute(ZuneIds.Track, row.SelectedSong.MediaId));

                    //        //if (Settings.Default.UpdateAlbumInfo)
                    //        //container.AddMetaData(row.SelectedSong.MetaData);

                    //        container.WriteToFile(row.FilePath);
                    //    }

                    //    //TODO: run a verifier over whats been written to ensure that the tags have actually been written to file
                    //}
                }
                catch (UnauthorizedAccessException uae)
                {
                    uaeExceptions.Add(uae);
                    //TODO: better error handling
                }
            }

            if (uaeExceptions.Count > 0)
                //usually occurs when a file is readonly
                Messenger.Default.Send<ErrorMessage, ApplicationViewModel>(
                    new ErrorMessage(ErrorMode.Error,
                                     "One or more files could not be written to. Have you checked the files are not marked read-only?"));
            else
            {
                Messenger.Default.Send<Type, ApplicationViewModel>(typeof(SuccessViewModel));
            }

            Mouse.OverrideCursor = null;
        }

        private static void MoveBack()
        {
            Messenger.Default.Send<Type, ApplicationViewModel>(typeof (SearchViewModel));
        }
    }
}