using System.Collections.ObjectModel;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{
    public interface IZuneWizardModel
    {
        SelectedAlbum SelectedAlbum { get; set; }
        string SearchText { get; set; }
        ObservableCollection<AlbumDetailsViewModel> AlbumsFromDatabase { get; set; }
    }
}