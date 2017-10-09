using Amity.Models;

namespace Amity.ViewModels
{
    public static class VMBoot
    {
        public static bool IsReadyToLoad(string appDir)
        {
            // TODO STUB restore Rating Register
            // Ensure we have db file to store data.
            Storage.Configure(appDir);
            Cache.Path = appDir;
            return Storage.CreateFile(appDir);
        }
    }
}
