using System;
using System.IO;

namespace Amity.Shared
{
    /// <summary>
    /// Simple class to handle common IO operations.
    /// </summary>
    public static class IOTools
    {
        /// <summary>
        /// Combines an array of string into a path or null;
        /// </summary>
        /// <param name="pathes"></param>
        /// <returns></returns>
        public static string CombinePath(params string[] pathes)
        {
            string path = null;
            try
            {
                path = Path.Combine(pathes);
            }
            catch (Exception e)
            {
            }
            return path;
        }

        /// <summary>
        /// Creates directory unless it already exists.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
