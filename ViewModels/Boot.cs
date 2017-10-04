
namespace Amity.ViewModels
{
    public static class VMBoot
    {
        public static bool IsReadyToLoad(string appDir)
        {
            // TODO STUB
            // Ensure we have db file to store data.
            //if (!Storage.CreateFile(appDir))
            //{
            //    return false;
            //}
            return true;
        }
    }
}
