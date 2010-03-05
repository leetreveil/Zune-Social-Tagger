using System;
using System.Collections.ObjectModel;
using Caliburn.PresentationFramework.Screens;
using ZuneSocialTagger.GUIV2.ViewModels;
using Caliburn.Core;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel : PropertyChangedBase, IZuneWizardModel
    {
        private Screen _currentPage;

        public ZuneWizardModel()
        {
            this.AlbumDetailsFromWebsite = new ExpandedAlbumDetailsViewModel();
            this.AlbumDetailsFromFile = new ExpandedAlbumDetailsViewModel();
            this.Rows = new ObservableCollection<DetailRow>();
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite { get; set; }
        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile { get; set; }
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