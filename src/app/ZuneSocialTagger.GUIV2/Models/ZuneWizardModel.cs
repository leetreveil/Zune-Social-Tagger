using Caliburn.PresentationFramework.Screens;
using Caliburn.Core;
using Caliburn.PresentationFramework;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel : PropertyChangedBase, IZuneWizardModel
    {
        private Screen _currentPage;

        public ZuneWizardModel()
        {
            this.Rows = new BindableCollection<DetailRow>();
        }

        public BindableCollection<DetailRow> Rows { get; set; }

        public Screen CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                NotifyOfPropertyChange(() => this.CurrentPage);
            }
        }
    }
}