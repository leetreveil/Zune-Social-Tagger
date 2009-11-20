using System.Collections.ObjectModel;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel
    {
        public ZuneWizardModel()
        {
            this.SearchBarViewModel = new SearchBarViewModel();
            this.AlbumDetailsFromWebsite = new WebsiteAlbumMetaDataViewModel();
            this.AlbumDetailsFromFile = new WebsiteAlbumMetaDataViewModel();
            this.SongsFromFile = new ObservableCollection<SongWithNumberAndGuid>();
            this.Rows = new ObservableCollection<DetailRow>();
        }

        public SearchBarViewModel SearchBarViewModel { get; set; }
        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromWebsite { get; set; }
        public WebsiteAlbumMetaDataViewModel AlbumDetailsFromFile { get; set; }
        public ObservableCollection<SongWithNumberAndGuid> SongsFromFile { get; set; }
        public ObservableCollection<DetailRow> Rows { get; set; }
    }
}