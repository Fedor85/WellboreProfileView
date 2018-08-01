using System;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Enums;

namespace WellboreProfileView.ViewModels
{
    public class MessageDialogControlViewModel : BindableBase, IDialogViewModel
    {
        private string message;

        public object Content { set { Message = (string)value; } }

        public string Title => "Внимание";

        public InternalDialogResult DialogResult { get; private set; }

        public DelegateCommand OkDCommand { get; }

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

        public MessageDialogControlViewModel()
        {
            Message = String.Empty;
            DialogResult = InternalDialogResult.None;
            OkDCommand = new DelegateCommand(Ok);
        }

        private void Ok()
        {
            DialogResult = InternalDialogResult.OK;
            ParentWindow.Close();
        }
    }
}