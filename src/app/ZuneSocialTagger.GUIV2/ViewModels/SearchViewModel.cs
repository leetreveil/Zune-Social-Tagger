using System.Collections.Generic;
using System.Windows.Input;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Commands;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : ZuneWizardPageViewModelBase
    {
        private RelayCommand<string> _searchCommand;
        private WebsiteAlbumMetaDataViewModel _websiteAlbumMetaDataViewModel;

        public SearchViewModel()
        {
            WebsiteAlbumMetaDataViewModel = new WebsiteAlbumMetaDataViewModel();
        }

        public WebsiteAlbumMetaDataViewModel WebsiteAlbumMetaDataViewModel
        {
            get { return _websiteAlbumMetaDataViewModel; }
            set
            {
                _websiteAlbumMetaDataViewModel = value;
                OnPropertyChanged("WebsiteAlbumMetaDataViewModel");
            }
        }


        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand<string>(SearchFor);
                }

                return _searchCommand;
            }
        }

        private static void SearchFor(string artist)
        {
            IEnumerable<AlbumSearchResult> searchResult = AlbumSearch.SearchFor(artist);

        }

        public override string NextButtonText
        {
            get { return "Search"; }
        }

        internal override bool IsValid()
        {
            return true;
        }
    }
}