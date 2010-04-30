using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.Core.ZuneWebsite;

namespace ZuneSocialTagger.GUI.ViewModels
{
    public class SearchResultsDetailViewModel : ViewModelBaseExtended
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
                    RaisePropertyChanged(() => SelectedAlbumTitle);
            }
        }
    }
}