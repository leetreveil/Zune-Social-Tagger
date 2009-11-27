using System.Collections.ObjectModel;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsDetailsViewModel : NotifyPropertyChangedImpl
    {
        private string _selectedAlbumTitle;

        public ObservableCollection<Track> SelectedAlbumSongs { get; set; }

        public SearchResultsDetailsViewModel()
        {
            this.SelectedAlbumSongs = new ObservableCollection<Track>();
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