using System.Collections.ObjectModel;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Screens;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneWizardModel
    {
        BindableCollection<DetailRow> Rows { get; set; }
        Screen CurrentPage { get; set; }
       // string SearchText { get; set; }
        //ExpandedAlbumDetailsViewModel SelectedAlbumDetails { get; set; }
        SearchHeaderViewModel SearchHeader { get; set; }
        ObservableCollection<Album> FoundAlbums { get; set; }
    }
}