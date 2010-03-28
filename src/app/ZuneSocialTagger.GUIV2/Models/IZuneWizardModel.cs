using System.Collections.ObjectModel;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneWizardModel
    {
        ObservableCollection<Album> FoundAlbums { get; set; }
        SelectedAlbum SelectedAlbum { get; set; }
        string SearchText { get; set; }
        ObservableCollection<AlbumDetailsViewModel> AlbumsFromDatabase { get; set; }
        int SelectedItemInListView { get; set; }
    }
}