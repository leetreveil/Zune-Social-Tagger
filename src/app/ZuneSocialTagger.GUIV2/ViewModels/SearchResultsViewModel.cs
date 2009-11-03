using System;
using System.Collections.Generic;
using System.Linq;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Models;
using System.Threading;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsViewModel : ZuneWizardPageViewModelBase
    {
        public AsyncObservableCollection<AlbumArtistAndTitleWithUrl> Albums { get; set; }
        public AsyncObservableCollection<SongWithNumberString> SelectedAlbumSongs { get; set; }

        private int _albumCounter;

        private string _albumCount;

        public string AlbumCount
        {
            get { return _albumCount; }
            set
            {
                _albumCount = value;
                base.OnPropertyChanged("AlbumCount");
            }
        }

        private string _selectedAlbumTitle;

        public string SelectedAlbumTitle
        {
            get { return _selectedAlbumTitle; }
            set
            {
                _selectedAlbumTitle = value;
                base.OnPropertyChanged("SelectedAlbumTitle");
            }
        }

        internal override bool IsValid()
        {
            return true;
        }

        internal override bool CanMoveNext()
        {
            return true;
        }

        public SearchResultsViewModel()
        {
            Albums = new AsyncObservableCollection<AlbumArtistAndTitleWithUrl>();
            SelectedAlbumSongs = new AsyncObservableCollection<SongWithNumberString>();
            ZuneWizardModel.GetInstance().NewAlbumsAvail += SearchResultsViewModel_NewAlbumsAvail;
        }

        private void SearchResultsViewModel_NewAlbumsAvail(IEnumerable<AlbumArtistAndTitleWithUrl> albums)
        {
            _albumCounter += albums.Count();

            foreach (var album in albums)
                this.Albums.Add(album);

            this.AlbumCount = string.Format("Albums ({0})", _albumCounter);
        }

        public void LoadAlbum(AlbumArtistAndTitleWithUrl albumArtistAndTitleWithUrl)
        {
            PageDownloader.DownloadAsync(albumArtistAndTitleWithUrl.Url, ScrapedPage);
        }

        private void ScrapedPage(string pageData)
        {
            var scraper = new AlbumWebpageScraper(pageData);

            this.SelectedAlbumTitle = scraper.ScrapeAlbumTitle();

            var enumerable =
                scraper.GetSongTitleAndIDs().Select(x => new{x.Title});

            SelectedAlbumSongs.Clear();

            //TODO: fix this hack that allows it to clear properly
            //without this being here there are extra items at the end where it hasnt cleared fully before adding new items
            Thread.Sleep(50);

            int counter = 0;
            foreach (var song in enumerable)
            {
                counter++;
                SelectedAlbumSongs.AddItemAsync(new SongWithNumberString(){Number = counter.ToString(),Title = song.Title});
                Console.WriteLine("added {0}",song.Title);
            }

        }

        public void ViewShown()
        {
           // Albums.Clear();
        }
    }
}