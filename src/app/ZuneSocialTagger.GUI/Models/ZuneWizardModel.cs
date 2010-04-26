using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{
    public class ZuneWizardModel : ViewModelBaseExtended
    {
        private ZuneObservableCollection<AlbumDetailsViewModel> _albumsFromDatabase;

        public SelectedAlbum SelectedAlbum { get; set; }
        public string SearchText { get; set; }

        public ZuneObservableCollection<AlbumDetailsViewModel> AlbumsFromDatabase
        {
            get
            {
                if (_albumsFromDatabase == null)
                {
                    _albumsFromDatabase = new ZuneObservableCollection<AlbumDetailsViewModel>();
                }

                return _albumsFromDatabase;
            }
        }
    }
}