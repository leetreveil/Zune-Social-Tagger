using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ZuneSocialTagger.Core.ZuneWebsiteScraper;
using ZuneSocialTagger.GUIV2.Commands;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : ZuneWizardPageViewModelBase
    {
        public SearchViewModel()
        {
            this.SearchBarViewModel = new SearchBarViewModel();
            this.SearchBarViewModel.FinishedSearching += SearchBarViewModel_FinishedSearching;
            this.WebsiteAlbumMetaDataViewModel = new WebsiteAlbumMetaDataViewModel();
            base.IsMovingNext += SearchViewModel_IsMovingNext;
        }

        void SearchBarViewModel_FinishedSearching(object sender, EventArgs e)
        {
            base.OnMoveNextOverride();
        }

        private void SearchViewModel_IsMovingNext(object sender, EventArgs e)
        {
           _searchBarViewModel.Search();
        }

        private WebsiteAlbumMetaDataViewModel _websiteAlbumMetaDataViewModel;
        public WebsiteAlbumMetaDataViewModel WebsiteAlbumMetaDataViewModel
        {
            get { return _websiteAlbumMetaDataViewModel; }
            set
            {
                _websiteAlbumMetaDataViewModel = value;
                OnPropertyChanged("WebsiteAlbumMetaDataViewModel");
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
            get { return "Search"; }
        }

        public string SearchText { get; set; }

        internal override bool IsValid()
        {
            return _searchBarViewModel.CanSearch;
        }

        internal override bool CanMoveNext()
        {
            return _searchBarViewModel.IsSearching == false;
        }

        private int _moveNextAttempts;

        private void MoveNext()
        {
            _moveNextAttempts++;

            if (_moveNextAttempts <= 1)
                base.OnMoveNextOverride();
        }

        //this is invoked when the view is loaded, different to when the view is constructed
        public void ViewShown()
        {
        }
    }
}