using System;
using System.Threading;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{
    public class ZuneWizardModel : ViewModelBaseExtended
    {
        private ZuneObservableCollection<AlbumDetailsViewModel> _albumsFromDatabase;

        public SelectedAlbum SelectedAlbum { get; set; }
        public string SearchText { get; set; }

        public ZuneObservableCollection<AlbumDetailsViewModel> AlbumsFromDatabase
        {
            get
            {
                if (_albumsFromDatabase == null)
                {
                    _albumsFromDatabase = new ZuneObservableCollection<AlbumDetailsViewModel>();
                }

                return _albumsFromDatabase;
            }
        }

        public void PerformSort(SortOrder sortOrder)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                switch (sortOrder)
                {
                    case SortOrder.DateAdded:
                        _albumsFromDatabase.SortDesc(x => x.ZuneAlbumMetaData.DateAdded);
                        break;
                    case SortOrder.Album:
                        _albumsFromDatabase.Sort(x => x.ZuneAlbumMetaData.AlbumTitle);
                        break;
                    case SortOrder.Artist:
                        _albumsFromDatabase.Sort(x => x.ZuneAlbumMetaData.AlbumArtist);
                        break;
                    case SortOrder.LinkStatus:
                        _albumsFromDatabase.Sort(x => x.LinkStatus);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}