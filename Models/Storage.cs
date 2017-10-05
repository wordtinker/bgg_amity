using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Amity.Models
{
    public static class Storage
    {
        private const string dbFile = "file.db";
        private static string connString;

        public static void Configure(string directory)
        {
            string dbFileName = Path.Combine(directory, dbFile);
            connString = string.Format("Data Source={0};Version=3;foreign keys=True;", dbFileName);
        }

        /// <summary>
        /// Creates DB file with tables if it does not exist.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool CreateFile(string directory)
        {
            string dbFileName = Path.Combine(directory, dbFile);
            if (File.Exists(dbFileName))
            {
                return true;
            }
            else
            {
                try
                {
                    SQLiteConnection.CreateFile(dbFileName);
                    InitializeTables();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private static void InitializeTables()
        {
            string sql = "CREATE TABLE IF NOT EXISTS Games(id TEXT PRIMARY KEY, name TEXT, rating REAL)";
            ExecuteNonQuery(sql);
        }
        private static void ExecuteNonQuery(string sql)
        {
            SQLiteConnection dbConn = new SQLiteConnection(connString);
            dbConn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConn))
            {
                cmd.ExecuteNonQuery();
            }
            dbConn.Close();
        }
        public static void DeleteAllGames()
        {
            string sql = "DELETE FROM Games";
            ExecuteNonQuery(sql);
        }
        public static void AddGame(Game game)
        {
            string sql = "INSERT INTO Games VALUES(@id, @name, @rating)";
            SQLiteConnection dbConn = new SQLiteConnection(connString);
            dbConn.Open();
            using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConn))
            {
                SQLiteParameter param = new SQLiteParameter("@id");
                param.Value = game.ID;
                param.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(param);

                param = new SQLiteParameter("@name");
                param.Value = game.Name;
                param.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(param);

                param = new SQLiteParameter("@rating");
                param.Value = game.Rating;
                param.DbType = System.Data.DbType.Double;
                cmd.Parameters.Add(param);

                cmd.ExecuteNonQuery();
            }
            dbConn.Close();
        }

        public static List<Game> GetGames()
        {
            List<Game> games = new List<Game>();

            string sql = "SELECT id, name, rating FROM Games";
            SQLiteConnection dbConn = new SQLiteConnection(connString);
            dbConn.Open();

            using (SQLiteCommand cmd = new SQLiteCommand(sql, dbConn))
            {
                SQLiteDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    games.Add(new Game { ID = dr.GetString(0), Name = dr.GetString(1), Rating = dr.GetDouble(2) });
                }
                dr.Close();
            }
            return games;
        }
    }
}
