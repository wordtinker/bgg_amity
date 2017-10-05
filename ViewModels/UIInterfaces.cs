
namespace Amity.ViewModels
{
    public interface IUIBaseService
    {
        // TODO Later
        //void ShowMessage(string message);
        //void BeginInvoke(Action method);
    }

    public interface IUIMainWindowService : IUIBaseService
    {
        string UserName { get; set; }
        bool? ShowUsernameEditor(MainViewModel vm);
        void Shutdown();
    }
}
