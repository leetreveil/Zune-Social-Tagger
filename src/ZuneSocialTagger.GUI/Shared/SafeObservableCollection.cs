using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;
using System;
using ZuneSocialTagger.GUI.Shared;


namespace ZuneSocialTagger.GUI.Shared
{
    /// <summary>
    /// Provides a threadsafe ObservableCollection of T
    /// </summary>
    public class SafeObservableCollection<T> : ObservableCollection<T>
    {
        #region Data
        private Dispatcher _dispatcher;
        private ReaderWriterLockSlim _lock;
        #endregion

        #region Ctor
        public SafeObservableCollection()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _lock = new ReaderWriterLockSlim();
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Clear all items
        /// </summary>
        protected override void ClearItems()
        {
            InvokeIfRequired(_dispatcher, () =>
            {
                _lock.EnterWriteLock();
                try
                {
                    base.ClearItems();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        /// <summary>
        /// Inserts an item
        /// </summary>
        protected override void InsertItem(int index, T item)
        {
            InvokeIfRequired(_dispatcher, () =>
            {
                if (index > this.Count)
                    return;

                _lock.EnterWriteLock();
                try
                {
                    base.InsertItem(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);

        }

        /// <summary>
        /// Moves an item
        /// </summary>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            InvokeIfRequired(_dispatcher, () =>
            {
                _lock.EnterReadLock();
                Int32 itemCount = this.Count;
                _lock.ExitReadLock();

                if (oldIndex >= itemCount |
                    newIndex >= itemCount |
                    oldIndex == newIndex)
                    return;

                _lock.EnterWriteLock();
                try
                {
                    base.MoveItem(oldIndex, newIndex);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);



        }

        /// <summary>
        /// Removes an item
        /// </summary>
        protected override void RemoveItem(int index)
        {

            InvokeIfRequired(_dispatcher, () =>
            {
                if (index >= this.Count)
                    return;

                _lock.EnterWriteLock();
                try
                {
                    base.RemoveItem(index);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }

        /// <summary>
        /// Sets an item
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            InvokeIfRequired(_dispatcher, () =>
            {
                _lock.EnterWriteLock();
                try
                {
                    base.SetItem(index, item);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }, DispatcherPriority.DataBind);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Return as a cloned copy of this Collection
        /// </summary>
        public T[] ToSyncArray()
        {
            _lock.EnterReadLock();
            try
            {
                T[] _sync = new T[this.Count];
                this.CopyTo(_sync, 0);
                return _sync;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        #endregion

        private static void InvokeIfRequired(Dispatcher disp, Action dotIt, DispatcherPriority priority)
        {
            if (disp.Thread != Thread.CurrentThread)
            {
                disp.Invoke(priority, dotIt);
            }
            else
                dotIt();
        }
    }
}

