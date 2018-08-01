using System.Windows.Controls;

namespace WellboreProfileView.Views
{
    public abstract class BaseCanvasControl : BaseRegionNameControl
    {
        public abstract Canvas DrawingRange { get;}
    }
}