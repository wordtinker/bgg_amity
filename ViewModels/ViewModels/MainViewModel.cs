using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using Amity.Models;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;

namespace Amity.ViewModels
{
    public class MainViewModel : BindableBase
    {
        // Members
        private IUIMainWindowService windowService;
        private bool processing;
        private int progressValue;
        private string log;

        // Properties
        public string UserName
        {
            get { return windowService.UserName; }
            set
            {
                windowService.UserName = value;
                RaisePropertyChanged();
            }
        }
        public bool Processing
        {
            get { return processing; }
            set
            {
                SetProperty(ref processing, value);
            }
        }
        public bool CanGatherGames
        {
            get { return !string.IsNullOrWhiteSpace(UserName) && !Processing; }
        }
        public bool CanGatherUsers
        {
            get { return !string.IsNullOrWhiteSpace(UserName)
                    && !Processing
                    && Games.Count != 0; }
        }
        public ObservableCollection<Game> Games { get; }
        public ObservableCollection<User> Users { get; }
        public int ProgressValue
        {
            get { return progressValue; }
            set { SetProperty(ref progressValue, value); }
        }
        public string Log
        {
            get { return log; }
            set { SetProperty(ref log, value); }
        }
        // Constructors
        public MainViewModel(IUIMainWindowService windowService)
        {
            this.windowService = windowService;
            this.Games = new ObservableCollection<Game>();
            this.Users = new ObservableCollection<User>();
            Storage.GetGames().ForEach(Games.Add);
            Storage.GetUsers().ForEach(Users.Add);
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
                    .ObservesProperty(() => UserName)
                    .ObservesProperty(() => Processing)
                    .ObservesCanExecute(() => CanGatherGames);
            }
        }
        public DelegateCommand GetUsers
        {
            get
            {
                return new DelegateCommand(async () => await GetSimilarUsers())
                    .ObservesProperty(() => UserName)
                    .ObservesProperty(() => Processing)
                    .ObservesProperty(() => Games)
                    .ObservesCanExecute(() => CanGatherUsers);
            }
        }

        private async Task GetOwnerGames()
        {
            // Block buttons 
            Processing = true;
            // Clear old data from DB and view
            Storage.DeleteAllGames();
            Games.Clear();
            // Ask BGG API for list of games for specific user
            List<Game> gameList = await Task.Run(
                () => BGGAPI.GetGamesForUser(UserName, RatingRegister.Instance.Min, RatingRegister.Instance.Max));
            // Update view and DB
            Storage.AddGames(gameList);
            gameList.ForEach(Games.Add);
            
            // Unblock buttons
            Processing = false;
        }

        private async Task GetSimilarUsers()
        {
            // Block buttons
            Processing = true;
            // Clear old data from DB and view
            Storage.DeleteAllUsers();
            Users.Clear();

            // Mark progress
            ProgressValue = 0;

            // ask analyzer to fetch list of similar users
            List<User> userList = await Task.Run(
                () => Analyzer.Run(new Progress<Tuple<double, string>>(
                    p =>
                    {
                        //Update the visual progress of the analysis.
                        ProgressValue = Convert.ToInt32(p.Item1);
                        if (!string.IsNullOrEmpty(p.Item2))
                        {
                            Log = string.Format("{0} is ready!", p.Item2);
                        }
                    }
                )));
            // Update the visual progress.
            ProgressValue = 100;
            // either show users or warning that none is found
            if (userList.Count == 0)
            {
                Log = "Sorry, no similar users.";
            }
            else
            {
                Storage.AddUsers(userList);
                userList.ForEach(Users.Add);
                Log = "Found some users";
            }

            // Unblock buttons
            Processing = false;
        }
    }
}
