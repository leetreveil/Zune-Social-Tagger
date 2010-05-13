using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZuneSocialTagger.Core.ZuneDatabase;
using ZuneSocialTagger.Core.ZuneWebsite;
using ZuneSocialTagger.GUI.Models;
using System.Linq;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class AlbumMoreInfoViewModel : ViewModelBase
    {
        public AlbumMoreInfoViewModel()
        {
            Messenger.Default.Register<AlbumDetailsViewModel>(this, GotAlbumDetails);

            this.MoveBackCommand = new RelayCommand(MoveBack);
            this.Tracks = new List<AlbumMoreInfoRow>();
        }

        private void GotAlbumDetails(AlbumDetailsViewModel albumDetails)
        {
            this.AlbumDetailsFromFile = albumDetails.ZuneAlbumMetaData.GetAlbumDetailsFrom();
            this.AlbumDetailsFromWebsite = albumDetails.WebAlbumMetaData.GetAlbumDetailsFrom();

            foreach (DbTrack dbTrack in albumDetails.ZuneAlbumMetaData.Tracks)
            {
                var albumMoreInfoRow = new AlbumMoreInfoRow();

                albumMoreInfoRow.TrackFromFile = new DetailRowSong
                                                     {
                                                         TrackTitle = dbTrack.Title,
                                                         TrackNumber = dbTrack.TrackNumber
                                                     };


                WebTrack webTrack =
                    albumDetails.WebAlbumMetaData.Tracks.Where(x => x.MediaId == dbTrack.MediaId).FirstOrDefault();

                if (webTrack != null)
                {
                    albumMoreInfoRow.TrackFromWeb = new DetailRowSong
                    {
                        TrackNumber = webTrack.TrackNumber,
                        TrackTitle = webTrack.Title
                    };

                    albumMoreInfoRow.LinkStatusImage = new Uri("pack://application:,,,/Assets/yes.png");
                }
                else
                {
                    albumMoreInfoRow.LinkStatusImage = new Uri("pack://application:,,,/Assets/no.png");
                    albumMoreInfoRow.LinkStatusText = "UNLINKED";
                }



                this.Tracks.Add(albumMoreInfoRow);
            }
        }

        public RelayCommand MoveBackCommand { get; private set; }
        public List<AlbumMoreInfoRow> Tracks { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }

        private void MoveBack()
        {
            Messenger.Default.Send<string, ApplicationViewModel>("SWITCHTOFIRSTVIEW");
        }
    }
}