using ZuneSocialTagger.GUIV2.Models;
using System.ComponentModel;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsDetailsViewModel : INotifyPropertyChanged
    {
        private string _selectedAlbumTitle;

        public event PropertyChangedEventHandler PropertyChanged;
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
                _selectedAlbumTitle = value;
                InvokePropertyChanged("SelectedAlbumTitle");
            }
        }

        private void InvokePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}