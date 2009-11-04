using System.Collections.ObjectModel;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    class DetailsViewModel : ZuneWizardPageViewModelBase
    {
        public DetailsViewModel()
        {
            //provide it with blank data as we need to set the defaults
            this.AlbumDetailsFromWebsite = ZuneWizardModel.GetInstance().AlbumDetailsFromWebsite;
            this.AlbumDetailsFromFile = ZuneWizardModel.GetInstance().AlbumDetailsFromFile;

            this.Rows = ZuneWizardModel.GetInstance().Rows;
        }

        public ObservableCollection<DetailRow> Rows { get; set; }

        private WebsiteAlbumMetaDataViewModel _albumDetailsFromWebsite;
        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromWebsite
        {
            get { return _albumDetailsFromWebsite; }
            set
            {
                _albumDetailsFromWebsite = value;
                OnPropertyChanged("AlbumDetailsFromWebsite");
            }
        }

        private WebsiteAlbumMetaDataViewModel _albumDetailsFromFile;
        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile
        {
            get { return _albumDetailsFromFile; }
            set
            {
                _albumDetailsFromFile = value;
                OnPropertyChanged("AlbumDetailsFromWebsite");
            }
        }

        private SongWithNumberAndGuid _selectedSong;
        public SongWithNumberAndGuid SelectedSong
        {
            get
            {
                return _selectedSong;
            }
            set
            {
                _selectedSong = value;
                OnPropertyChanged("SelectedSong");
            }

        }

        internal override bool IsValid()
        {
            return true;
        }

        internal override bool CanMoveNext()
        {
            return true;
        }

        public override string NextButtonText
        {
            get
            {
                return "Save";
            }
        }
    }
}
