using System.Collections.Generic;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Behaviors
{
    public class NavigationDeleteSelectedRowsEventBehavior : Behavior<TableView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.CommandBindings.Add(new CommandBinding(GridCommands.DeleteFocusedRow, OnExecuted));
        }

        private void OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TableView view = AssociatedObject.FocusedView as TableView;
            if (view.SelectedRows.Count != 0)
            {
                view.Grid.BeginDataUpdate();
                List<object> removeItems = new List<object>();
                foreach (int selectedLogItem in view.GetSelectedRowHandles())
                    removeItems.Add(view.Grid.GetRow(selectedLogItem));

                IRemoveRangeCollection removeRangeCollection = view.Grid.ItemsSource as IRemoveRangeCollection;
                removeRangeCollection.RemoveRange(removeItems);
                view.Grid.EndDataUpdate();
            }
            else if (view.FocusedRowHandle != GridControl.InvalidRowHandle)
            {
                view.DeleteRow(view.FocusedRowHandle);
            }
        }
    }
}
