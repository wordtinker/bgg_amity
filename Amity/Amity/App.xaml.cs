using System;
using System.Windows;
using Amity.Shared;
using Amity.ViewModels;

namespace Amity
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string appName = "Amity"; 
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            // Get app directory
            string appDir;
            try
            {
                appDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
            catch (PlatformNotSupportedException)
            {
                MessageBox.Show("Platform is not supported.\nAmity can't start.");
                return;
            }
            appDir = IOTools.CombinePath(appDir, appName);
            // Save application directory path for later
            Current.Properties["appDir"] = appDir;

            // Create directory if not exist
            if (!IOTools.CreateDirectory(appDir))
            {
                MessageBox.Show(string.Format("Something bad happened in {0} directory.\nAmity can't start.", appDir));
                return;
            }

            // Make sure VM is ready to start.
            if (!VMBoot.IsReadyToLoad(appDir))
            {
                MessageBox.Show(string.Format("Can't start the app."));
                return;
            }

            // Start the main window
            MainWindow wnd = new MainWindow();
            wnd.Show();
        }
    }
}
