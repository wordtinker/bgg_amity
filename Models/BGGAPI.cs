using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Amity.Models
{
    public struct Game
    {
        public string Name { get; internal set; }
        public string ID { get; internal set; }
        public byte Rating { get; internal set; }
    }

    internal static class Converter
    {
        internal static byte RatingFromString(string rate)
        {
            // use "en-US" for proper "." conversion
            return Convert.ToByte(Convert.ToDouble(rate, new CultureInfo("en-US")));
        }
    }

    public static class FilterExtensions
    {
        public static List<Game> FilterGames(this XDocument doc)
        {
            try
            {
                var games = from node in doc.Root.Elements("item")
                            select new Game
                            {
                                Name = node.Value,
                                ID = node.Attribute("objectid").Value,
                                Rating = Converter.RatingFromString(node.Descendants("rating").Single().Attribute("value").Value)
                            };
                return games.ToList();
            }
            catch (Exception)
            {
                return new List<Game>();
            }
            
        }
        public static List<User> FilterNames(this XDocument doc)
        {
            try
            {
                var users = from node in doc.Root.Descendants("comment")
                            select new User
                            {
                                Name = node.Attribute("username").Value,
                                Rating = Converter.RatingFromString(node.Attribute("rating").Value)
                            };
                return users.ToList();
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }
    }

    public interface IBGGAPI
    {
        Task<XDocument> GetGamesForUser(string username, int minRating, int maxRating);
        Task<XDocument> GetUsersOfTheGame(string gameId, int pageNumber);
    }

    public class BGGAPI : IBGGAPI
    {
        private const int DEFAULT_DELAY = 1000;
        private async Task<XDocument> GetXMLFrom(string uri)
        {
            // use separate instance every time instead one static instance
            // better spam prevention
            using (HttpClient client = new HttpClient())
            {
                do
                {
                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    {
                        // wait before asking for result again
                        await Task.Delay(DEFAULT_DELAY);
                        // throw on errors
                        response.EnsureSuccessStatusCode();

                        if (response.StatusCode != HttpStatusCode.OK) continue;

                        // Gather results
                        using (HttpContent content = response.Content)
                        {
                            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                            XDocument doc;
                            using (MemoryStream ms = new MemoryStream(bytes))
                            {
                                doc = XDocument.Load(ms);
                            }
                            return doc;
                        }
                    }
                } while (true);
            }
        }

        public async Task<XDocument> GetGamesForUser(string username, int minRating, int maxRating)
        {
            string baseURI = "https://www.boardgamegeek.com/xmlapi2/collection?username={0}&subtype=boardgame&excludesubtype=boardgameexpansion&rated=1&stats=1&brief=1&minrating={1}&rating={2}";
            string URI = string.Format(baseURI, username, minRating, maxRating);
            return await GetXMLFrom(URI);
        }

        public async Task<XDocument> GetUsersOfTheGame(string gameId, int pageNumber)
        {
            string baseURI = "https://www.boardgamegeek.com/xmlapi2/thing?type=boardgame&id={0}&ratingcomments=1&page={1}&pagesize=100";
            string URI = string.Format(baseURI, gameId, pageNumber);
            return await GetXMLFrom(URI);
        }
    }
}
