namespace WellboreProfileView.Views
{
    public abstract class BaseViewport3DControl : BaseRegionNameControl
    {
        public abstract FeedbackViewport3D DrawingRange3D { get; }
    }
}