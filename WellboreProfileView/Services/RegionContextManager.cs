using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using WellboreProfileView.Events;
using WellboreProfileView.Interfaces;
using WellboreProfileView.RegionContext;
using WellboreProfileView.ViewModels;

namespace WellboreProfileView.Services
{
    public class RegionContextManager : IRegionContextManager
    {
        [Dependency]
        public IEventAggregator EventAggregator { get; set; }

        [Dependency]
        public IRegionManager RegionManager { get; set; }

        public IList<object> GetPrameters(params object[] parameters)
        {
            IList<object> listParameters = new List<object>();
            foreach (object parameter in parameters)
                listParameters.Add(parameter);

            return listParameters;
        }

        public void SetRegionContext(string regionName, object context)
        {
            object regionContext = GetRegionContext(regionName);
            switch (regionName)
            {
                case RegionNames.NavigationTreeViewRegion:
                    SetNavigationTreeViewRegionContext(regionContext as INavigationTreeViewRegionContext, context as BaseTreeViewModel);
                    break;
                case RegionNames.MainPageRegion:
                    SetMainPageRegionContext(regionContext as IMainPageRegionContext, context as INavigationTreeViewRegionContext);
                    break;
                case RegionNames.MainPageButtonsPanelRegion:
                    SetMainPageButtonsPanelRegionContext(regionContext as IMainPageButtonsPanelControlRegionContext, (long)context);
                    break;
                case RegionNames.MainPageCaptionRegion:
                    SetMainPageCaptionRegionContext(regionContext as IMainPageCaptionRegionRegionContext, context as string);
                    break;
                case RegionNames.PageRegion:
                    SetPageRegionContext(regionContext as IPageRegionContext, context);
                    break;
                case RegionNames.MultiPageUpRegion:
                    SetMultiPageUpRegionContext(regionContext as IEntityRegionContext, (long)context);
                    break;
                case RegionNames.MultiPageBottomRegion:
                    SetMultiPageBottomRegionContext(regionContext as IEntityRegionContext, (long)context);
                    break;
            }
        }

        public void ActionSubscribeChangeRegionContext(string regionName, Action<object> action)
        {
            PubSubEvent<object> changeRegionContextEvent = GetChangeRegionContextEvent(regionName);
            if (changeRegionContextEvent != null)
                changeRegionContextEvent.Subscribe(action);

            action(GetRegionContext(regionName));
        }

        public void UnsubscribeChangeRegionContext(string regionName, Action<object> action)
        {
            PubSubEvent<object> changeRegionContextEvent = GetChangeRegionContextEvent(regionName);
            if (changeRegionContextEvent != null)
                changeRegionContextEvent.Unsubscribe(action);

        }

        private void SetNavigationTreeViewRegionContext(INavigationTreeViewRegionContext regionContext, BaseTreeViewModel context)
        {
            regionContext.EntityId = context.Id;
            regionContext.EntityTypeId = context.GetEntityTypeId();
            regionContext.FullName = context.FullName();

            if (EventAggregator.GetEvent<ChangeNavigationTreeViewRegionContextEvent>() != null)
                EventAggregator.GetEvent<ChangeNavigationTreeViewRegionContextEvent>().Publish(regionContext);
        }

        private void SetMainPageRegionContext(IMainPageRegionContext regionContext, INavigationTreeViewRegionContext context)
        {
            regionContext.NavigationTreeViewRegionContext = context;

            if (EventAggregator.GetEvent<ChangeMainPageRegionContextEvent>() != null)
                EventAggregator.GetEvent<ChangeMainPageRegionContextEvent>().Publish(regionContext);
        }

        private void SetMainPageButtonsPanelRegionContext(IMainPageButtonsPanelControlRegionContext regionContext, long context)
        {
            regionContext.DisplayPageRegionTypeId = context;
            if (EventAggregator.GetEvent<ChangeMainPageButtonsPanelControlRegionContextEvent>() != null)
                EventAggregator.GetEvent<ChangeMainPageButtonsPanelControlRegionContextEvent>().Publish(regionContext);
        }

