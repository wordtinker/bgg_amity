using System.Threading.Tasks;
using System.Xml.Linq;
using Amity.Shared;
using System;

namespace Amity.Models
{
    /// <summary>
    /// Use that class instead of HttpClient cache. BGG API response headers force no caching.
    /// </summary>
    public class Cache : IBGGAPI
    {
        private IBGGAPI handler;
        private string FileName;
        public static string Path { get; set; }

        public Cache(IBGGAPI handler)
        {
            this.handler = handler;
            FileName = "{0}_{1}.xml";
        }

        public async Task<XDocument> GetGamesForUser(string username, int minRating, int maxRating)
        {
            return await handler.GetGamesForUser(username, minRating, maxRating);
        }

        public async Task<XDocument> GetUsersOfTheGame(string gameId, int pageNumber)
        {
            // TODO Later expiration
            string name = string.Format(FileName, gameId, pageNumber);
            string filePath = IOTools.CombinePath(Path, name);
            // if cached return
            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                return XDocument.Load(filePath);
            }
            // read and save
            else
            {
                XDocument doc = await handler.GetUsersOfTheGame(gameId, pageNumber);
                if (!string.IsNullOrEmpty(filePath))
                {
                    doc.Save(filePath);
                }
                return doc;
            }
        }
    }
}
