using System;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Commands;
using Prism.Events;
using WellboreProfileView.Events;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Controls;
using WellboreProfileView.Interfaces.Enums;
using WellboreProfileView.Interfaces.Services;

namespace WellboreProfileView.ViewModels
{
    public class NavigationButtonsPanelControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IDialogService DialogService { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Dependency]
        public IEventAggregator EventAggregator { get; set; }


        private IButtonsEventCommandService buttonsEventCommandService;

        private bool active;

        public event EventHandler IsActiveChanged;

        public bool IsActive
        {
            get
            {
                return active;
            }
            set
            {
                if (active != value)
                {
                    active = value;
                    if (active)
                        Activate();
                    else
                        DeActivate();
                }
            }
        }

        public DelegateCommand RefreshTreeViewDCommand { get; }

        public DelegateCommand EditTreeViewDCommand { get; }


        public NavigationButtonsPanelControlViewModel(IButtonsEventCommandService buttonsEventCommandService)
        {
            this.buttonsEventCommandService = buttonsEventCommandService;
            RefreshTreeViewDCommand = buttonsEventCommandService.FindOrRegisteredCommands(CommandNames.RefreshTreeViewDCommand);
            EditTreeViewDCommand = buttonsEventCommandService.FindOrRegisteredCommands(CommandNames.EditTreeViewDCommand);
        }

        private void Activate()
        {
            buttonsEventCommandService.Subscribe(CommandNames.EditTreeViewDCommand, EditTreeView, true);
        }

        private void DeActivate()
        {
            buttonsEventCommandService.UnSubscribe(CommandNames.EditTreeViewDCommand, EditTreeView);
        }

        private void EditTreeView()
        {
            EventAggregator.GetEvent<SaveDataChangeEvent>().Publish();
            InternalDialogResult dialogResult = DialogService.ShowDialog(UnityContainer.Resolve<IDialogControl>(ControlNames.EditAreasAndWellsControl));
            if (dialogResult == InternalDialogResult.Refresh)
                buttonsEventCommandService.Run(CommandNames.RefreshTreeViewDCommand);
        }
    }
}