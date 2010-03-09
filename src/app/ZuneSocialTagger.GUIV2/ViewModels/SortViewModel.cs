using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Caliburn.Core;
using Caliburn.PresentationFramework;
using ZuneSocialTagger.GUIV2.Models;


namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SortViewModel : PropertyChangedBase
    {
        private BindableCollection<AlbumDetailsViewModel> _albums;
        private SortOrder _sortOrder;

        public SortViewModel(BindableCollection<AlbumDetailsViewModel> albums)
        {
            _albums = albums;
            this.SortOrder = SortOrder.NotSorted;
        }

        public event Action<BindableCollection<AlbumDetailsViewModel>> SortCompleted = delegate { };

        public SortOrder SortOrder
        {
            get { return _sortOrder; }
            set
            {
                _sortOrder = value;
                NotifyOfPropertyChange(() => this.SortOrder);
            }
        }

        public void Sort(SortOrder sortOrder)
        {
            this.SortOrder = sortOrder;
            PerformSort();
        }

        public void Sort()
        {
            SetSortState();
            PerformSort();
        }

        private void PerformSort()
        {
            ThreadPool.QueueUserWorkItem(_ =>
                 {
                     BindableCollection<AlbumDetailsViewModel> sortedBindableAlbums;

                     switch (SortOrder)
                     {
                         case SortOrder.DateAdded:
                             sortedBindableAlbums =
                                 _albums.OrderByDescending(
                                     x => x.ZuneAlbumMetaData.DateAdded).
                                     ToBindableCollection();
                             break;
                             ;
                         case SortOrder.Album:
                             sortedBindableAlbums =
                                 _albums.OrderBy(x => x.ZuneAlbumMetaData.AlbumTitle).
                                     ToBindableCollection();
                             break;
                         case SortOrder.Artist:
                             sortedBindableAlbums =
                                 _albums.OrderBy(x => x.ZuneAlbumMetaData.AlbumArtist).
                                     ToBindableCollection();
                             break;
                         case SortOrder.LinkStatus:
                             sortedBindableAlbums =
                                 _albums.OrderByDescending(x => x.LinkStatus).
                                     ToBindableCollection();
                             break;
                         default:
                             throw new ArgumentOutOfRangeException();
                     }

                     this.SortCompleted.Invoke(sortedBindableAlbums);
                 });
        }

        private void SetSortState()
        {
            //skip not sorted as we do not want to display that while looping through sort orders
            List<SortOrder> sortOrders =
                Enum.GetValues(typeof (SortOrder)).Cast<SortOrder>().Where(x => x != SortOrder.NotSorted).ToList();

            int index = sortOrders.IndexOf(this.SortOrder);

            this.SortOrder = index == sortOrders.Count - 1 ? sortOrders[0] : sortOrders[index + 1];
        }
    }

    public enum SortOrder
    {
        DateAdded,
        Album,
        Artist,
        LinkStatus,
        NotSorted
    }
}