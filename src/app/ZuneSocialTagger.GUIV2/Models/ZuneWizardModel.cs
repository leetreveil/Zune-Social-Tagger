using System.Collections.ObjectModel;
using Caliburn.PresentationFramework;
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
            this.SearchBarViewModel = new SearchBarViewModel();
            this.AlbumDetailsFromWebsite = new WebsiteAlbumMetaDataViewModel();
            this.AlbumDetailsFromFile = new WebsiteAlbumMetaDataViewModel();
            this.Rows = new ObservableCollection<DetailRow>();
            this.DatabaseAlbums = new BindableCollection<Album>();
        }

        public SearchBarViewModel SearchBarViewModel { get; set; }
        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromWebsite { get; set; }
        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile { get; set; }
        public ObservableCollection<DetailRow> Rows { get; set; }


        /// <summary>
        /// These are the albums that are read from the zune database
        /// </summary>
        public BindableCollection<Album> DatabaseAlbums { get; set; }

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