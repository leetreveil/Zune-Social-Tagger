using System.Collections.Generic;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Details.DesignTime
{
    public class DetailsDesignViewModel
    {
        public DetailsDesignViewModel()
        {
            this.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel
                                            {
                                                Artist = "Pendulum",
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

            this.Rows = new List<object>();

            this.Rows.Add(new DiscHeader { DiscNumber = "Disc 1" });
            this.Rows.Add(new DetailRow {SongDetails = new TrackWithTrackNum{TrackNumber = "1",TrackTitle = "Prelude"}});
            this.Rows.Add(new DetailRow { SongDetails = new TrackWithTrackNum { TrackNumber = "2", TrackTitle = "Slam" } });
            this.Rows.Add(new DetailRow { SongDetails = new TrackWithTrackNum { TrackNumber = "3", TrackTitle = "Plasticworld" } });
            this.Rows.Add(new DiscHeader{DiscNumber = "Disc 2"});
            this.Rows.Add(new DetailRow { SongDetails = new TrackWithTrackNum { TrackNumber = "5", TrackTitle = "Through The Loop" } });
            this.Rows.Add(new DetailRow { SongDetails = new TrackWithTrackNum { TrackNumber = "6", TrackTitle = "Sounds Of Life" } });
            this.Rows.Add(new DetailRow { SongDetails = new TrackWithTrackNum { TrackNumber = "7", TrackTitle = "Girl In The Fire" } });
            this.Rows.Add(new DetailRow { SongDetails = new TrackWithTrackNum { TrackNumber = "8", TrackTitle = "Tarantula" } });
            this.Rows.Add(new DetailRow { SongDetails = new TrackWithTrackNum { TrackNumber = "9", TrackTitle = "Out Here" } });
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }
        public List<object> Rows { get; set; }
    }
}