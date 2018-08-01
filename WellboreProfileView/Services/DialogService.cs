using System;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Practices.Unity;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Controls;
using WellboreProfileView.Interfaces.Enums;
using WellboreProfileView.Interfaces.Services;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace WellboreProfileView.Services
{
    public sealed class DialogService : IDialogService
    {
        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        public InternalDialogResult ShowDialog<T>(T context) where T : IDialogControl
        {
            return Invoke(context, null);
        }

        public InternalDialogResult Ask(string message)
        {
            IDialogControl askDialogControlViewModel = UnityContainer.Resolve<IDialogControl>(ControlNames.AskDialogControl);
            return Invoke(askDialogControlViewModel, message);
        }

        public void Message(string message)
        {
            IDialogControl askDialogControlViewModel = UnityContainer.Resolve<IDialogControl>(ControlNames.MessageDialogControlViewModel);
            Invoke(askDialogControlViewModel, message);
        }

        public string OpenFileDialog(string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }

        private InternalDialogResult Invoke<T>(T dialogView, object content) where T : IDialogControl
        {
            FrameworkElement view = dialogView as FrameworkElement;
            Window wrapperWindow = GetWindow(view);
            IDialogViewModel dialogViewModel = (IDialogViewModel)view.DataContext;
            if (content != null)
                dialogViewModel.Content = content;

            wrapperWindow.Title = dialogViewModel.Title;
            wrapperWindow.ShowDialog();
            return dialogViewModel.DialogResult;
        }

        private Window GetWindow(FrameworkElement view)
        {
            Window wrapperWindow = new Window
                                       {
                                           ShowInTaskbar = false,
                                           WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                           WindowState = WindowState.Normal,
                                           SizeToContent = SizeToContent.WidthAndHeight,
                                           HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                           VerticalContentAlignment = VerticalAlignment.Stretch
                                       };

            Window mainWindow = Application.Current.MainWindow;

            if (mainWindow != null)
            {
                wrapperWindow.Owner = mainWindow;
                wrapperWindow.Icon = mainWindow.Icon;
            }

            wrapperWindow.Content = view;
            if (IsNoResize(view))
            {
                wrapperWindow.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                wrapperWindow.MinHeight = view.MinHeight + 50;
                wrapperWindow.MinWidth = view.MinWidth + 25;
                wrapperWindow.MaxHeight = view.MaxHeight;
                wrapperWindow.MaxWidth = view.MaxWidth;
            }
            return wrapperWindow;
        }

        private bool IsNoResize(FrameworkElement view)
        {
            return view.MaxHeight == double.PositiveInfinity || view.MaxHeight == Double.PositiveInfinity;
        }
    }
}