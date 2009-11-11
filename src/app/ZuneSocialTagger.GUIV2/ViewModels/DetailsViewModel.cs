using System.Collections.ObjectModel;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    class DetailsViewModel : ZuneWizardPageViewModelBase
    {
        private readonly ZuneWizardModel _model;

        public DetailsViewModel(ZuneWizardModel model)
        {
            _model = model;
        }

        public ObservableCollection<DetailRow> Rows 
        {
            get { return _model.Rows; }
        } 

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromWebsite
        {
            get { return _model.AlbumDetailsFromWebsite; }
        }

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile
        {
            get { return _model.AlbumDetailsFromFile; }
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
