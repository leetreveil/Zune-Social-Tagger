using System.Collections.Generic;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.DetailsReadyOnly.DesignTime
{
    public class DetailsReadOnlyDesignViewModel
    {
        public string ImageUrl { get; set; }
        public string Artist { get; set; }
        public string AlbumTitle { get; set; }
        public string ReleaseYear { get; set; }
        public string TrackCount { get; set; }
        public List<TrackWithTrackNum> Tracks { get; set; }
        public string Genre { get; set; }

        public DetailsReadOnlyDesignViewModel()
        {
            this.Genre = "Pop";
            this.Artist = "65daysofstatic";
            this.AlbumTitle = "We Were Exploding Anyway";
            this.ReleaseYear = "2010";
            this.TrackCount = "9 Songs";
            this.Tracks = new List<TrackWithTrackNum>();
            this.Tracks.Add(new TrackWithTrackNum {TrackNumber = "01",TrackTitle = "Mountainhead"});
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "02", TrackTitle = "Crash Tactics" });
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "03", TrackTitle = "Dance Dance Dance" });
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "04", TrackTitle = "Piano Fights" });
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "05", TrackTitle = "Weak04" });
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "01", TrackTitle = "Mountainhead" });
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "02", TrackTitle = "Crash Tactics" });
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "03", TrackTitle = "Dance Dance Dance" });
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "04", TrackTitle = "Piano Fights" });
            this.Tracks.Add(new TrackWithTrackNum { TrackNumber = "05", TrackTitle = "Weak04" });
        }
    }
}