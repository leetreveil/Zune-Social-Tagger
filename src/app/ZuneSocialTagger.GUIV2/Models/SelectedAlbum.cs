using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class SelectedAlbum
    {
        public SelectedAlbum()
        {
            this.Tracks = new ObservableCollection<Song>();
        }
        /// <summary>
        /// The selected albums view model
        /// </summary>
        public AlbumDetailsViewModel AlbumDetails { get; set; }
        public ObservableCollection<Song> Tracks { get; set; }
        public ExpandedAlbumDetailsViewModel ZuneAlbumMetaData { get; set; }
        public ExpandedAlbumDetailsViewModel WebAlbumMetaData { get; set; }
        public ObservableCollection<Track> SongsFromWebsite { get; set; }
    }
}