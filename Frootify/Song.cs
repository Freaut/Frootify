using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using NAudio.Wave;

namespace Frootify
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Directory { get; set; }
        public TimeSpan Duration { get; set; }
        public BitmapImage Image { get; set; }

        public string GetSongTitle(string _path)
        {
            string title = string.Empty;
            try
            {
                var file = TagLib.File.Create(_path);
                title = file.Tag.Title; // Attempt to get title through metadata
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (string.IsNullOrEmpty(title))
                title = System.IO.Path.GetFileNameWithoutExtension(_path);
            
            return title;
        }

        public string GetSongArtist(string filePath)
        {
            string artist = string.Empty;

            try
            {
                var file = TagLib.File.Create(filePath);
                artist = file.Tag.FirstPerformer; // Attempt to get artist through metadata
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (string.IsNullOrEmpty(artist))
                artist = "Unknown";

            return artist;
        }

        public Song(string _directory)
        {
            Title = GetSongTitle(_directory);
            Artist = GetSongArtist(_directory);
            Directory = _directory;

            try
            {
                using (Mp3FileReader mp3 = new Mp3FileReader(_directory))
                {
                    TimeSpan originalDuration = mp3.TotalTime;
                    string formattedDuration = originalDuration.ToString(@"hh\:mm\:ss");
                    Duration = TimeSpan.Parse(formattedDuration);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                Duration = TimeSpan.Zero;
            }
            Image = new BitmapImage(new Uri("https://media.discordapp.net/attachments/1130259258705969152/1172333204431196181/93457970588ac31c0d4e06f41469f003.jpg", UriKind.RelativeOrAbsolute));
        }
    }
}