        private void SetMainPageCaptionRegionContext(IMainPageCaptionRegionRegionContext regionContext, string context)
        {
            regionContext.Caption = context;
            if (EventAggregator.GetEvent<ChangeMainPageCaptionRegionContextEvent>() != null)
                EventAggregator.GetEvent<ChangeMainPageCaptionRegionContextEvent>().Publish(regionContext);
        }

        private void SetPageRegionContext(IPageRegionContext pageRegionContext, object context)
        {
            IList<object> listParameters = context as IList<object>;
            pageRegionContext.EntityId = Utils.ConvertParser.GetConvertValue<long>(listParameters[0]);
            pageRegionContext.DisplayPageRegionTypeId = Utils.ConvertParser.GetConvertValue<long>(listParameters[1]);
            if (EventAggregator.GetEvent<ChangePageRegionContextEvent>() != null)
                EventAggregator.GetEvent<ChangePageRegionContextEvent>().Publish(pageRegionContext);
        }

        private void SetMultiPageUpRegionContext(IEntityRegionContext pageRegionContext, long context)
        {
            pageRegionContext.EntityId = context;
            if (EventAggregator.GetEvent<ChangeMultiPageUpRegionContextEvent>() != null)
                EventAggregator.GetEvent<ChangeMultiPageUpRegionContextEvent>().Publish(pageRegionContext);
        }

        private void SetMultiPageBottomRegionContext(IEntityRegionContext pageRegionContext, long context)
        {
            pageRegionContext.EntityId = context;
            if (EventAggregator.GetEvent<ChangeMultiPageBottomRegionContextEvent>() != null)
                EventAggregator.GetEvent<ChangeMultiPageBottomRegionContextEvent>().Publish(pageRegionContext);
        }

        private object GetRegionContext(string regionName)
        {
            IRegion region = RegionManager.Regions[regionName];
            switch (regionName)
            {
                case RegionNames.NavigationTreeViewRegion:
                    if (region.Context == null)
                        region.Context = new NavigationTreeViewRegionContext();
                    break;
                case RegionNames.MainPageRegion:
                    if (region.Context == null)
                        region.Context = new MainPageRegionContext();
                    break;
                case RegionNames.MainPageButtonsPanelRegion:
                    if (region.Context == null)
                        region.Context = new MainPageButtonsPanelControlRegionContext();
                    break;
                case RegionNames.MainPageCaptionRegion:
                    if (region.Context == null)
                        region.Context = new MainPageCaptionRegionRegionContext();
                    break;
                case RegionNames.PageRegion:
                    if (region.Context == null)
                        region.Context = new PageRegionContext();
                    break;
                case RegionNames.MultiPageUpRegion:
                case RegionNames.MultiPageBottomRegion:
                    if (region.Context == null)
                        region.Context = new EntityRegionContext();
                    break;
            }
            return region.Context;
        }

        private PubSubEvent<object> GetChangeRegionContextEvent(string regionName)
        {
            switch (regionName)
            {
                case RegionNames.NavigationTreeViewRegion:
                    return EventAggregator.GetEvent<ChangeNavigationTreeViewRegionContextEvent>();
                case RegionNames.MainPageRegion:
                    return EventAggregator.GetEvent<ChangeMainPageRegionContextEvent>();
                case RegionNames.MainPageButtonsPanelRegion:
                    return EventAggregator.GetEvent<ChangeMainPageButtonsPanelControlRegionContextEvent>();
                case RegionNames.MainPageCaptionRegion:
                    return EventAggregator.GetEvent<ChangeMainPageCaptionRegionContextEvent>();
                case RegionNames.PageRegion:
                    return EventAggregator.GetEvent<ChangePageRegionContextEvent>();
                case RegionNames.MultiPageUpRegion:
                    return EventAggregator.GetEvent<ChangeMultiPageUpRegionContextEvent>();
                case RegionNames.MultiPageBottomRegion:
                    return EventAggregator.GetEvent<ChangeMultiPageBottomRegionContextEvent>();
            }
            return null;
        }
    }
}