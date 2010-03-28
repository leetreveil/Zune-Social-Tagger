using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.GUIV2.Models
{
    /// <summary>
    /// Every row in the DetailsView has this information.
    /// </summary>
    public class Song
    {
        private ObservableCollection<Track> _songsFromWebsite;

        public MetaData MetaData { get; set; }
        public string FilePath { get; private set; }
        public IZuneTagContainer Container { get; set; }
        public Track SelectedSong { get; set; }
        public IEnumerable<Track> Tracks { get; set; }

        public Song(string filePath, IZuneTagContainer container)
        {
            FilePath = filePath;
            Container = container;
            this.MetaData = container.ReadMetaData();

            this.MetaData.TrackNumber = TrackNumberCleaner(this.MetaData.TrackNumber);

            //use the first contributing artist if the album artist does not exist
            if (string.IsNullOrEmpty(this.MetaData.AlbumArtist))
                this.MetaData.AlbumArtist = this.MetaData.ContributingArtists.First();
        }

        private string TrackNumberCleaner(string trackNumber)
        {
            if (trackNumber == string.Empty)
            {
                return "0";
            }

            if (trackNumber.Contains('/'))
            {
                return trackNumber.Split('/').First();
            }

            return trackNumber;
        }

        /// <summary>
        /// when this is first set we try to match the songs from the zune website to whats in the songs metadata
        /// </summary>
        public ObservableCollection<Track> SongsFromWebsite
        {
            get { return _songsFromWebsite; }
            set
            {
                _songsFromWebsite = value;

                //update selected song
                SelectedSong = MatchThisSongToAvailableSongs();
            }
        }

        /// <summary>
        /// Matches song titles
        /// </summary>
        /// <returns></returns>
        private Track MatchThisSongToAvailableSongs()
        {
            //this matches album songs to zune website songs in the details view
            IEnumerable<Track> matchedSongs =
                this.SongsFromWebsite.Where(song => song.MetaData.Title.ToLower() == this.MetaData.Title.ToLower());

            return matchedSongs.Count() > 0 ? matchedSongs.First() : new Track();
        }
    }
}