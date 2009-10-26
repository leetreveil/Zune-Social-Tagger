using System.Windows.Input;
using ZuneSocialTagger.GUI.Commands;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.Core.ID3Tagger;

namespace ZuneSocialTagger.GUI.ViewModels
{
    class DetailsViewModel : ViewModelBase
    {
    	private DelegateCommand<string> _getZuneAlbumWebpage;
        private readonly AsyncObservableCollection<SongWithNumberString> _songs = new AsyncObservableCollection<SongWithNumberString>();
        private readonly MetaData _metaData = new MetaData();

        public AsyncObservableCollection<SongWithNumberString> Songs
        {
            get{ return _songs;}
        }

        public string AlbumArtist
        {
            get { return _metaData.AlbumArtist; }
        }

        public ICommand DownloadZuneWebpageCommand
        {
            get
            {
                if (_getZuneAlbumWebpage == null)
                {
                    _getZuneAlbumWebpage = new DelegateCommand<string>(DownloadUrl);
                }
                return _getZuneAlbumWebpage;
            }
        }
		
		private void DownloadUrl(string url)
        {
            PageDownloader.DownloadAsync(url, ScrapeWebpage);
        }

        private void ScrapeWebpage(string pageData)
        {
            var scraper = new ZuneAlbumWebpageScraper(pageData);

            int number = 0;
            foreach (var songGuid in scraper.GetSongTitleAndIDs())
            {
                number++;
                _songs.Add(new SongWithNumberString() {Number = number.ToString(), Title = songGuid.Title});
            }

            _metaData.AlbumArtist = scraper.ScrapeAlbumArtist();
            _metaData.AlbumTitle = scraper.ScrapeAlbumTitle();
            _metaData.Year = scraper.ScrapeAlbumReleaseYear().ToString();
        }
    }

    internal class SongWithNumberString
    {
        public string Title { get; set; }
        public string Number { get; set; }
    }
}
