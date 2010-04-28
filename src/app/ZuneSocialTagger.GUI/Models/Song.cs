using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.GUI.Models
{
    /// <summary>
    /// Every row in the DetailsView has this information.
    /// </summary>
    public class Song
    {
        public MetaData MetaData { get; set; }
        public string FilePath { get; private set; }
        public IZuneTagContainer Container { get; set; }
        public Track SelectedSong { get; set; }

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

        public Song()
        {
  
        }

        private static string TrackNumberCleaner(string trackNumber)
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
        /// Matches song titles
        /// </summary>
        /// <returns></returns>
        public Track MatchThisSongToAvailableSongs(IEnumerable<Track> tracksToMatch)
        {
            //this matches album songs to zune website songs in the details view
            Track matchBySongTitle =
                tracksToMatch.Where(song => song.MetaData.Title.ToLower() == this.MetaData.Title.ToLower()).FirstOrDefault();

            //Track matchByTrackNumber =
            //    tracksToMatch.Where(song => song.MetaData.TrackNumber == this.MetaData.TrackNumber).FirstOrDefault();

            //if (matchBySongTitle == null && matchByTrackNumber != null)
            //{
            //    return matchByTrackNumber;
            //}

            return matchBySongTitle ?? new Track();
        }
    }
}