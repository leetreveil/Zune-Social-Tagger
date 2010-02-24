using System.Collections.ObjectModel;
using Caliburn.PresentationFramework.Screens;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneWizardModel
    {
        SearchBarViewModel SearchBarViewModel { get; set; }
        WebsiteAlbumMetaDataViewModel AlbumDetailsFromWebsite { get; set; }
        WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile { get; set; }
        ObservableCollection<DetailRow> Rows { get; set; }
        Screen CurrentPage { get; set; }
    }
}