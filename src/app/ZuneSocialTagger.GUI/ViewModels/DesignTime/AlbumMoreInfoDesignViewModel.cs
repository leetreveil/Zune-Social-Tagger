using System;
using System.Collections.Generic;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels.DesignTime
{
    public class AlbumMoreInfoDesignViewModel
    {
        public List<AlbumMoreInfoRow> Tracks { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }

        public AlbumMoreInfoDesignViewModel()
        {
            this.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel
                {
                    Artist = "Pendulum",
                    ArtworkUrl = "http://images.play.com/covers/12691916x.jpg",
                    SongCount = "10",
                    Title = "Immersion",
                    Year = "2010"
                };

            this.AlbumDetailsFromWebsite = new ExpandedAlbumDetailsViewModel
                {
                    Artist = "Pendulum",
                    ArtworkUrl = "http://images.play.com/covers/12691916x.jpg",
                    SongCount = "10",
                    Title = "Immersion",
                    Year = "2010"
                };

            this.Tracks = new List<AlbumMoreInfoRow>();

            this.Tracks.Add(new AlbumMoreInfoRow
                                {
                                    TrackFromFile = new DetailRowSong { TrackNumber = "1", TrackTitle = "Prelude" }, 
                                    LinkStatusImage = new Uri("pack://application:,,,/Assets/yes.png"),
                                    TrackFromWeb = new DetailRowSong { TrackNumber = "1", TrackTitle = "Prelude (Album)" }
                                });
            this.Tracks.Add(new AlbumMoreInfoRow
                                {
                                    TrackFromFile = new DetailRowSong { TrackNumber = "2", TrackTitle = "Slam" }, 
                                    LinkStatusImage = new Uri("pack://application:,,,/Assets/no.png"),
                                    LinkStatusText = "UNLINKED"
                                });
            this.Tracks.Add(new AlbumMoreInfoRow
            {
                TrackFromFile = new DetailRowSong { TrackNumber = "1", TrackTitle = "Some really really really really really really long album title" },
                LinkStatusImage = new Uri("pack://application:,,,/Assets/yes.png"),
                TrackFromWeb = new DetailRowSong { TrackNumber = "1", TrackTitle = "Prelude (Album)" }
            });



            //this.Tracks.Add(new AlbumMoreInfoRow { TrackFromFile = new DetailRowSong { TrackNumber = "3", TrackTitle = "Plasticworld" } });
            //this.Tracks.Add(new AlbumMoreInfoRow { TrackFromFile = new DetailRowSong { TrackNumber = "4", TrackTitle = "Fasten Your Seatbelt" } });
            //this.Tracks.Add(new AlbumMoreInfoRow { TrackFromFile = new DetailRowSong { TrackNumber = "5", TrackTitle = "Through The Loop" } });
            //this.Tracks.Add(new AlbumMoreInfoRow { TrackFromFile = new DetailRowSong { TrackNumber = "6", TrackTitle = "Sounds Of Life" } });
            //this.Tracks.Add(new AlbumMoreInfoRow { TrackFromFile = new DetailRowSong { TrackNumber = "7", TrackTitle = "Girl In The Fire" } });
            //this.Tracks.Add(new AlbumMoreInfoRow { TrackFromFile = new DetailRowSong { TrackNumber = "8", TrackTitle = "Tarantula" } });
            //this.Tracks.Add(new AlbumMoreInfoRow { TrackFromFile = new DetailRowSong { TrackNumber = "9", TrackTitle = "Out Here" } });
        }
    }
}