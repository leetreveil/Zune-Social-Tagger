using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsDetailViewModel : NotifyPropertyChangedImpl
    {
        private string _selectedAlbumTitle;

        public ObservableCollection<Track> SelectedAlbumSongs { get; set; }

        public SearchResultsDetailViewModel()
        {
            this.SelectedAlbumSongs = new ObservableCollection<Track>();
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