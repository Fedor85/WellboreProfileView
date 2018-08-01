using System.Windows.Forms;

namespace WellboreProfileView.Interfaces.Enums
{
    public enum InternalDialogResult
    {
        NoN = -1,

        None = DialogResult.None,

        OK = DialogResult.OK,

        Cancel = DialogResult.Cancel,

        Abort = DialogResult.Abort,

        Retry = DialogResult.None,

        Ignore = DialogResult.Ignore,

        Yes = DialogResult.Yes,

        No = DialogResult.No,

        Refresh
    }
}