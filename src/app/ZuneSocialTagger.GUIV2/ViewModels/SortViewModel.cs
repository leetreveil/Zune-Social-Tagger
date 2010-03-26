using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using ZuneSocialTagger.GUIV2.Models;
using ZuneSocialTagger.GUIV2.Properties;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SortViewModel : NotifyPropertyChangedImpl
    {
        private SortOrder _sortOrder;

        public SortViewModel()
        {
            this.SortCommand = new RelayCommand(Sort);
            this.SortOrder = SortOrder.NotSorted;
            SetSortState();
        }

        public event Action<SortOrder> SortClicked = delegate { };

        public RelayCommand SortCommand { get; private set; }

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
            Settings.Default.SortOrder = this.SortOrder;
        }

        public void Sort()
        {
            SetSortState();
            SortClicked.Invoke(this.SortOrder);
        }

        /// <summary>
        /// Move the sort order to the next one in the list
        /// </summary>
        private void SetSortState()
        {
            //skip not sorted as we do not want to display that while looping through sort orders
            List<SortOrder> sortOrders =
                Enum.GetValues(typeof (SortOrder)).Cast<SortOrder>().Where(x => x != SortOrder.NotSorted).ToList();

            int index = sortOrders.IndexOf(this.SortOrder);

            this.SortOrder = index == sortOrders.Count - 1 ? sortOrders[0] : sortOrders[index + 1];

            Settings.Default.SortOrder = this.SortOrder;

            var xub = Settings.Default.SortOrder;
        }
    }
}