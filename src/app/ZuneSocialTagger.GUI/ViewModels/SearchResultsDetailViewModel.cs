using System.Collections.ObjectModel;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SearchResultsDetailViewModel : ViewModelBaseExtended
    {
        private string _selectedAlbumTitle;

        public ObservableCollection<WebTrack> SelectedAlbumSongs { get; set; }

        public SearchResultsDetailViewModel()
        {
            this.SelectedAlbumSongs = new ObservableCollection<WebTrack>();
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