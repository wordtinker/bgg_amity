
namespace Amity.ViewModels
{
    public interface IUIMainWindowService
    {
        string UserName { get; set; }
        bool? ShowUsernameEditor(MainViewModel vm);
        void Shutdown();
    }
}
