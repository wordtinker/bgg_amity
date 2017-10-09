using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Amity.Models
{
    public struct User
    {
        public string Name { get; internal set; }
        public int Variation { get; internal set; }
        public byte Rating { get; internal set; }
        public override bool Equals(object obj)
        {
            if (!(obj is User)) return false;

            User item = (User)obj;
            return this.Name.Equals(item.Name);
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }

    internal class Pyramid
    {
        private List<User>[] levels;
        private int bottom;

        public int Count
        {
            get
            {
                return levels.Sum(l => l.Count);
            }
        }

        public Pyramid(byte variation)
        {
            variation++;
            levels = new List<User>[variation];
            for (int i = 0; i < variation; i++)
            {
                levels[i] = new List<User>();
            }
        }

        internal void Shift(List<User> users)
        {
            var newUserOnLevel = users;
            for (int i = 0; i < levels.Length; i++)
            {
                var intersection = levels[i].Intersect(newUserOnLevel);
                var left = levels[i].Except(newUserOnLevel);
                var right = newUserOnLevel.Except(levels[i]);
                levels[i] = intersection.ToList();
                if (i + 1 < levels.Length)
                {
                    levels[i + 1].AddRange(left);
                }
                if (i >= bottom)
                {
                    levels[i].AddRange(right);
                    break;
                }
                else
                {
                    newUserOnLevel = left.Union(right).ToList();
                }
            }
            bottom++;
        }

        internal List<User> ToList()
        {
            List<User> users = new List<User>();
            for (int i = 0; i < levels.Length; i++)
            {
                List<User> level = levels[i];
                // Update variation of the user
                users = users.Concat(level.Select(u => { u.Variation = i; return u; })).ToList();
            }
            return users;
        }
    }

    public static class Analyzer
    {
        private static async Task<List<User>> GetUsersOfTheGame(Game game)
        {
            var reg = RatingRegister.Instance[game.Rating];
            byte minRating = reg.Item1;
            byte maxRating = reg.Item2;
            List<User> userList = new List<User>();
            // Keep asking new pages until we reach no more users
            IBGGAPI caller = new Cache(new BGGAPI());
            for (int page = 1;; page++)
            {
                XDocument doc = await caller.GetUsersOfTheGame(game.ID, page);
                List<User> usersFromPage = doc.FilterNames();
                var filtered = from u in usersFromPage
                               where u.Rating >= minRating &&
                                     u.Rating <= maxRating
                               select u;
                byte lowerBound = usersFromPage.Select(u => u.Rating).DefaultIfEmpty().Min();
                userList.AddRange(filtered);
                // Count will be 0 on error or page is out of bound.
                if (usersFromPage.Count == 0 || lowerBound < minRating) break;
                
            }
            return userList;
        }
        
        public static async Task<List<User>> Run(IProgress<Tuple<double, string>> progress)
        {
            progress.Report(Tuple.Create(0.0, string.Empty));
            Pyramid pyramid = new Pyramid(RatingRegister.Instance.Variation);
            int i = 0;
            List<Game> games = Storage.GetGames();
            foreach (Game game in games)
            {
                pyramid.Shift(await GetUsersOfTheGame(game));
                i++;
                progress.Report(Tuple.Create((double)i / games.Count * 100, game.Name));
                // stop if there is no sense going further
                if (pyramid.Count == 0 && i > RatingRegister.Instance.Variation) break;
            }
            return pyramid.ToList();
        }
    }
}
