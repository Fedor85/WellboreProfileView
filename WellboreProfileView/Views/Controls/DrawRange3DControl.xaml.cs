using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Views
{
    /// <summary>
    /// Interaction logic for DrawRange3DControl.xaml
    /// </summary>
    public partial class DrawRange3DControl : BaseViewport3DControl, IDraw3DControl
    {
        public DrawRange3DControl()
        {
            InitializeComponent();
            DrawingRange3D.SubstrateViewport3D = SubstrateViewport3D;
        }

        public override FeedbackViewport3D DrawingRange3D { get { return FeedbackViewport3D; } }
    }
}