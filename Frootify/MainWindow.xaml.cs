﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using System.Linq;
using MySql.Data.MySqlClient;
using Frootify.PluginSystem;

namespace Frootify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        // Add equalizer (smart equalizing?)
        // Modify bass
        // Change sound quality
        // Option to favorite songs, as well as a favorites playlist
        // Song stats - Times played, most played songs, etc

        #region Enums

        public enum RepeatState
        {
            Playlist,
            Song
        }

        public enum SongSelectionState
        {
            Next,
            Shuffle
        }

        #endregion

        #region Variables

        public static bool USEJSON = true;
        
        private const string _connectionString = "Datasource=127.0.0.1;username=root;password=;database=Frootify;";

        public AudioPlayer _audioPlayer;
        private float _volume;
        private Thread _updateThread;
        private MouseButton _lastClickedButton_Playlists;
        private MouseButton _lastClickedButton_Song;

        public static System.Drawing.Bitmap _previousimg  = Properties.Resources.previous;
        public static System.Drawing.Bitmap _skipimg      = Properties.Resources.skip;
        
        public static BitmapImage _biPlay                 = Utils.ConvertBitmapToBitmapImage(Properties.Resources.play);
        public static BitmapImage _biPause                = Utils.ConvertBitmapToBitmapImage(Properties.Resources.pause);
        public static BitmapImage _shuffleImage           = Utils.ConvertBitmapToBitmapImage(Properties.Resources.shuffle);
        public static BitmapImage _shuffleImage_selected  = Utils.ConvertBitmapToBitmapImage(Properties.Resources.shuffle_selected);
        public static BitmapImage _repeatImage            = Utils.ConvertBitmapToBitmapImage(Properties.Resources.repeat);
        public static BitmapImage _repeatImage_selected   = Utils.ConvertBitmapToBitmapImage(Properties.Resources.repeat_selected);
        
        private Playlist? SelectedPlaylist { get; set; }
        private Song? SelectedSong { get; set; }
        private RepeatState _repeatMode { get; set; } = RepeatState.Playlist;
        private SongSelectionState _songSelectionState { get; set; } = SongSelectionState.Next;

        private List<Playlist> _playlists           = new List<Playlist>();
        private List<Song> _queue                   = new List<Song>();
        private TimeSpan _updateInterval            = new TimeSpan(0, 0, 1);
        private TimeSpan _songTime { get; set; }    = TimeSpan.Zero;
        private Random _random                      = new Random();
        private bool _isChangedByProgram            = false;
        private ListBoxItem? clickedListBoxItem;
        private ListBoxItem? clickedSongItem;
        private Playlist? _clickedPlaylist;
        private Song? _clickedSong;

        private EventAggregator? EventAggregator { get; set; }
        private Thread _eventThread;
        public PluginLoader pluginLoader { get; set; }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            #region Playlist Listbox Contextmenu

            ContextMenu contextMenu = new ContextMenu();
            MenuItem addSongsMenuItem = new MenuItem { Header = "Add Songs" };
            MenuItem deletePlaylistMenuItem = new MenuItem { Header = "Delete Playlist" };

            MenuItem settingsMenu = new MenuItem { Header = "Settings" };

            addSongsMenuItem.Click += MenuItem_AddSongs_Click;
            deletePlaylistMenuItem.Click += MenuItem_DeletePlaylist_Click;
            settingsMenu.Click += OpenOptions_Click;

            contextMenu.Items.Add(addSongsMenuItem);
            contextMenu.Items.Add(deletePlaylistMenuItem);
            contextMenu.Items.Add(settingsMenu);
            PlaylistListBox.ContextMenu = contextMenu;
            PlaylistListBox.PreviewMouseRightButtonDown += PlaylistListBox_PreviewMouseRightButtonDown;

            #endregion

            #region Songs Listbox Contextmenu

            ContextMenu songs_contextMenu = new ContextMenu();
            MenuItem queueSongMenuItem = new MenuItem { Header = "Add To Queue" };
            MenuItem removeSongMenuItem = new MenuItem { Header = "Remove From Playlist" };

            queueSongMenuItem.Click += MenuItem_QueueSong_Click;
            removeSongMenuItem.Click += MenuItem_RemoveSong_Click;

            songs_contextMenu.Items.Add(queueSongMenuItem);
            songs_contextMenu.Items.Add(removeSongMenuItem);
            //songs_contextMenu.Items.Add(changeOutputDevice);
            SongsListBox.ContextMenu = songs_contextMenu;
            SongsListBox.PreviewMouseRightButtonDown += SongsListBox_PreviewMouseRightButtonDown;

            #endregion

            #region Load Volume Settings From Prev Session

            _volume = Properties.Settings.Default.Volume;

            if (VolumeText != null)
                VolumeText.Text = _volume < 10 ? $"0{_volume}%" : $"{_volume}%";
            VolumeSlider.Value = _volume;

            #endregion

            #region Icon Setup

            PlayPauseImage.Source   = _biPlay;
            RepeatImage.Source      = _repeatImage;
            PreviousImage.Source    = Utils.ConvertBitmapToBitmapImage(_previousimg);
            SkipImage.Source        = Utils.ConvertBitmapToBitmapImage(_skipimg);
            ShuffleImage.Source     = _shuffleImage;

            #endregion

            #region Audioplayer Setup

            _audioPlayer = new AudioPlayer(Convert.ToInt32(VolumeSlider.Value) / 100.0f);
            AudioPlayer.SongFinishedEvent += AudioPlayer_SongFinishedEvent;
            CreatePlaylistWindow.PlaylistAddedEvent += CreatePlaylist_PlaylistAddedEvent;

            #endregion

            ProgressSlider.Value = 0;

            #region Update Thread

            _updateThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Update();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    Thread.Sleep(_updateInterval);
                }
            });
            _updateThread.Start();

            #endregion

            #region Attempting SQL Connection

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    USEJSON = false;
                }
            }
            catch
            {
                USEJSON = true;

            }

            #endregion

            LoadPlaylists();

            #region Plugin Thread

            _eventThread = new Thread(() =>
            {
                EventAggregator = new EventAggregator();
                pluginLoader = new PluginLoader(EventAggregator);

                string? pluginDir = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName + "/Plugins/";

                if (!Directory.Exists(pluginDir))
                    Directory.CreateDirectory(pluginDir);

                //ExamplePlugin examplePlugin = new ExamplePlugin();
                //pluginLoader._pluginManager.LoadPlugin(examplePlugin);

                //pluginLoader._pluginManager.LoadPlugins(pluginDir);

                var inMemoryPlugins = new List<IPlugin>
                {
                    new ExamplePlugin()
                };

                pluginLoader._pluginManager.DiscoverPlugins(
                    clearExisting: true,
                    inMemoryPlugins: inMemoryPlugins,
                    pluginDir: pluginDir
                );

                // pluginLoader._pluginManager.DiscoverPlugins();

                pluginLoader.Start(EventAggregator);

            });
            _eventThread.Start();

            #endregion
        }

        #region Notifications

        private void ShowNotification(Notification notification)
        {
            new Thread(() =>
            {
                EventAggregator?.Publish(new NotificationPopupEvent());
            }).Start();

            Application.Current.Dispatcher.Invoke(() =>
            {
                WPFNotification notificationWindow = new WPFNotification(notification, this);
                notificationWindow.Owner = this;
                notificationWindow.Show();
            });
        }

        #endregion

        #region Update Loop

        private void Update()
        {
            if (_audioPlayer.isPlaying)
            {
                _songTime = _songTime.Add(_updateInterval);

                if (SelectedSong == null)
                    return;

                if (_songTime > SelectedSong.Duration)
                {
                    SelectNextSong();
                }

                double percentageElapsed = (_songTime.TotalSeconds / SelectedSong.Duration.TotalSeconds) * 100;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _isChangedByProgram = true;
                    ProgressSlider.Value = percentageElapsed;
                    _isChangedByProgram = false;
                    SongTimeText.Text = _songTime.ToString(@"hh\:mm\:ss");
                    SongTotalTimeText.Text = SelectedSong.Duration.ToString(@"hh\:mm\:ss");
                    CurrentSong.Text = SelectedSong.Title;
                });
            }
            
            // Budget fix deluxe
            Application.Current.Dispatcher.Invoke(() =>
            {
                PlayPauseImage.Source = _audioPlayer.isPlaying ? _biPause : _biPlay;
            });
        }

        #endregion

        #region Listbox Helpers

        private T? FindAncestor<T>(DependencyObject current)
        where T : DependencyObject
        {
            do
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);

            return null;
        }

        #region Previews

        private void PlaylistListBox_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            clickedListBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

            if (clickedListBoxItem != null)
            {
                // Debug.WriteLine(clickedListBoxItem.DataContext);
                Playlist? clickedPlaylist = _playlists.FirstOrDefault(p => p.Equals(clickedListBoxItem.DataContext));
                if (clickedPlaylist != null)
                {
                    _clickedPlaylist = clickedPlaylist;
                    PlaylistListBox.ContextMenu.IsOpen = true;
                }
            }
        }

        private void SongsListBox_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedPlaylist == null)
                return;

            clickedSongItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);

            if (clickedSongItem != null)
            {
                // Debug.WriteLine(clickedSongItem.DataContext);
                Song? clickedSong = SelectedPlaylist.Songs.FirstOrDefault(p => p.Equals(clickedSongItem.DataContext));
                if (clickedSong != null)
                {
                    _clickedSong = clickedSong;
                    SongsListBox.ContextMenu.IsOpen = true;
                }
            }
        }

        private void ListBoxSongItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastClickedButton_Song = MouseButton.Left;
        }

        private void ListBoxSongItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastClickedButton_Song = MouseButton.Right;
        }

        #endregion

        #endregion

        #region Output Device

        public void ChangeOutputDevice(DevicePair selectedDevice)
        {
            _audioPlayer.Terminate();

            _audioPlayer = new AudioPlayer(_volume, selectedDevice.Index);
            _audioPlayer.SetVolume(_volume / 100.0f);

            if (SelectedSong != null || _audioPlayer.isPlaying)
            {
                _audioPlayer.Pause();
                PlaySelectedSong(SelectedSong.Directory, true);
                _audioPlayer.Skip(_songTime.TotalSeconds);
                _audioPlayer.Resume();
            }
        }

        private void OpenOptions_Click(object sender, RoutedEventArgs e)
        {
            Options options = new Options(_audioPlayer, this);
            options.Owner = this;
            options.Show();
        }

        #endregion

        #region Playlist

        #region Add Songs To Playlist

        private void MenuItem_AddSongs_Click(object sender, RoutedEventArgs e)
        {
            if (_clickedPlaylist == null)
                return;

            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.wav;*.ogg|All Files|*.*",
                Title = "Select Audio Files",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] selectedFiles = openFileDialog.FileNames;

                foreach (string filePath in selectedFiles)
                    _clickedPlaylist.Songs.Add(new Song(filePath));

                if (USEJSON)
                {
                    JSONHelper.Update(_clickedPlaylist);
                }
                else
                {
                    try
                    {
                        SQL.Update(_clickedPlaylist);
                    }
                    catch
                    {
                        JSONHelper.Update(_clickedPlaylist);
                    }
                }

                LoadPlaylists();
                SelectedPlaylist = _playlists.Find(x => x.Id == SelectedPlaylist?.Id);
                UpdateSongsListBox();
            }
        }

        #endregion

        #region Deleting Playlist

        private void MenuItem_DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (_clickedPlaylist == null)
                return;

            if (USEJSON)
            {
                JSONHelper.Delete(_clickedPlaylist);
            }
            else
            {
                try
                {
                    SQL.Delete(_clickedPlaylist);
                }
                catch
                {
                    JSONHelper.Delete(_clickedPlaylist);
                }
            }

            _clickedPlaylist = null;
            LoadPlaylists();

            if (SelectedPlaylist == null)
                return;
            SelectedPlaylist = _playlists.Find(x => x.Id == SelectedPlaylist.Id);
            UpdateSongsListBox();
        }

        #endregion

        #region Remove Song From Playlist

        private void MenuItem_RemoveSong_Click(object sender, RoutedEventArgs e)
        {
            if (_clickedSong == null)
                return;

            if (SelectedPlaylist == null)
                return;

            SelectedPlaylist.Songs.Remove(_clickedSong);
            _clickedSong = null;

            try
            {
                SQL.Update(SelectedPlaylist);
            }
            catch
            {
                JSONHelper.Update(SelectedPlaylist);
            }

            LoadPlaylists();
        }

        #endregion

        #region Playlist Added Event

        private void CreatePlaylist_PlaylistAddedEvent(object? sender, Playlist e)
        {
            LoadPlaylists();
        }

        #endregion

        #region Playlist Previews

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastClickedButton_Playlists = MouseButton.Left;
        }

        private void ListBoxItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastClickedButton_Playlists = MouseButton.Right;
        }

        #endregion

        #region Load Playlists

        private void LoadPlaylists()
        {
            if (USEJSON)
            {
                _playlists = JSONHelper.Fetch();
            }
            else
            {
                try
                {
                    _playlists = SQL.Fetch();
                }
                catch
                {
                    _playlists = JSONHelper.Fetch();
                }
            }

            PlaylistListBox.ItemsSource = _playlists;
        }

        #endregion

        #region Selection Changed Event

        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lastClickedButton_Playlists != MouseButton.Left)
                return;

            Playlist? selectedPlaylist = PlaylistListBox.SelectedItem as Playlist;

            if (selectedPlaylist == null)
                return;

            SelectedPlaylist = selectedPlaylist;
            PlaylistName.Text = selectedPlaylist.Name;

            UpdateSongsListBox();
        }

        #endregion

        #region Create Playlist

        private void CreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            CreatePlaylistWindow createPlaylistWindow = new CreatePlaylistWindow();
            createPlaylistWindow.ShowDialog();
        }

        #endregion

        #endregion

        #region Song related

        #region Queue Song

        private void MenuItem_QueueSong_Click(object sender, RoutedEventArgs e)
        {
            if (_clickedSong == null)
                return;

            _queue.Add(_clickedSong);
            _clickedSong = null;
        }

        #endregion

        #region Song Finished & Selecting Next Songs

        private void AudioPlayer_SongFinishedEvent(object? sender, string e)
        {
            try
            {
                // Debug.WriteLine("SongFinishedEvent invoked");
                SelectNextSong();
            }
            catch { }
        }

        private void SelectNextSong()
        {
            // Repeat has priority
            if (_repeatMode == RepeatState.Song)
            {
                if (SelectedSong != null)
                {
                    RestartSong();
                    return;
                }
            }

            // Queue second priority
            if (_queue.Count > 0)
            {
                if (SelectedSong == _queue.First())
                {
                    RestartSong();
                    _queue.RemoveAt(0);
                    return;
                }
                else
                {
                    SelectedSong = _queue.FirstOrDefault();
                    if (SelectedSong == null)
                        return;

                    PlaySelectedSong(SelectedSong.Directory, true);
                    _queue.RemoveAt(0);
                    return;
                }
            }

            if (SelectedPlaylist == null)
                return;

            // Otherwise select next
            if (SelectedSong != null)
            {
                if (_songSelectionState == SongSelectionState.Next)
                {
                    int index = SelectedPlaylist.Songs.IndexOf(SelectedSong);
                    int nextIndex = (index + 1) % SelectedPlaylist.Songs.Count;

                    SelectedSong = SelectedPlaylist.Songs[nextIndex];
                    PlaySelectedSong(SelectedSong.Directory, true);
                    return;
                }
            }

            // Shuffle
            SelectedSong = SelectedPlaylist.Songs[_random.Next(SelectedPlaylist.Songs.Count)];
            PlaySelectedSong(SelectedSong.Directory, true);
        }

        #endregion

        #region Shuffle & Repeat

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            bool next = _songSelectionState == SongSelectionState.Next;
            _songSelectionState = next ? SongSelectionState.Shuffle : SongSelectionState.Next;

            ShuffleImage.Source = !next ? _shuffleImage : _shuffleImage_selected;
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            bool playlist = _repeatMode == RepeatState.Playlist;
            _repeatMode = playlist ? RepeatState.Song : RepeatState.Playlist;

            RepeatImage.Source = !playlist ? _repeatImage : _repeatImage_selected;
        }

        #endregion

        #region Restarting Song

        private void RestartSong()
        {
            new Thread(() =>
            {
                EventAggregator?.Publish(new SongChangedEvent(SelectedSong));
            }).Start();

            _songTime = TimeSpan.FromSeconds(0);
            _audioPlayer.Skip(0);

            if (SelectedSong == null)
                return;

            ShowNotification(new Notification
            {
                Title = "Now Playing",
                Message = $"{SelectedSong.Title} - {SelectedSong.Artist}",
                Image = _playlists.First()._imgpath
            });
        }

        #endregion

        #region Skipping songs

        public void SkipForward()
        {
            try
            {
                SelectNextSong();
            }
            catch { }
        }

        public void SkipBackward()
        {
            try
            {
                if (_songTime.TotalSeconds > 10)
                {
                    RestartSong();
                    return;
                }

                if (SelectedSong == null)
                    return;

                if (SelectedPlaylist == null)
                    return;

                int index = SelectedPlaylist.Songs.IndexOf(SelectedSong);
                int previousIndex = (index - 1 + SelectedPlaylist.Songs.Count) % SelectedPlaylist.Songs.Count;

                SelectedSong = SelectedPlaylist.Songs[previousIndex];
                PlaySelectedSong(SelectedSong.Directory, true);
            }
            catch { }
        }

        private void SkipForward_Click(object sender, RoutedEventArgs e)
        {
            SkipForward();
        }

        private void SkipBackward_Click(object sender, RoutedEventArgs e)
        {
            SkipBackward();
        }

        #endregion

        #region PlayPause

        public void PlayPause()
        {
            if (SelectedSong != null)
            {
                try
                {
                    if (_audioPlayer.isPlaying)
                    {
                        // Debug.WriteLine("Pause");
                        _audioPlayer.Pause();
                    }
                    else if (_audioPlayer.isPaused)
                    {
                        // Debug.WriteLine("Resume");
                        _audioPlayer.Resume();
                    }
                    else // Should really never be happening
                    {
                        // Debug.WriteLine("Else");
                        _audioPlayer.Play(SelectedSong.Directory);
                        _songTime = TimeSpan.Zero;
                    }
                }
                catch { }
            }

        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            PlayPause();
        }

        #endregion

        #region Play Selected Song

        public void PlaySelectedSong(string filePath, bool click = false)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            if (click)
            {
                // Debug.WriteLine("Click");
                _audioPlayer.Play(filePath);
                _songTime = TimeSpan.Zero;

                new Thread(() =>
                {
                    EventAggregator?.Publish(new SongChangedEvent(SelectedSong));
                }).Start();

                if (SelectedSong != null)
                {
                    ShowNotification(new Notification
                    {
                        Title = "Now Playing",
                        Message = $"{SelectedSong.Title} - {SelectedSong.Artist}",
                        Image = _playlists.First()._imgpath
                    });
                }

                return;
            }
        }

        #endregion

        #region Song Time Event

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isChangedByProgram)
                return;

            if (_audioPlayer != null && SelectedSong != null)
            {
                int timeStamp = Convert.ToInt32(ProgressSlider.Value);
                double newPosition = (timeStamp / 100.0) * SelectedSong.Duration.TotalSeconds;
                _songTime = TimeSpan.FromSeconds(newPosition);
                _audioPlayer.Skip(newPosition);
            }
        }

        #endregion

        #region Song Selection Changed Event

        private void Song_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lastClickedButton_Song != MouseButton.Left)
                return;

            Song? selectedSong = SongsListBox.SelectedItem as Song;
            if (selectedSong != null)
            {
                SelectedSong = selectedSong;
                PlaySelectedSong(selectedSong.Directory, true);
                CurrentSong.Text = SelectedSong.Title;
            }
            else if (SelectedSong != null)
            {
                PlaySelectedSong(SelectedSong.Directory, true);
            }
        }

        #endregion

        #region Update Songs Listbox

        private void UpdateSongsListBox()
        {
            if (SelectedPlaylist != null)
            {
                TimeSpan totalTime = TimeSpan.Zero;

                foreach (var song in SelectedPlaylist.Songs)
                    totalTime = totalTime.Add(song.Duration);

                AmountOfTracks.Text = $"{SelectedPlaylist.Songs.Count.ToString()} Songs - {totalTime.ToString(@"hh\:mm\:ss")}";

                SongsListBox.ItemsSource = SelectedPlaylist.Songs;
            }
        }

        #endregion

        #region Changing Volume

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int volumeValue = Convert.ToInt32(VolumeSlider.Value);
            _volume = volumeValue;

            if (_audioPlayer != null)
                _audioPlayer.SetVolume(_volume / 100.0f);
            if (VolumeText != null)
                VolumeText.Text = volumeValue < 10 ? $"0{volumeValue}%" : $"{volumeValue}%";
        }

        #endregion

        #endregion

        #region Closebutton

        private void CloseButton_Click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Volume = _volume;
            Properties.Settings.Default.Save();
            _audioPlayer.Stop();
            _audioPlayer.Terminate();
            Application.Current.Shutdown();
        }

        #endregion
    }
}