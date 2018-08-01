using System.Windows.Controls;

using WellboreProfileView.Interfaces;

namespace WellboreProfileView.Views
{
    /// <summary>
    /// Interaction logic for DrawRangeControl.xaml
    /// </summary>
    public partial class DrawRangeControl : BaseCanvasControl, IDrawRangeControl
    {
        public DrawRangeControl()
        {
            InitializeComponent();
        }

        public override Canvas DrawingRange { get { return Canvas;} }
    }
}