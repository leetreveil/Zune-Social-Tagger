using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Screens;

namespace ZuneSocialTagger.GUIV2.Models
{
    public interface IZuneWizardModel
    {
        BindableCollection<DetailRow> Rows { get; set; }
        Screen CurrentPage { get; set; }
    }
}