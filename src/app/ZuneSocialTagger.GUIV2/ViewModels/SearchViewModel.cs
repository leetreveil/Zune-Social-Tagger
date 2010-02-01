using System;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : ZuneWizardPageViewModelBase
    {
        private readonly ZuneWizardModel _model;

        public SearchViewModel(ZuneWizardModel model)
        {
            _model = model;
            this.SearchBarViewModel.StartedSearching += SearchBarViewModel_StartedSearching;
            this.MoveNextClicked += SearchViewModel_MoveNextClicked;
        }

        void SearchViewModel_MoveNextClicked(object sender, EventArgs e)
        {
            _model.SearchBarViewModel.Search();
        }

        void SearchBarViewModel_StartedSearching(object sender, EventArgs e)
        {
            //we only want to move to the next page if we are on the current one
            //this prevents moving to another page when this page is not 'visible'
            if (base.IsCurrentPage)
                base.OnMoveNextOverride();
        }

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile
        {
            get { return _model.AlbumDetailsFromFile; }
        }

        public SearchBarViewModel SearchBarViewModel
        {
            get { return _model.SearchBarViewModel; }
        }

        internal override string NextButtonText
        {
            get { return "Search"; }
        }

        internal override bool IsNextEnabled()
        {
            return _model.SearchBarViewModel.CanSearch;
        }
    }
}