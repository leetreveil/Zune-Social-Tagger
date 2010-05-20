using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    }  
}