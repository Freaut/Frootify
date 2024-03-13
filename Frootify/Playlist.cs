using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Frootify
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BitmapImage Image { get; set; }
        public List<Song> Songs { get; set; }

        public Playlist(int _id, string _name, string _description, string _imagepath, string? songs_encoded = "")
        {
            Id = _id;
            Name = _name;
            Description = _description;
            Image = new BitmapImage(new Uri(_imagepath, UriKind.RelativeOrAbsolute));

            List<Song> songs = new List<Song>();
            if (!string.IsNullOrEmpty(songs_encoded))
            {
                string values = Utils.Decode(songs_encoded);
                songs.AddRange(JsonSerializer.Deserialize<string[]>(values).Select(song => new Song(song)));
            }

            Songs = songs;
        }
    }
}
