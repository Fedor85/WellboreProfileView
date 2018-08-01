using System;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Grid;
using Prism.Mvvm;
using WellboreProfileView.ViewModels.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public abstract class BaseRootGridViewModel<T>: BindableBase where T : BindableBase
    {
        private object cuurentItem;

        public Func<bool> CommitEditing { get; set; }

        public event Action<object> ChangeCurrentItem;

        public object CurrentItem
        {
            get { return cuurentItem; }
            set
            {
                cuurentItem = value;
                if (ChangeCurrentItem != null)
                    ChangeCurrentItem.Invoke(value);

                RaisePropertyChanged();
            }
        }

        public SmartObservableCollection<T> MainItems { get; protected set; }

        public bool RunCommitEditing()
        {
            if (CommitEditing != null)
                return CommitEditing();

            return true;
        }

        public void SetDefaultCurrentItem()
        {
            if (MainItems.Count > 0)
                CurrentItem = MainItems[0];
        }

        [Command(false)]
        public void ValidateRow(GridRowValidationEventArgs args)
        {
            IValidateGridObject validateGridObject = args.Row as IValidateGridObject;
            if (validateGridObject == null)
                return;
           
            string message = String.Empty;
            if (!validateGridObject.IsValidate(((GridControl)args.Source).DataContext, out message))
            {
                args.IsValid = false;
                args.ErrorContent = message;
            }
        }
    }
}