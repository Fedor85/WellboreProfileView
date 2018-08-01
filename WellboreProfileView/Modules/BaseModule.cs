using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Modules
{
    public abstract class BaseModule : IModule
    {
        [Dependency]
        public IRegionManager RegionManager { get; set; }

        [Dependency]
        public IUnityContainer UnityContainer { get; set; }

        [Dependency]
        public IControlViewManager ControlViewManager { get; set; }

        public virtual void Initialize()
        {
        }
    }
}