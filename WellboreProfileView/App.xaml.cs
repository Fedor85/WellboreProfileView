using System;
using System.Windows;

namespace WellboreProfileView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Bootstrapper bootstrapper = new Bootstrapper();
            try
            {
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown();
            }
        }
    }
}