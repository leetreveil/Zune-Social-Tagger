using System.ComponentModel;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SuccessViewModel : INotifyPropertyChanged
    {
        public SuccessViewModel()
        {
            this.AlbumDetailsFromWebsite = ZuneWizardModel.GetInstance().AlbumDetailsFromWebsite;
            this.AlbumDetailsFromFile = ZuneWizardModel.GetInstance().AlbumDetailsFromFile;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}