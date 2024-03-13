using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace Frootify
{
    public class SQL
    {

        private const string _connectionString = "Datasource=127.0.0.1;username=root;password=;database=Frootify;";

        #region Fetch

        public static List<Playlist> Fetch()
        {
            List<Playlist> temp = new List<Playlist>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand("SELECT * FROM playlists", connection))
                    {
                        MySqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                string description = reader.GetString(2);
                                string image = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                string songsEncoded = reader.GetString(4);

                                temp.Add(new Playlist(id, name, description, image, songsEncoded));
                            }
                        }

                        reader.Close();
                        return temp;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    return temp;
                }
            }
        }

        #endregion

        #region Create Playlist

        public static void CreatePlaylist(string name, string description, string img, string songs)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string cmd = "INSERT INTO playlists (Name, Description, Image, Songs) VALUES (@Name, @Desc, @Image, @Songs)";
                    using (MySqlCommand insertCommand = new MySqlCommand(cmd, connection))
                    {
                        insertCommand.Parameters.Add(new MySqlParameter("@Name", name));
                        insertCommand.Parameters.Add(new MySqlParameter("@Desc", description));
                        insertCommand.Parameters.Add(new MySqlParameter("@Image", img));
                        insertCommand.Parameters.Add(new MySqlParameter("@Songs", songs));
                        insertCommand.ExecuteNonQuery();
                        insertCommand.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        #endregion

        #region Update

        public static void Update(Playlist playlist)
        {
            string[] songs = playlist.Songs.Select(song => song.Directory).ToArray();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string cmd = "UPDATE playlists SET Songs = @Songs WHERE Id = @Id";
                    using (MySqlCommand insertCommand = new MySqlCommand(cmd, connection))
                    {
                        insertCommand.Parameters.Add(new MySqlParameter("@Id", playlist.Id));
                        insertCommand.Parameters.Add(new MySqlParameter("@Songs", Utils.Encode(JsonSerializer.Serialize(songs))));
                        insertCommand.ExecuteNonQuery();
                        insertCommand.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        #endregion

        #region Delete

        public static void Delete(Playlist playlist)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string cmd = "DELETE FROM playlists WHERE Id = @Id";
                    using (MySqlCommand insertCommand = new MySqlCommand(cmd, connection))
                    {
                        insertCommand.Parameters.Add(new MySqlParameter("@Id", playlist.Id));
                        insertCommand.ExecuteNonQuery();
                        insertCommand.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        #endregion

    }
}
