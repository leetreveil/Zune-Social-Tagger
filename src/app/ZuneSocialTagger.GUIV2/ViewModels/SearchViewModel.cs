using Microsoft.Practices.Unity;
using ZuneSocialTagger.GUIV2.Models;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework;
using Album = ZuneSocialTagger.Core.Album;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : Screen
    {
        private readonly IUnityContainer _container;
        private readonly IZuneWizardModel _model;

        public SearchViewModel(IUnityContainer container, IZuneWizardModel model, SearchHeaderViewModel searchHeaderViewModel)
        {
            _container = container;
            _model = model;

            this.SearchHeader = searchHeaderViewModel;
            this.SearchHeader.SearchBar.StartedSearching += SearchBar_StartedSearching;
        }

        void SearchBar_StartedSearching(BindableCollection<Album> albums)
        {
            var searchResultsViewModel = _container.Resolve<SearchResultsViewModel>();
            searchResultsViewModel.Albums = albums;
            searchResultsViewModel.SearchHeader = this.SearchHeader;

            _model.CurrentPage = searchResultsViewModel;
        }

        public SearchHeaderViewModel SearchHeader { get; set; }

        public void Search()
        {
            this.SearchHeader.SearchBar.Search();
        }
        
        public void MoveBack()
        {
            _model.CurrentPage = _container.Resolve<WebAlbumListViewModel>();
        }
    }
}