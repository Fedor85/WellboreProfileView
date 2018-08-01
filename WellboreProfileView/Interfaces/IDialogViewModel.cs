using WellboreProfileView.Interfaces.Enums;

namespace WellboreProfileView.Interfaces
{
    public interface IDialogViewModel
    {
        string Title { get; }

        object Content { set; }

        InternalDialogResult DialogResult { get; }
    }
}