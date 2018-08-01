using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Prism.Regions;
using WellboreProfileView.Enums;
using WellboreProfileView.Events;
using WellboreProfileView.ToolBox;
using WellboreProfileView.ViewModels;
using WellboreProfileView.Views;

namespace WellboreProfileView.Test
{
    [TestClass]
    public class BootstrapperTestFixture
    {
        [TestMethod]
        public void BootstrapperTest()
        {
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
            IUnityContainer container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            IRegionManager regionManager = container.Resolve<IRegionManager>();
            IEventAggregator eventAggregator = container.Resolve<IEventAggregator>();
            NavigationControlViewModel navigationControlViewModel = GetNavigationControlViewModel(regionManager);
            List<long> areaIds = GetRandomIds(navigationControlViewModel.Root.Areas, 5);
            foreach (long areaId in areaIds)
                navigationControlViewModel.Root.SetSelectItem((long)EntityType.Area, areaId);

            List<BaseTreeViewModel> allwell = new List<BaseTreeViewModel>();
            foreach (BaseTreeViewModel area in navigationControlViewModel.Root.Areas)
                allwell.AddRange(area.Childs);

            List<long> wellIds = GetRandomIds(allwell, 5);
            MainPageButtonsPanelControlViewModel mainPageButtonsPanelControlViewModel = GetMainPageButtonsPanelControlViewModel(regionManager);
            foreach (long wellId in wellIds)
            {
                navigationControlViewModel.Root.SetSelectItem((long)EntityType.Well, wellId);
                mainPageButtonsPanelControlViewModel.RefreshPageDCommand.Execute();
                foreach (DisplayPageRegion displayPageRegion in mainPageButtonsPanelControlViewModel.DisplayPageRegions)
                {
                    mainPageButtonsPanelControlViewModel.CurrentDisplayPageRegion = displayPageRegion;
                    eventAggregator.GetEvent<UpDownMultiTableEvent>().Publish();
                    eventAggregator.GetEvent<UpDownMultiTableEvent>().Publish();
                } 
            }
        }

        private List<long> GetRandomIds(IList<BaseTreeViewModel> viewModels, int count)
        {
            List<long> allIds = new List<long>();
            foreach (BaseTreeViewModel viewModel in viewModels)
                allIds.Add(viewModel.Id);

            int allCount = allIds.Count;
            if (allCount < 1 || allCount <= count)
                return allIds;

            List<long> ids = new List<long>();
            Random random = new Random(0);
            while (ids.Count < count)
            {
                long randomId = allIds[random.Next(allCount)];
                if (!ids.Contains(randomId))
                    ids.Add(randomId);
            }
            return ids;
        }

        private NavigationControlViewModel GetNavigationControlViewModel(IRegionManager regionManager)
        {
            IRegion navigationTreeViewRegion = regionManager.Regions[RegionNames.NavigationTreeViewRegion];
            NavigationControl navigationControl = navigationTreeViewRegion.GetView(ControlNames.NavigationControl) as NavigationControl;
            return navigationControl.DataContext as NavigationControlViewModel;
        }

        private MainPageButtonsPanelControlViewModel GetMainPageButtonsPanelControlViewModel(IRegionManager regionManager)
        {
            IRegion mainPageButtonsPanelRegion = regionManager.Regions[RegionNames.MainPageButtonsPanelRegion];
            MainPageButtonsPanelControl navigationControl = mainPageButtonsPanelRegion.GetView(ControlNames.MainPageButtonsPanelControl) as MainPageButtonsPanelControl;
            return navigationControl.DataContext as MainPageButtonsPanelControlViewModel;
        }
    }
}