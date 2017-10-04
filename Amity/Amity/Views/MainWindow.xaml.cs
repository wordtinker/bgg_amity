using System.Windows;
using Amity.Views;
using Amity.ViewModels;

namespace Amity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Constructor
        public MainWindow()
        {
            MainWindowService service = new MainWindowService(this);
            this.DataContext = new MainViewModel(service);
            InitializeComponent();
        }
    }
}
