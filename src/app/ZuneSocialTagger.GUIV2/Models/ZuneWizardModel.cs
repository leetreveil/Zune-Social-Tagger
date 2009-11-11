using System.Collections.ObjectModel;
using System.ComponentModel;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel : INotifyPropertyChanged
    {
        public ZuneWizardModel()
        {
            this.SearchBarViewModel = new SearchBarViewModel();
            this.AlbumDetailsFromWebsite = new WebsiteAlbumMetaDataViewModel();
            this.AlbumDetailsFromFile = new WebsiteAlbumMetaDataViewModel();
            this.SongsFromFile = new ObservableCollection<SongWithNumberAndGuid>();
            this.Rows = new ObservableCollection<DetailRow>();
        }

        private SearchBarViewModel _searchBarViewModel;
        public SearchBarViewModel SearchBarViewModel
        {
            get { return _searchBarViewModel; }
            set
            {
                _searchBarViewModel = value;
                OnPropertyChanged("SearchBarViewModel");
            }
        }

        private WebsiteAlbumMetaDataViewModel _albumDetailsFromWebsite;
        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromWebsite
        {
            get { return _albumDetailsFromWebsite; }
            set
            {
                _albumDetailsFromWebsite = value;
                OnPropertyChanged("AlbumDetailsFromWebsite");
            }
        }

        private WebsiteAlbumMetaDataViewModel _albumDetailsFromFile;
        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile
        {
            get { return _albumDetailsFromFile; }
            set
            {
                _albumDetailsFromFile = value;
                OnPropertyChanged("AlbumDetailsFromFile");
            }
        }

        public ObservableCollection<SongWithNumberAndGuid> SongsFromFile { get; set; }
        public ObservableCollection<DetailRow> Rows { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}