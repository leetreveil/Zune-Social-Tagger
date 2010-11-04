using System.Collections.ObjectModel;
using ZuneSocialTagger.GUI.ViewsViewModels.Shared;

namespace ZuneSocialTagger.GUI.ViewsViewModels.Search
{
    public class SearchResultsDetailViewModel : ViewModelBase
    {
        public ObservableCollection<TrackWithTrackNum> SelectedAlbumSongs { get; set; }

        public SearchResultsDetailViewModel()
        {
            this.SelectedAlbumSongs = new ObservableCollection<TrackWithTrackNum>();
        }

        private string _selectedAlbumTitle;
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