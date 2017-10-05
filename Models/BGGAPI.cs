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
    public class Game
    {
        public string Name { get; internal set; }
        public string ID { get; internal set; }
        public double Rating { get; internal set; }

        internal static double RatingFromString(string rate)
        {
            // use "en-US" for proper "." conversion
            return Convert.ToDouble(rate, new CultureInfo("en-US"));
        }
    }

    public static class FilterExtensions
    {
        public static List<Game> FilterGames(this XDocument doc)
        {
            var games = from node in doc.Root.Elements("item")
                        select new Game
                        {
                            Name = node.Value,
                            ID = node.Attribute("objectid").Value,
                            Rating = Game.RatingFromString(node.Descendants("rating").Single().Attribute("value").Value)
                        };
            return games.ToList();
        }
    }

    public static class BGGAPI
    {
        private static async Task<XDocument> GetXMLFrom(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                do
                {
                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    {
                        // wait before asking for result again
                        await Task.Delay(500);
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

        public static async Task<IEnumerable<Game>> GetGamesForUser(string username, int minRating, int maxRating)
        {
            string baseURI = "https://www.boardgamegeek.com/xmlapi2/collection?username={0}&subtype=boardgame&excludesubtype=boardgameexpansion&rated=1&stats=1&brief=1&minrating={1}&rating={2}";
            string URI = string.Format(baseURI, username, minRating, maxRating);

            try
            {
                XDocument doc = await GetXMLFrom(URI);
                return doc.FilterGames();
            }
            catch (Exception ex)
            {
                return new List<Game>();
            }
        }
    }
}
