namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SearchHeaderViewModel
    {
        public SearchHeaderViewModel()
        {
            this.SearchBar = new SearchBarViewModel();
        }

        public ExpandedAlbumDetailsViewModel AlbumDetails { get; set; }
        public SearchBarViewModel SearchBar { get; private set; }
    }
}