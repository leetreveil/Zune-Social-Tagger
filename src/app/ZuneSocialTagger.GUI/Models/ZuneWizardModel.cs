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
            set
            {
                _albumsFromDatabase = value;
                RaisePropertyChanged(() => this.AlbumsFromDatabase);
            }
        }

        public void PerformSort(SortOrder sortOrder)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                switch (sortOrder)
                {
                    case SortOrder.DateAdded:
                        DoSort(x => x.ZuneAlbumMetaData.DateAdded);
                        break;
                    case SortOrder.Album:
                        DoSort(x => x.ZuneAlbumMetaData.AlbumTitle);
                        break;
                    case SortOrder.Artist:
                        DoSort(x => x.ZuneAlbumMetaData.AlbumArtist);
                        break;
                    case SortOrder.LinkStatus:
                        DoSort(x => x.LinkStatus);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void DoSort<T>(Func<AlbumDetailsViewModel, T> sortKey)
        {
            //we want to order by descending if we use DateTime because we want the newest at the top
            //all the other states we want ascending
            //this.AlbumsFromDatabase = typeof (T) == typeof (DateTime)
            //                  ? this.AlbumsFromDatabase.OrderByDescending(sortKey).ToObservableCollection()
            //                  : this.AlbumsFromDatabase.OrderBy(sortKey).ToObservableCollection();
        }
    }
}