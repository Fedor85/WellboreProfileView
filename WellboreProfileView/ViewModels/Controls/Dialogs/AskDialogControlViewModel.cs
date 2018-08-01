using System;
using System.Windows;
using DevExpress.Mvvm;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Enums;

namespace WellboreProfileView.ViewModels
{
    public class AskDialogControlViewModel : BindableBase, IDialogViewModel
    {
        private string message;

        public object Content { set { Message = (string)value;}}

        public InternalDialogResult DialogResult { get; private set; }

        public DelegateCommand OkDCommand { get; }

        public DelegateCommand CanselDCommand { get; }

        public string Title => "Внимание";

        public string Message
        {
            get
            {
                return message;

            }
            set
            {
                message = value;
                RaisePropertyChanged();
            }
        }

        public Window ParentWindow { get; set; }

        public AskDialogControlViewModel()
        {
            Message = String.Empty;
            DialogResult = InternalDialogResult.None;
            OkDCommand = new DelegateCommand(Ok);
            CanselDCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            DialogResult = InternalDialogResult.No;
            ParentWindow.Close();
        }

        private void Ok()
        {
            DialogResult = InternalDialogResult.OK;
            ParentWindow.Close();
        }
    }
}