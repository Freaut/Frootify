using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace Frootify
{
    public class JSONHelper
    {
        #region Fetch

        public static List<Playlist> Fetch()
        {
            List<Playlist> temp = new List<Playlist>();

            string? projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;

            if (projectDirectory == null)
                return temp;

            projectDirectory += "/Playlists/";

            foreach (var jsonfile in Directory.GetFiles(projectDirectory))
            {
                try
                {
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.IncludeFields = true;
                    string jsontext = File.ReadAllText(jsonfile);

                    var jsonplaylist = JsonSerializer.Deserialize<JSONPlaylist>(jsontext, options);

                    if (jsonplaylist == null)
                        continue;

                    temp.Add(
                        new Playlist(
                            jsonplaylist.Id,
                            jsonplaylist.Name,
                            jsonplaylist.Description,
                            jsonplaylist.ImgPath,
                            jsonplaylist.Songs
                        )
                    );
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            return temp;
        }

        #endregion

        #region Create Playlist

        public static void CreatePlaylist(string name, string description, string img, string songs)
        {
            string? projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            projectDirectory += "/Playlists/";

            if (projectDirectory == null)
                return;

            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            int index = Directory.GetFiles(projectDirectory).Count();
            string fileName =  $"{projectDirectory}{name}_{index}.json";

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(JsonSerializer.Serialize<JSONPlaylist>(new JSONPlaylist(index, name, description, img, songs)));
            }
        }

        #endregion

        #region Update

        // Needs debugging for sure
        public static void Update(Playlist playlist)
        {
            string[] songs = playlist.Songs.Select(song => song.Directory).ToArray();

            string? projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            projectDirectory += "/Playlists/";

            if (projectDirectory == null)
                return;

            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            string fileName = $"{projectDirectory}{playlist.Name}_{playlist.Id}.json";

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(
                    JsonSerializer.Serialize(
                        new JSONPlaylist(
                            playlist.Id,
                            playlist.Name,
                            playlist.Description,
                            playlist._imgpath,
                            Utils.Encode(System.Text.Json.JsonSerializer.Serialize(songs))
                        )
                    )
                );
            }
        }

        #endregion

        #region Delete

        public static void Delete(Playlist playlist)
        {
            string? projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            projectDirectory += "/Playlists/";

            if (projectDirectory == null)
                return;

            string fileName = $"{projectDirectory}{playlist.Name}_{playlist.Id}.json";
            try
            {
                File.Delete(fileName);
            }
            catch { }
        }

        #endregion


    }
}
