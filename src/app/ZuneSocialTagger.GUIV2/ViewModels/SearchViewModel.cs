using Microsoft.Practices.ServiceLocation;
using ZuneSocialTagger.GUIV2.Models;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework;
using Album = ZuneSocialTagger.Core.Album;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchViewModel : Screen
    {
        private readonly IServiceLocator _locator;
        private readonly IZuneWizardModel _model;

        public SearchViewModel(IServiceLocator locator, IZuneWizardModel model)
        {
            _locator = locator;
            _model = model;

            this.SearchHeader = model.SearchHeader;
            this.SearchHeader.SearchBar.StartedSearching += SearchBar_StartedSearching;
        }

        private void SearchBar_StartedSearching(BindableCollection<Album> albums)
        {
            _model.FoundAlbums = albums;
            _model.CurrentPage = _locator.GetInstance<SearchResultsViewModel>();
        }

        public SearchHeaderViewModel SearchHeader { get; set; }

        public void Search()
        {
            this.SearchHeader.SearchBar.Search();
        }
        
        public void MoveBack()
        {
            _model.CurrentPage = _locator.GetInstance<WebAlbumListViewModel>();
        }
    }
}