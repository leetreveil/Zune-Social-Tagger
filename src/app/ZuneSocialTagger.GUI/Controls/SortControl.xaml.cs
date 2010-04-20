using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZuneSocialTagger.GUI.Converters;
using ZuneSocialTagger.GUI.Models;

namespace ZuneSocialTagger.GUI.Controls
{
    /// <summary>
    /// Interaction logic for SortControl.xaml
    /// </summary>
    public partial class SortControl : UserControl
    {
        public event Action<SortOrder> SortClicked = delegate { };

        public SortControl()
        {
            InitializeComponent();
            SetValue(SortOrderProperty, SortOrder.NotSorted);
        }

        public static readonly DependencyProperty SortOrderProperty =
            DependencyProperty.Register("SortOrder", typeof (SortOrder), typeof (SortControl),
                                        new PropertyMetadata(SortOrderChanged));

        private static void SortOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (SortControl) d;

            view.btnSort.Content = new SortOrderToTextConverter().Convert(e.NewValue, typeof (SortOrder), null, null);
        }

        public SortOrder SortOrder
        {
            get { return (SortOrder) GetValue(SortOrderProperty); }
            set { SetValue(SortOrderProperty, value); }
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            //skip not sorted as we do not want to display that while looping through sort orders
            List<SortOrder> sortOrders =
                Enum.GetValues(typeof (SortOrder)).Cast<SortOrder>().Where(x => x != SortOrder.NotSorted).ToList();

            int index = sortOrders.IndexOf(this.SortOrder);

            SortOrder nextSortOrder = index == sortOrders.Count - 1 ? sortOrders[0] : sortOrders[index + 1];

            this.SetValue(SortOrderProperty, nextSortOrder);

            this.SortClicked.Invoke(nextSortOrder);
        }
    }
}