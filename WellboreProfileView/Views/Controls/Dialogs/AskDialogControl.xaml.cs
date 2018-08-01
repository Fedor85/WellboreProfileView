using System.Windows.Controls;
using WellboreProfileView.Interfaces.Controls;

namespace WellboreProfileView.Views
{
    /// <summary>
    /// Interaction logic for AskDialogControl.xaml
    /// </summary>
    public partial class AskDialogControl : UserControl, IDialogControl
    {
        public AskDialogControl()
        {
            InitializeComponent();
        }
    }
}