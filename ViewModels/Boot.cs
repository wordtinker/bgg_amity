using Amity.Models;

namespace Amity.ViewModels
{
    public static class VMBoot
    {
        public static bool IsReadyToLoad(string appDir)
        {
            // Ensure we have db file to store data.
            Storage.Configure(appDir);
            Cache.Path = appDir;
            return Storage.CreateFile(appDir);
        }
    }
}
