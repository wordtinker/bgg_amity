using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using Amity.Models;
using System.Collections.ObjectModel;

namespace Amity.ViewModels
{
    public class MainViewModel : BindableBase
    {
        // Members
        private IUIMainWindowService windowService;
        private bool processing;

        // Properties
        public string UserName
        {
            get { return windowService.UserName; }
            set
            {
                windowService.UserName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CanGatherGames));
            }
        }
        public bool Processing
        {
            get { return processing; }
            set {
                processing = value;
                RaisePropertyChanged(nameof(CanGatherGames));
            }
        }
        public bool CanGatherGames
        {
            get { return !string.IsNullOrWhiteSpace(UserName) && ! Processing; }
        }
        public ObservableCollection<Game> Games { get; }

        // Constructors
        public MainViewModel(IUIMainWindowService windowService)
        {
            this.windowService = windowService;
            this.Games = new ObservableCollection<Game>();
            Storage.GetGames().ForEach(Games.Add);
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
            // Clear old data from DB and view
            Storage.DeleteAllGames();
            Games.Clear();
            // Block button
            Processing = true;
            // Ask BGG API for list of games for specific user
            var gameList = await Task.Run(
                () => BGGAPI.GetGamesForUser(UserName, RatingRegister.Instance.Min, RatingRegister.Instance.Max));
            // Update view and DB
            foreach (Game g in gameList)
            {
                Storage.AddGame(g);
                Games.Add(g);
            }
            // Unblock button
            Processing = false;
        }
    }
}
