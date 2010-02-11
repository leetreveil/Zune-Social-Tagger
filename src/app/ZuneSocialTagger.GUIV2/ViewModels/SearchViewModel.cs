using System;
using Microsoft.Practices.Unity;
using ZuneSocialTagger.GUIV2.Models;
using Caliburn.PresentationFramework.Screens;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;

        public SearchViewModel(IUnityContainer container, IZuneWizardModel model)
        {
            _container = container;
            _model = model;
            this.SearchBar.StartedSearching += SearchBarViewModel_StartedSearching;
        }

        void SearchBarViewModel_StartedSearching(object sender, EventArgs e)
        {
            //we only want to move to the next page if we are on the current one
            //this prevents moving to another page when this page is not 'visible'

            _model.CurrentPage = _container.Resolve<SearchResultsViewModel>();
        }

        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile
        {
            get { return _model.AlbumDetailsFromFile; }
        }

        public SearchBarViewModel SearchBar
        {
            get { return _model.SearchBarViewModel; }
        }

        public void Search()
        {
            this.SearchBar.Search();
        }
    }
}