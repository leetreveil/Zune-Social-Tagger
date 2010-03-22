using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsDetailViewModel : NotifyPropertyChangedImpl
    {
        private string _selectedAlbumTitle;

        public AsyncObservableCollection<Track> SelectedAlbumSongs { get; set; }

        public SearchResultsDetailViewModel()
        {
            this.SelectedAlbumSongs = new AsyncObservableCollection<Track>();
        }

        public string SelectedAlbumTitle
        {
            get { return _selectedAlbumTitle; }
            set
            {
                    _selectedAlbumTitle = value;
                    NotifyOfPropertyChange(() => SelectedAlbumTitle);
            }
        }
    }
}