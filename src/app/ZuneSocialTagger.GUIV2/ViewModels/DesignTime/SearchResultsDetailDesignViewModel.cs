using System.Collections.Generic;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.GUIV2.ViewModels.DesignTime
{
    public class SearchResultsDetailDesignViewModel
    {
        public SearchResultsDetailDesignViewModel()
        {
            var metaData1 = new MetaData() { TrackNumber = "01", Title = "Prelude" };
            var metaData2 = new MetaData() { TrackNumber = "02", Title = "Slam" };
            var metaData3 = new MetaData() { TrackNumber = "03", Title = "Plasticworld" };
            var metaData4 = new MetaData() { TrackNumber = "04", Title = "Fasten Your Seatbelt" };
            var metaData5 = new MetaData() { TrackNumber = "05", Title = "Through The Loop" };
            var metaData6 = new MetaData() { TrackNumber = "06", Title = "Sounds Of Life" };
            var metaData7 = new MetaData() { TrackNumber = "07", Title = "Girl In The Fire" };
            var metaData8 = new MetaData() { TrackNumber = "08", Title = "Tarantula" };
            var metaData9 = new MetaData() { TrackNumber = "09", Title = "Out Here" };

            var track1 = new Track() { MetaData = metaData1 };
            var track2 = new Track() { MetaData = metaData2 }; 
            var track3 = new Track() { MetaData = metaData3 };          
            var track4 = new Track() { MetaData = metaData4 };  
            var track5 = new Track() { MetaData = metaData5 }; 
            var track6 = new Track() { MetaData = metaData6 };
            var track7 = new Track() { MetaData = metaData7 }; 
            var track8 = new Track() { MetaData = metaData8 };  
            var track9 = new Track() { MetaData = metaData9 };

            this.SelectedAlbumSongs = new List<Track>();

            this.SelectedAlbumSongs.Add(track1);
            this.SelectedAlbumSongs.Add(track2);
            this.SelectedAlbumSongs.Add(track3);
            this.SelectedAlbumSongs.Add(track4);
            this.SelectedAlbumSongs.Add(track5);
            this.SelectedAlbumSongs.Add(track6);
            this.SelectedAlbumSongs.Add(track7);
            this.SelectedAlbumSongs.Add(track8);
            this.SelectedAlbumSongs.Add(track9);
        }

        public string SelectedAlbumTitle { get { return "Hold Your Colour"; } }
        public List<Track> SelectedAlbumSongs { get; set; }
    }
}