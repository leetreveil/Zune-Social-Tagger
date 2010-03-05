using System.Collections.ObjectModel;
using Caliburn.PresentationFramework.Screens;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneWizardModel
    {
        ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }
        ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
        ObservableCollection<DetailRow> Rows { get; set; }
        Screen CurrentPage { get; set; }
    }
}