using System;
using System.Windows.Input;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : ZuneWizardPageViewModelBase
    {
        public SearchViewModel()
        {
            this.SearchBarViewModel = ZuneWizardModel.GetInstance().SearchBarViewModel;
            this.AlbumDetailsFromFile = ZuneWizardModel.GetInstance().AlbumDetailsFromFile;
            this.SearchBarViewModel.StartedSearching += SearchBarViewModel_StartedSearching;
        }

        void SearchBarViewModel_StartedSearching(object sender, EventArgs e)
        {
           this.SearchBarViewModel.StartedSearching -= SearchBarViewModel_StartedSearching;
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

        private RelayCommand<string> _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand<string>(searchString =>
                    {

                    });
                }

                return _searchCommand;
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
           // this.SearchBarViewModel.StartedSearching += SearchBarViewModel_StartedSearching;
        }
    }
}