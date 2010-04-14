using System.Collections.ObjectModel;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneWizardModel
    {
        SelectedAlbum SelectedAlbum { get; set; }
        string SearchText { get; set; }
        ObservableCollection<AlbumDetailsViewModel> AlbumsFromDatabase { get; set; }
    }
}