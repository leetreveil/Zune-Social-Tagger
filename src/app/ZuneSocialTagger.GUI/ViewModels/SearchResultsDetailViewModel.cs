using System.Collections.ObjectModel;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SearchResultsDetailViewModel : ViewModelBase
    {
        private string _selectedAlbumTitle;

        public ObservableCollection<DetailRowSong> SelectedAlbumSongs { get; set; }

        public SearchResultsDetailViewModel()
        {
            this.SelectedAlbumSongs = new ObservableCollection<DetailRowSong>();
        }

        public string SelectedAlbumTitle
        {
            get { return _selectedAlbumTitle; }
            set
            {
                    _selectedAlbumTitle = value;
                    RaisePropertyChanged(() => SelectedAlbumTitle);
            }
        }
    }
}