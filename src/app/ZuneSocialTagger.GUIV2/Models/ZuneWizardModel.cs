using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel : INotifyPropertyChanged
    {
        #region SingletonStuff

        private static ZuneWizardModel instance = new ZuneWizardModel();

        private ZuneWizardModel()
        {
            this.SearchBarViewModel = new SearchBarViewModel();
            this.SelectedAlbumSongs = new AsyncObservableCollection<SongWithNumberAndGuid>();
            this.AlbumDetailsFromWebsite = new WebsiteAlbumMetaDataViewModel();
            this.AlbumDetailsFromFile = new WebsiteAlbumMetaDataViewModel();
            this.SongsFromFile = new ObservableCollection<SongWithNumberAndGuid>();
            this.Rows = new ObservableCollection<DetailRow>();
        }

        public static ZuneWizardModel GetInstance()
        {
            return instance;
        }

        #endregion

        public event Action<ZuneNetAlbumMetaData> AlbumMetaDataChanged = delegate { };
        public event Action<IEnumerable<AlbumSearchResult>> NewAlbumsAvail = delegate { };

        public void InvokeNewAlbumsAvailable(IEnumerable<AlbumSearchResult> albums)
        {
            Action<IEnumerable<AlbumSearchResult>> action = NewAlbumsAvail;
            if (action != null) action(albums);
        }

        public void InvokeAlbumMetaDataChanged(ZuneNetAlbumMetaData obj)
        {
            Action<ZuneNetAlbumMetaData> changed = AlbumMetaDataChanged;
            if (changed != null) changed(obj);
        }

        private SearchBarViewModel _searchBarViewModel;
        public SearchBarViewModel SearchBarViewModel
        {
            get { return _searchBarViewModel; }
            set
            {
                _searchBarViewModel = value;
                OnPropertyChanged("SearchBarViewModel");
            }
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
                OnPropertyChanged("AlbumDetailsFromFile");
            }
        }

        public AsyncObservableCollection<SongWithNumberAndGuid> SelectedAlbumSongs { get; set; }

        public ObservableCollection<SongWithNumberAndGuid> SongsFromFile { get; set; }

        public ObservableCollection<DetailRow> Rows { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null) changed(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}