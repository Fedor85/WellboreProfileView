using System.Windows.Controls;
using WellboreProfileView.Interfaces.Controls;

namespace WellboreProfileView.Views
{
    /// <summary>
    /// Interaction logic for MessageDialogControl.xaml
    /// </summary>
    public partial class MessageDialogControl : UserControl, IDialogControl
    {
        public MessageDialogControl()
        {
            InitializeComponent();
        }
    }
}