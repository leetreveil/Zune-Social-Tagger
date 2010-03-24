using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel : ViewModelBase, IZuneWizardModel
    {
        public ZuneWizardModel()
        {
           // this.Rows = new BindableCollection<DetailRow>();
            this.FoundAlbums = new ObservableCollection<Album>();
        }

        public ObservableCollection<DetailRow> Rows { get; set; }
        public ObservableCollection<Album> FoundAlbums { get; set; }

        /// <summary>
        /// The details of the selected album from file
        /// </summary>
        public ExpandedAlbumDetailsViewModel FileAlbumDetails { get; set; }

        /// <summary>
        /// The Details of the selected album from the zune website
        /// </summary>
        public ExpandedAlbumDetailsViewModel WebAlbumDetails { get; set; }

        public string SearchText { get; set; }
    }
}