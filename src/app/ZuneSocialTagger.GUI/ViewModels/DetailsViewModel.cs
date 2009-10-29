using System.Windows.Input;
using ZuneSocialTagger.GUI.Commands;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels
{
    class DetailsViewModel : ViewModelBase
    {
    	private DelegateCommand<string> _getZuneAlbumWebpage;
        private readonly AsyncObservableCollection<SongWithNumberString> _songs = new AsyncObservableCollection<SongWithNumberString>();

        public DetailsViewModel()
        {
            //provide it with blank data as we need to set the defaults
            WebsiteAlbumMetaDataViewModel = new WebsiteAlbumMetaDataViewModel(new ZuneNetAlbumMetaData());
        }

        public AsyncObservableCollection<SongWithNumberString> Songs
        {
            get{ return _songs;}
        }

        private WebsiteAlbumMetaDataViewModel _websiteAlbumMetaDataViewModel;
        public WebsiteAlbumMetaDataViewModel WebsiteAlbumMetaDataViewModel
        {
            get { return _websiteAlbumMetaDataViewModel; }
            set
            { 
                _websiteAlbumMetaDataViewModel = value;
                OnPropertyChanged("WebsiteAlbumMetaDataViewModel");
            }
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
            var scraper = new AlbumWebpageScraper(pageData);

            int number = 0;
            foreach (var songGuid in scraper.GetSongTitleAndIDs())
            {
                number++;
                _songs.Add(new SongWithNumberString {Number = number.ToString(), Title = songGuid.Title});
            }

            var metaData = new ZuneNetAlbumMetaData
                               {
                                   Artist = scraper.ScrapeAlbumArtist(),
                                   Title = scraper.ScrapeAlbumTitle(),
                                   Year = scraper.ScrapeAlbumReleaseYear().ToString(),
                                   ArtworkUrl = scraper.ScrapeAlbumArtworkUrl(),
                                   SongCount = Songs.Count + " songs"
                               };

            WebsiteAlbumMetaDataViewModel = new WebsiteAlbumMetaDataViewModel(metaData);
        }
    }
}
