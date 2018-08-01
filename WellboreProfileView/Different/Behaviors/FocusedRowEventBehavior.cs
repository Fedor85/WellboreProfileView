using System.Collections.Generic;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace WellboreProfileView.Behaviors
{
    public class FocusedRowEventBehavior : Behavior<TableView>
    {
        private Dictionary<int, TableView> hashCodeViews = new Dictionary<int, TableView>();

        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register("FocusedItem", typeof(object), typeof(FocusedRowEventBehavior), new PropertyMetadata(null));

        public object FocusedItem
        {
            get
            {
                return (object)GetValue(DependencyProperty);
            }
            set
            {
                SetValue(DependencyProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            hashCodeViews.Add(AssociatedObject.GetHashCode(), AssociatedObject);
            Attached(AssociatedObject);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            foreach (KeyValuePair<int, TableView> keyValuePair in hashCodeViews)
                Detaching(keyValuePair.Value);
        }

        private void FocusedRowHandleChanged(object sender, FocusedRowHandleChangedEventArgs e)
        {
            FocusedItem = e.RowData.Row;
        }

        private void FocusedViewChanged(object sender, FocusedViewChangedEventArgs e)
        {
            TableView tableView = ((TableView)sender).FocusedView as TableView;
            int currentHashCodeView = tableView.GetHashCode();
            if (!hashCodeViews.ContainsKey(currentHashCodeView))
            {
                hashCodeViews.Add(currentHashCodeView, tableView);
                Attached(tableView);
            }
        }

        private void Attached(TableView tableView)
        {
            tableView.FocusedRowHandleChanged += FocusedRowHandleChanged;
            tableView.FocusedViewChanged += FocusedViewChanged;
        }
        private void Detaching(TableView tableView)
        {
            tableView.FocusedRowHandleChanged -= FocusedRowHandleChanged;
            tableView.FocusedViewChanged -= FocusedViewChanged;
        }
    }
}