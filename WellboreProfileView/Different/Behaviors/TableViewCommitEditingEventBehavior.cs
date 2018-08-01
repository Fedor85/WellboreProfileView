using System;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace WellboreProfileView.Behaviors
{
    public class TableViewCommitEditingEventBehavior : Behavior<TableView>
    {

        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register("CommitEditing", typeof(Func<bool>), typeof(TableViewCommitEditingEventBehavior), new PropertyMetadata(null));

        public Func<bool> CommitEditing
        {
            get
            {
                return (Func<bool>)GetValue(DependencyProperty);
            }
            set
            {
                SetValue(DependencyProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PropertyChanged += AssociatedObjectPropertyChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PropertyChanged -= AssociatedObjectPropertyChanged;

            CommitEditing = null;
        }

        private void AssociatedObjectPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CommitEditing = AssociatedObject.CommitEditing;

        }
    }
}