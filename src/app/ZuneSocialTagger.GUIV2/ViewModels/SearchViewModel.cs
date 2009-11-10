using System;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : ZuneWizardPageViewModelBase
    {
        public SearchViewModel()
        {
            this.SearchBarViewModel = ZuneWizardModel.GetInstance().SearchBarViewModel;
            this.AlbumDetailsFromFile = ZuneWizardModel.GetInstance().AlbumDetailsFromFile;
        }

        void SearchBarViewModel_FinishedSearching(object sender, EventArgs e)
        {
           this.SearchBarViewModel.StartedSearching -= SearchBarViewModel_FinishedSearching;
           base.OnMoveNextOverride();
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

        public override string NextButtonText
        {
            get { return "Next"; }
        }

        internal override bool IsValid()
        {
            return true;
        }

        internal override bool CanMoveNext()
        {
            return false;
        }

        //this is invoked when the view is loaded, different to when the view is constructed
        public void ViewShown()
        {
            this.SearchBarViewModel.StartedSearching += SearchBarViewModel_FinishedSearching;
        }
    }
}