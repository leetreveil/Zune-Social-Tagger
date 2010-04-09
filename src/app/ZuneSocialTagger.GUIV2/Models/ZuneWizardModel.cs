using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
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

        public int SelectedItemInListView { get; set; }
    }
}