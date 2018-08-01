using System;
using System.Collections.Generic;
using Prism;
using Prism.Commands;
using Prism.Events;
using WellboreProfileView.Enums;
using WellboreProfileView.Events;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Services;
using WellboreProfileView.ToolBox;

namespace WellboreProfileView.ViewModels
{
    public class MainPageButtonsPanelControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        private ISettingServices settingServices;

        private IRegionContextManager regionContextManager;

        private IEventAggregator eventAggregator;

        private List<DisplayPageRegion> displayPageRegion;

        private DisplayPageRegion currentDisplayPageRegion;

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

        public DisplayPageRegion CurrentDisplayPageRegion
        {
            get { return currentDisplayPageRegion; }
            set
            {
                currentDisplayPageRegion = value;
                RaisePropertyChanged();
                UpDownMultiTableDCommand.RaiseCanExecuteChanged();
                UpdateRegionContext();
            }
        }

        public List<DisplayPageRegion> DisplayPageRegions
        {
            get { return displayPageRegion; }
            set
            {
                displayPageRegion = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand RefreshPageDCommand { get; }

        public DelegateCommand SavePageDCommand { get; }

        public DelegateCommand ImportPageDCommand { get; }

        public DelegateCommand UpDownMultiTableDCommand { get; }

        public MainPageButtonsPanelControlViewModel(IButtonsEventCommandService buttonsEventCommandService, ISettingServices settingServices, IRegionContextManager regionContextManager, IEventAggregator eventAggregator)
        {
            this.settingServices = settingServices;
            this.regionContextManager = regionContextManager;
            this.eventAggregator = eventAggregator;
            RefreshPageDCommand = buttonsEventCommandService.FindOrRegisteredCommands(CommandNames.RefreshPageData);
            SavePageDCommand = buttonsEventCommandService.FindOrRegisteredCommands(CommandNames.SavePageData);
            ImportPageDCommand = buttonsEventCommandService.FindOrRegisteredCommands(CommandNames.ImportPageData);
            UpDownMultiTableDCommand = new DelegateCommand(UpDownMultiTable, CanUpDownMultiTable);
            InitializeDisplayPageRegionTypes();
        }

        private void InitializeDisplayPageRegionTypes()
        {
            DisplayPageRegions = new List<DisplayPageRegion>();
            DisplayPageRegions.Add(new DisplayPageRegion((long)DisplayPageRegionType.OneTable, EnumsHelper.GetDisplayPageRegionTypeName(DisplayPageRegionType.OneTable)));
            DisplayPageRegions.Add(new DisplayPageRegion((long)DisplayPageRegionType.MultiTableText, EnumsHelper.GetDisplayPageRegionTypeName(DisplayPageRegionType.MultiTableText)));
            DisplayPageRegions.Add(new DisplayPageRegion((long)DisplayPageRegionType.MultiTableProfile, EnumsHelper.GetDisplayPageRegionTypeName(DisplayPageRegionType.MultiTableProfile)));
            DisplayPageRegions.Add(new DisplayPageRegion((long)DisplayPageRegionType.MultiTablePlan, EnumsHelper.GetDisplayPageRegionTypeName(DisplayPageRegionType.MultiTablePlan)));
            DisplayPageRegions.Add(new DisplayPageRegion((long)DisplayPageRegionType.MultiTableMultiDraw, EnumsHelper.GetDisplayPageRegionTypeName(DisplayPageRegionType.MultiTableMultiDraw)));
            DisplayPageRegions.Add(new DisplayPageRegion((long)DisplayPageRegionType.MultiTable3D, EnumsHelper.GetDisplayPageRegionTypeName(DisplayPageRegionType.MultiTable3D)));
        }

        private void Activate()
        {
            if (CurrentDisplayPageRegion == null)
            {
                long? saveDisplayPageRegionType = settingServices.GetDisplayPageRegionType();
                if (saveDisplayPageRegionType.HasValue)
                {
                    DisplayPageRegion findDisplayPageRegion = DisplayPageRegions.Find(item => item.Id == saveDisplayPageRegionType.Value);
                    if (findDisplayPageRegion != null)
                        CurrentDisplayPageRegion = findDisplayPageRegion;
                }

                if (CurrentDisplayPageRegion == null)
                    CurrentDisplayPageRegion = DisplayPageRegions[0];
            }
        }

        private void DeActivate()
        {
            if (CurrentDisplayPageRegion != null)
                settingServices.SaveDisplayPageRegionType(CurrentDisplayPageRegion.Id);
        }

        private void UpdateRegionContext()
        {
            regionContextManager.SetRegionContext(RegionName, CurrentDisplayPageRegion.Id);
        }

        private void UpDownMultiTable()
        {
            eventAggregator.GetEvent<UpDownMultiTableEvent>().Publish();
        }

        private bool CanUpDownMultiTable()
        {
            return CurrentDisplayPageRegion != null && EnumsHelper.IsMultiTable(CurrentDisplayPageRegion.Id);
        }
    }
}