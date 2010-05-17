using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using GalaSoft.MvvmLight.Threading;
using ZuneSocialTagger.GUI.ViewsViewModels.WebAlbumList;

namespace ZuneSocialTagger.GUI.Models
{
    [Serializable]
    public class ZuneObservableCollection<T> : ObservableCollection<T>
    {
        [field: NonSerialized]
        public event Action NeedsUpdating = delegate { };

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object newItem in e.NewItems)
                {
                    if (newItem.GetType() == typeof(AlbumDetailsViewModel))
                    {
                        var casted = (AlbumDetailsViewModel)newItem;

                        casted.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == "LinkStatus")
                            {
                                NeedsUpdating.Invoke();
                            }
                        };
                    }
                }
            }

            base.OnCollectionChanged(e);
        }

        ///<summary>
        /// Sorts the items of the collection in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from an item.</param>
        public void Sort<TKey>(Func<T, TKey> keySelector)
        {
            InternalSort(Items.OrderBy(keySelector));
        }

        public void SortDesc<TKey>(Func<T, TKey> keySelector)
        {
            InternalSort(Items.OrderByDescending(keySelector));
        }

        /// <summary>
        /// Moves the items of the collection so that their orders are the same as those of the items provided.
        /// </summary>
        /// <param name="sortedItems">An <see cref="IEnumerable{T}"/> to provide item orders.</param>
        private void InternalSort(IEnumerable<T> sortedItems)
        {
            List<T> sortedToList = sortedItems.ToList();
             DispatcherHelper.CheckBeginInvokeOnUI(()=> {
                 this.Clear();
                 sortedToList.ForEach(this.Add);
             });
        }
    }  
}