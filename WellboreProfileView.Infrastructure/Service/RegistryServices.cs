using System;
using Microsoft.Win32;
using Utils;
using WellboreProfileView.Interfaces.Services;

namespace WellboreProfileView.Infrastructure.Services
{
    public class RegistryServices : ISettingServices
    {
        private const string SettingsRegistryKey = "Software\\WellboreProfileView\\Settings";

        private const string LastOpenedNavigationTreeViewItem = "LastOpenedNavigationTreeViewItem";

        private const string DisplayPageRegionType = "DisplayPageRegionType";

        private const string TreeViewWellSplitterDistance = "TreeViewWellSplitterDistance";

        private const string MultiTablePageControlSplitterDistance = "MultiTablePageControlSplitterPosition";

        private const string MultiTablePositionType = "MultiTablePositionType";

        public string GetLastOpenedNavigationTreeViewItem()
        {
            RegistryKey registryKey = GetRegistryKey();
            string node = GetStringValue(registryKey, LastOpenedNavigationTreeViewItem);
            registryKey.Close();
            return node;
        }

        public void SaveLastOpenedNavigationTreeViewItem(long nodeTypeId, long nodeId)
        {
            RegistryKey registryKey = GetRegistryKey();
            registryKey.SetValue(LastOpenedNavigationTreeViewItem, StringHelper.GetTreeViewItemSaveValue(nodeTypeId, nodeId));
            registryKey.Close();
        }

        public double? GetTreeViewWellSplitterDistance()
        {
            RegistryKey registryKey = GetRegistryKey();
            double? splitterDistance = GetValue<double>(registryKey, TreeViewWellSplitterDistance);
            registryKey.Close();
            return splitterDistance;
        }

        public void SaveTreeViewWellSplitterDistance(double width)
        {
            RegistryKey registryKey = GetRegistryKey();
            registryKey.SetValue(TreeViewWellSplitterDistance, width);
            registryKey.Close();
        }

        public long? GetDisplayPageRegionType()
        {
            RegistryKey registryKey = GetRegistryKey();
            int? displayPageRegionType = GetValue<int>(registryKey, DisplayPageRegionType);
            registryKey.Close();
            return displayPageRegionType;
        }

        public void SaveDisplayPageRegionType(long displayPageRegionType)
        {
            RegistryKey registryKey = GetRegistryKey();
            registryKey.SetValue(DisplayPageRegionType, displayPageRegionType);
            registryKey.Close();
        }

        public double? GetMultiTablePageControlSplitterDistance()
        {
            RegistryKey registryKey = GetRegistryKey();
            double? splitterDistance = GetValue<double>(registryKey, MultiTablePageControlSplitterDistance);
            registryKey.Close();
            return splitterDistance;
        }

        public void SaveMultiTablePageControlSplitterDistance(double height)
        {
            RegistryKey registryKey = GetRegistryKey();
            registryKey.SetValue(MultiTablePageControlSplitterDistance, height);
            registryKey.Close();
        }

        public long? GetMultiTablePositionTypeId()
        {
            RegistryKey registryKey = GetRegistryKey();
            long? multiTablePositionTypeId = GetValue<long>(registryKey, MultiTablePositionType);
            registryKey.Close();
            return multiTablePositionTypeId;
        }

        public void SaveMultiTablePositionType(long multiTablePositionTypeId)
        {
            RegistryKey registryKey = GetRegistryKey();
            registryKey.SetValue(MultiTablePositionType, multiTablePositionTypeId);
            registryKey.Close();
        }

        private RegistryKey GetRegistryKey()
        {
            return Registry.CurrentUser.CreateSubKey(SettingsRegistryKey);
        }

        private string GetStringValue(RegistryKey registryKey, string name)
        {
            object value = registryKey.GetValue(name);
            if (value == null)
                return String.Empty;

            return ConvertParser.GetConvertValue<string>(value);
        }

        private T? GetValue<T>(RegistryKey registryKey, string name) where T : struct
        {
            object value = registryKey.GetValue(name);
            if (value == null)
                return null;

            return ConvertParser.GetConvertValue<T>(value);
        }
    }
}
