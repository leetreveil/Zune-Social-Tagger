namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SuccessViewModel
    {
        private readonly ExpandedAlbumDetailsViewModel _albumDetailsFromWebsite;
        private readonly ExpandedAlbumDetailsViewModel _albumDetailsFromFile;

        public SuccessViewModel(ExpandedAlbumDetailsViewModel albumDetailsFromWebsite,
            ExpandedAlbumDetailsViewModel albumDetailsFromFile)
        {
            _albumDetailsFromWebsite = albumDetailsFromWebsite;
            _albumDetailsFromFile = albumDetailsFromFile;
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromWebsite
        {
            get { return _albumDetailsFromWebsite; }
        }

        public ExpandedAlbumDetailsViewModel AlbumDetailsFromFile
        {
            get { return _albumDetailsFromFile; }
        }
    }
}