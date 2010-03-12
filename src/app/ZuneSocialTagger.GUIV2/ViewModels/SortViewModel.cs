using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Core;
using ZuneSocialTagger.GUIV2.Models;

namespace ZuneSocialTagger.GUIV2.ViewModels
{
    public class SortViewModel : PropertyChangedBase
    {
        private SortOrder _sortOrder;

        public SortViewModel()
        {
            this.SortOrder = SortOrder.NotSorted;
            SetSortState();
        }

        public event Action<SortOrder> SortClicked = delegate { };

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
        }
    }
}