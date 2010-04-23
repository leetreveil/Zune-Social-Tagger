using System;
using System.Threading;
using System.Windows.Threading;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{
    public class ZuneWizardModel : ViewModelBaseExtended
    {
        private readonly Dispatcher _dispatcher;
        private ZuneObservableCollection<AlbumDetailsViewModel> _albumsFromDatabase;

        public ZuneWizardModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

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
            if (typeof(T) == typeof(DateTime))
                _dispatcher.Invoke(new Action(() => this.AlbumsFromDatabase.SortDesc(sortKey)));
            else
                _dispatcher.Invoke(new Action(() => this.AlbumsFromDatabase.Sort(sortKey)));
        }
    }
}