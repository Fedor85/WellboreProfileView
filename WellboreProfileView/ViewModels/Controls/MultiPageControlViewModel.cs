using System;
using DevZest.Windows;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Events;
using WellboreProfileView.Enums;
using WellboreProfileView.Events;
using WellboreProfileView.Interfaces;
using WellboreProfileView.Interfaces.Services;

namespace WellboreProfileView.ViewModels
{
    public class MultiPageControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IRegionContextManager RegionContextManager { get; set; }

        [Dependency]
        public IControlViewManager ControlViewManager { get; set; }

        [Dependency]
        public ISettingServices SettingServices { get; set; }

        [Dependency]
        public IEventAggregator eventAggregator { get; set; }

        private SplitterDistance splitterDistance;

        private IPageRegionContext pageRegionContext;

        private long multiTablePositionTypeId { get; set; }

        private bool active;

        public event EventHandler IsActiveChanged;

        public SplitterDistance SplitterDistance
        {
            get
            {
                return splitterDistance;
            }
            set
            {
                splitterDistance = value;
                RaisePropertyChanged();
            }
        }

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

        private void Activate()
        {
            InitializeMultiTablePositionType();
            eventAggregator.GetEvent<UpDownMultiTableEvent>().Subscribe(UpDownMultiTable);
            RegionContextManager.ActionSubscribeChangeRegionContext(RegionName, ChangeRegionContext);
            InitializeSplitterDistance();
        }

        private void DeActivate()
        {
            eventAggregator.GetEvent<UpDownMultiTableEvent>().Unsubscribe(UpDownMultiTable);
            ControlViewManager.DeactivateAllActiveViewToMultiPageControl();
            RegionContextManager.UnsubscribeChangeRegionContext(RegionName, ChangeRegionContext);
            SaveSplitterDistance();
            SaveMultiTablePositionType();
        }

        private void ChangeRegionContext(object regionContext)
        {
            pageRegionContext = regionContext as IPageRegionContext;
            UpdateMultiRegionContext();
        }

        private void UpDownMultiTable()
        {
            multiTablePositionTypeId = EnumsHelper.GetNextMultiTablePositionTypeId(multiTablePositionTypeId);
            UpdateMultiRegionContext();
        }

        private void UpdateMultiRegionContext()
        {
            ControlViewManager.DeactivateAllActiveViewToMultiPageControl();
            RegionContextManager.SetRegionContext(RegionNames.MultiPageUpRegion, pageRegionContext.EntityId);
            RegionContextManager.SetRegionContext(RegionNames.MultiPageBottomRegion, pageRegionContext.EntityId);
            ControlViewManager.ActivateMultiPageControlViews(pageRegionContext.DisplayPageRegionTypeId, multiTablePositionTypeId);
        }

        private void InitializeSplitterDistance()
        {
            double? splitterDistance = SettingServices.GetMultiTablePageControlSplitterDistance();
            SplitterDistance = new SplitterDistance(splitterDistance.HasValue ? splitterDistance.Value : 0, SplitterUnitType.Star);
        }

        private void InitializeMultiTablePositionType()
        {
            long? saveMultiTablePositionTypeId = SettingServices.GetMultiTablePositionTypeId();
            multiTablePositionTypeId = saveMultiTablePositionTypeId.HasValue ? saveMultiTablePositionTypeId.Value : (long)MultiTablePositionType.Down;
        }

        private void SaveSplitterDistance()
        {
            SettingServices.SaveMultiTablePageControlSplitterDistance(SplitterDistance.Value);
        }

        private void SaveMultiTablePositionType()
        {
            SettingServices.SaveMultiTablePositionType(multiTablePositionTypeId);
        }
    }
}