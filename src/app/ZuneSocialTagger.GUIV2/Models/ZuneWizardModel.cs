using System.Collections.ObjectModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.Core;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel : PropertyChangedBase, IZuneWizardModel
    {
        private Screen _currentPage;

        public ZuneWizardModel()
        {
            this.Rows = new ObservableCollection<DetailRow>();
        }

        public ObservableCollection<DetailRow> Rows { get; set; }

        public Screen CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                NotifyOfPropertyChange(() => CurrentPage);
            }
        }
    }
}