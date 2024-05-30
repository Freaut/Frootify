using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Forms;

namespace Frootify
{
    /// <summary>
    /// Interaction logic for CreatePlaylistWindow.xaml
    /// </summary>
    public partial class CreatePlaylistWindow : Window
    {
        private const string _default_img = "https://res.cloudinary.com/dcogdkkwa/image/upload/v1700703597/xorhko3yu3hs54eljeg9.jpg";
        private string? _imagePath;
        private string[]? _selectedSongs;
        private List<string>? _selectedSongFileNames { get; set; }
        public static event EventHandler<Playlist>? PlaylistAddedEvent;

        public CreatePlaylistWindow()
        {
            InitializeComponent();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*",
                Title = "Select Image Files",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string imagePath = openFileDialog.FileName;
                _imagePath = imagePath;
            }
        }

        private void AddSongs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.wav;*.ogg|All Files|*.*",
                Title = "Select Audio Files",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] selectedFiles = openFileDialog.FileNames;
                _selectedSongs = selectedFiles;
                _selectedSongFileNames = new List<string>();

                foreach (string filePath in selectedFiles)
                {
                    _selectedSongFileNames.Add(System.IO.Path.GetFileName(filePath));
                }
            }

            SelectedSongsListBox.ItemsSource = _selectedSongFileNames;
        }

        private void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            string url = string.Empty;

            if (!string.IsNullOrEmpty(_imagePath))
            {
                (bool result, url) = Utils.UploadImage(_imagePath);
                if (!result)
                {
                    Debug.WriteLine("Failed to upload image");
                }
            }

            string songs = _selectedSongs == null ? string.Empty : Utils.Encode(JsonSerializer.Serialize(_selectedSongs));
            string name = PlaylistNameTextBox.Text;
            string desc = PlaylistDescriptionTextBox.Text;
            string finalimg = string.IsNullOrEmpty(url) ? _default_img : url;

            if (MainWindow.USEJSON)
            {
                JSONHelper.CreatePlaylist(name, desc, finalimg, songs);
            }
            else
            {
                try
                {
                    SQL.CreatePlaylist(name, desc, finalimg, songs);
                }
                catch
                {
                    JSONHelper.CreatePlaylist(name, desc, finalimg, songs);
                }
            }

            PlaylistAddedEvent?.Invoke(this, new Playlist(0, name, desc, finalimg, songs));
            this.Close();
        }
    }
}
