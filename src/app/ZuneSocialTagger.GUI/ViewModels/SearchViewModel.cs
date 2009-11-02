using System.Windows.Input;
using ZuneSocialTagger.GUI.Commands;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly ZuneNetAlbumMetaData _metaData;

        private DelegateCommand<string> _searchCommand;

        public SearchViewModel(ZuneNetAlbumMetaData metaData)
        {
            _metaData = metaData;

            //provide it with blank data as we need to set the defaults
            WebsiteAlbumMetaDataViewModel = new WebsiteAlbumMetaDataViewModel(metaData);
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

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new DelegateCommand<string>(SearchFor);
                }

                return _searchCommand;
            }
        }

        private static void SearchFor(string artist)
        {
            
        }
    }
}