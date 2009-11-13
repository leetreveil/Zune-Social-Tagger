using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class DetailRow
    {
        private SongWithNumberAndGuid _selectedSong;
        private ObservableCollection<SongWithNumberAndGuid> _songsFromWebsite;

        public string SongTitle { get; set; }
        public string FilePath { get; set; }
        public ZuneTagContainer TagContainer { get; set; }

        public DetailRow(string songTitle, string filePath, ZuneTagContainer container)
        {
            this.SongTitle = songTitle;
            this.FilePath = filePath;
            this.TagContainer = container;
        }

        public SongWithNumberAndGuid SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                _selectedSong = value;
                AddSelectedSongToContainer();
            }
        }

        public ObservableCollection<SongWithNumberAndGuid> SongsFromWebsite
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
        /// Updates the container with the new details selected by the user
        /// </summary>
        private void AddSelectedSongToContainer()
        {
            //VERY IMPORTANT WE DO NOT WRITE BLANK GUIDS
            if (SelectedSong.Guid != Guid.Empty)
                this.TagContainer.Add(new MediaIdGuid {Guid = this.SelectedSong.Guid, MediaId = MediaIds.ZuneMediaID});
        }


        private SongWithNumberAndGuid MatchThisSongToAvailableSongs()
        {
            //this matches album songs to zune website songs in the details view
            IEnumerable<SongWithNumberAndGuid> matchedSongs =
                SongsFromWebsite.Where(song => song.Title.ToLower() == this.SongTitle.ToLower());

            return matchedSongs.Count() > 0 ? matchedSongs.First() : new SongWithNumberAndGuid();
        }
    }
}