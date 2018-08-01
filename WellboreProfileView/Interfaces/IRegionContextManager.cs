using System;
using System.Collections.Generic;

namespace WellboreProfileView.Interfaces
{
    public interface IRegionContextManager
    {
        IList<object> GetPrameters(params object[] parameters);

        void SetRegionContext(string regionName, object context);

        void ActionSubscribeChangeRegionContext(string regionName, Action<object> action);

        void UnsubscribeChangeRegionContext(string regionName, Action<object> action);
    }
}