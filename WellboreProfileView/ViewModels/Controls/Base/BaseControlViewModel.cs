using System;
using System.ComponentModel;

namespace WellboreProfileView.ViewModels
{
    public abstract class BaseControlViewModel : BaseRegionUserControlViewModel
    {
        public Action<CancelEventArgs> WindowClosingHandler => ClosingUserControl;

        protected abstract void ClosingUserControl(CancelEventArgs cansel);
    }
}