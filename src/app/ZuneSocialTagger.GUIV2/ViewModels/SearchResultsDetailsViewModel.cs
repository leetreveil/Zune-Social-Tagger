using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsDetailsViewModel : NotifyPropertyChangedImpl
    {
        private string _selectedAlbumTitle;

        public AsyncObservableCollection<SongWithNumberAndGuid> SelectedAlbumSongs { get; set; }

        public SearchResultsDetailsViewModel()
        {
            this.SelectedAlbumSongs = new AsyncObservableCollection<SongWithNumberAndGuid>();
        }

        public string SelectedAlbumTitle
        {
            get { return _selectedAlbumTitle; }
            set
            {
                if (value != _selectedAlbumTitle)
                {
                    _selectedAlbumTitle = value;
                    base.InvokePropertyChanged("SelectedAlbumTitle");
                }
            }
        }
    }
}