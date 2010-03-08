using System.Collections.ObjectModel;
using Caliburn.PresentationFramework.Screens;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneWizardModel
    {
        ObservableCollection<DetailRow> Rows { get; set; }
        Screen CurrentPage { get; set; }
    }
}