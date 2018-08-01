namespace WellboreProfileView.ViewModels.Interfaces
{
    public interface IValidateGridObject
    {
        bool IsValidate(object parentObject, out string message);
    }
}