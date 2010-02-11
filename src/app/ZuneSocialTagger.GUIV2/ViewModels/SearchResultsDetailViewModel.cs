using Caliburn.PresentationFramework;
using ZuneSocialTagger.Core;
using Caliburn.PresentationFramework.Screens;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchResultsDetailViewModel : Screen
    {
        private string _selectedAlbumTitle;

        public BindableCollection<Track> SelectedAlbumSongs { get; set; }

        public SearchResultsDetailViewModel()
        {
            this.SelectedAlbumSongs = new BindableCollection<Track>();
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