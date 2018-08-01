using System;
using Microsoft.Practices.Unity;
using Prism;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.ViewModels
{
    public class MainPageCaptionControlViewModel : BaseRegionUserControlViewModel, IActiveAware
    {
        [Dependency]
        public IRegionContextManager RegionContextManager { get; set; }

        private bool active;

        private string caption;

        public event EventHandler IsActiveChanged;

        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                caption = value;
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
            RegionContextManager.ActionSubscribeChangeRegionContext(RegionName, ChangeRegionContext);
        }

        private void DeActivate()
        {
            RegionContextManager.UnsubscribeChangeRegionContext(RegionName, ChangeRegionContext);
            Caption = String.Empty;
        }

        private void ChangeRegionContext(object regionContext)
        {
            IMainPageCaptionRegionRegionContext mainPageCaptionRegionRegionContext = regionContext as IMainPageCaptionRegionRegionContext;
            Caption = mainPageCaptionRegionRegionContext == null ? String.Empty : mainPageCaptionRegionRegionContext.Caption;
        }
    }
}