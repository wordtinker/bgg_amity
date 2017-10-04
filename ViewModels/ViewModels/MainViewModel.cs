using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;

namespace Amity.ViewModels
{
    public class MainViewModel : BindableBase
    {
        // Members
        private IUIMainWindowService windowService;

        // Properties
        public string UserName
        {
            get { return windowService.UserName; }
            set
            {
                // TODO clear DB?
                windowService.UserName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanGatherGames));
            }
        }
        public bool CanGatherGames
        {
            get { return !string.IsNullOrWhiteSpace(UserName); }
        }

        // Constructors
        public MainViewModel(IUIMainWindowService windowService)
        {
            this.windowService = windowService;

        }

        public DelegateCommand EditUserName
        {
            get
            {
                return new DelegateCommand(() => windowService.ShowUsernameEditor(this));
            }
        }
        public DelegateCommand ExitApp
        {
            get
            {
                return new DelegateCommand(() => windowService.Shutdown());
            }
        }
        public DelegateCommand GetGames
        {
            get
            {
                return new DelegateCommand(async () => await GetOwnerGames())
                    .ObservesCanExecute(() => CanGatherGames);
            }
        }

        private async Task GetOwnerGames()
        {
            await Task.Delay(500);
            // TODO STUB
        }
    }
}
