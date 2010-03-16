using System.Collections.ObjectModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.Core;
using Caliburn.PresentationFramework;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel : PropertyChangedBase, IZuneWizardModel
    {
        private Screen _currentPage;

        public ZuneWizardModel()
        {
            this.Rows = new BindableCollection<DetailRow>();
            this.SearchHeader = new SearchHeaderViewModel();
            this.FoundAlbums = new ObservableCollection<Album>();
        }

        public Screen CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                NotifyOfPropertyChange(() => this.CurrentPage);
            }
        }

        public BindableCollection<DetailRow> Rows { get; set; }
        public ObservableCollection<Album> FoundAlbums { get; set; }
        public SearchHeaderViewModel SearchHeader { get; set; }
    }
}