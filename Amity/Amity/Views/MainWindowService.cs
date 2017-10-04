using Amity.ViewModels;

namespace Amity.Views
{
    class MainWindowService : IUIMainWindowService
    {
        private MainWindow mainWindow;

        public MainWindowService(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public string UserName
        {
            get
            {
                // Fetch owner username from config file, it could be empty.
                return Properties.Settings.Default.UserName;
            }
            set
            {
                Properties.Settings.Default.UserName = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool? ShowUsernameEditor(MainViewModel vm)
        {
            UserWindow dialog = new UserWindow();
            dialog.DataContext = vm;
            //// Ensure the alt+tab is working properly.
            dialog.Owner = mainWindow;
            return dialog.ShowDialog();
        }

        public void Shutdown()
        {
            App.Current.Shutdown();
        }
    }
}
