using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class DetailRow
    {
        public DetailRow(SongWithNumberAndGuid fileSong, string filePath, ZuneTagContainer container)
        {
            this.FilePath = filePath;
            this.TagContainer = container;
            this.FileSong = fileSong;
        }

        public MediaIdGuid AlbumArtistGuid { get; set; }
        public MediaIdGuid AlbumMediaGuid { get; set; }
        public string FilePath { get; set; }
        public ZuneTagContainer TagContainer { get; set; }
        public SongWithNumberAndGuid FileSong { get; set; }
        public SongWithNumberAndGuid SelectedSong { get; set; }
    

        private ObservableCollection<SongWithNumberAndGuid> _songsFromWebsite;
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
        public void UpdateContainer()
        {
            //TODO: check if the tags are already correct because we shouldnt be writing anything to disk if the tags are already the same

            //VERY IMPORTANT WE DO NOT WRITE BLANK GUIDS
            if (SelectedSong.Guid != Guid.Empty)
            {
                this.TagContainer.Add(AlbumArtistGuid);
                this.TagContainer.Add(AlbumMediaGuid);
                this.TagContainer.Add(new MediaIdGuid { Guid = this.SelectedSong.Guid, MediaId = MediaIds.ZuneMediaID });
            }
        }


        private SongWithNumberAndGuid MatchThisSongToAvailableSongs()
        {
            //this matches album songs to zune website songs in the details view
            IEnumerable<SongWithNumberAndGuid> matchedSongs =
                SongsFromWebsite.Where(song => song.Title.ToLower() == FileSong.Title.ToLower());

            return matchedSongs.Count() > 0 ? matchedSongs.First() : new SongWithNumberAndGuid();
        }
    }
}