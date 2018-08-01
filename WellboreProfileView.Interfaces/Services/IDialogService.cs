using WellboreProfileView.Interfaces.Controls;
using WellboreProfileView.Interfaces.Enums;

namespace WellboreProfileView.Interfaces.Services
{
    public interface IDialogService
    {
        InternalDialogResult ShowDialog<T>(T context) where T : IDialogControl;

        InternalDialogResult Ask(string message);

        void Message(string message);

        string OpenFileDialog(string filter);
    }
}