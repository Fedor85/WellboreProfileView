using System;
using System.Windows.Controls;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Views
{
    public abstract class BaseRegionNameControl : UserControl, IRegionUserControl
    {
        private string regionName;

        public string RegionName
        {
            get
            {
                return regionName;
            }
            set
            {
                regionName = value;
                if (ChangeRegionName!= null)
                    ChangeRegionName.Invoke();

            }
        }

        public event Action ChangeRegionName;
    }
}