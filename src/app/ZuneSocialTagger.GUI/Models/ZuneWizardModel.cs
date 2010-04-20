using System.Collections.ObjectModel;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{
    public class ZuneWizardModel : NotifyPropertyChangedImpl, IZuneWizardModel
    {
        private ObservableCollection<AlbumDetailsViewModel> _albumsFromDatabase;

        public ZuneWizardModel()
        {
            this.AlbumsFromDatabase = new ObservableCollection<AlbumDetailsViewModel>();
        }

        public SelectedAlbum SelectedAlbum { get; set; }
        public string SearchText { get; set; }
        public ObservableCollection<AlbumDetailsViewModel> AlbumsFromDatabase
        {
            get { return _albumsFromDatabase; }
            set
            {
                _albumsFromDatabase = value;
                NotifyOfPropertyChange(() => this.AlbumsFromDatabase);
            }
        }
    }
}