using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ZuneSocialTagger.Core;
using ZuneSocialTagger.GUIV2.ViewModels;
using System.Collections.Generic;

namespace ZuneSocialTagger.GUIV2.Models
{
    public class ZuneWizardModel : ViewModelBase, IZuneWizardModel
    {
        private ObservableCollection<AlbumDetailsViewModel> _albumsFromDatabase;

        public ZuneWizardModel()
        {
            this.FoundAlbums = new ObservableCollection<Album>();
            this.AlbumsFromDatabase = new ObservableCollection<AlbumDetailsViewModel>();
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

        public ObservableCollection<AlbumDetailsViewModel> AlbumsFromDatabase
        {
            get { return _albumsFromDatabase; }
            set
            {
                _albumsFromDatabase = value;
                RaisePropertyChanged("AlbumsFromDatabase");
            }
        }
    }
}