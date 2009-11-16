using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.GUIV2.Models
{
    /// <summary>
    /// Every row in the DetailsView has this information.
    /// </summary>s
    public class DetailRow
    {
        private SongWithNumberAndGuid _selectedSong;
        private ObservableCollection<SongWithNumberAndGuid> _songsFromWebsite;

        public string Index { get; set; }
        public string SongTitle { get; set; }
        public string FilePath { get; set; }
        public ZuneTagContainer TagContainer { get; set; }

        public DetailRow(ZuneTagContainer container,string filePath)
        {
            this.FilePath = filePath;
            this.TagContainer = container;
            Init();
        }

        private void Init()
        {
            MetaData metaData = this.TagContainer.ReadMetaData();

            this.SongTitle = metaData.SongTitle;
            this.Index = metaData.Index;      
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

        /// <summary>
        /// when this is first set we try to match the songs from the zune website to whats in the songs metadata
        /// </summary>
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


        /// <summary>
        /// Matches song titles
        /// </summary>
        /// <returns></returns>
        private SongWithNumberAndGuid MatchThisSongToAvailableSongs()
        {
            //this matches album songs to zune website songs in the details view
            IEnumerable<SongWithNumberAndGuid> matchedSongs =
                this.SongsFromWebsite.Where(song => song.Title.ToLower() == this.SongTitle.ToLower());

            return matchedSongs.Count() > 0 ? matchedSongs.First() : new SongWithNumberAndGuid();
        }
    }
}