using System.Collections.ObjectModel;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using ZuneSocialTagger.GUI.ViewModels;

namespace ZuneSocialTagger.GUI.Models
{
    public class ZuneObservableCollection<T> : ObservableCollection<T>
    {
        public event Action NeedsUpdating = delegate { };

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NeedsUpdating.Invoke();

            if (e.Action ==  NotifyCollectionChangedAction.Add)
            {
                foreach (object newItem in e.NewItems)
                {
                    if (newItem.GetType() == typeof(AlbumDetailsViewModel))
                    {
                        var casted = (AlbumDetailsViewModel) newItem;

                        casted.PropertyChanged += (sender, args) => {
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