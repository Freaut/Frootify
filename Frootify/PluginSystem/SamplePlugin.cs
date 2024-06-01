using Org.BouncyCastle.Crypto.Operators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static System.Windows.Forms.Design.AxImporter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Frootify.PluginSystem
{
    /// <summary>
    /// Example plugin to keep track of song stats
    /// </summary>

    [Plugin]
    public class ExamplePlugin : PluginBase
    {
        public override string Name => "Song Stats Plugin";
        public override UserControl? PluginControl { get; set; }
        //private Dictionary<string, int>? songStats { get; set; }
        private ObservableCollection<KeyValuePair<string, int>> songStats { get; set; }


        public override void Execute()
        {
            string? projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName + "/SongStatsPlugin/";
            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            string jsonfile = projectDirectory + "statdata.json";

            if (File.Exists(jsonfile))
            {
                string jsontext = File.ReadAllText(jsonfile);
                if (!String.IsNullOrEmpty(jsontext))
                {
                    var jsonStats = JsonSerializer.Deserialize<ObservableCollection<KeyValuePair<string, int>>>(jsontext);
                    songStats = jsonStats;
                }
            }
            else
            {
                File.Create(jsonfile);
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                PluginControl = new SongStatsUserControl(songStats);
            });

            Debug.WriteLine($"[{Name}] Plugin is executing.");
        }

        private void WriteStats()
        {
            string? projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName + "/SongStatsPlugin/";
            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            string jsonfile = projectDirectory + "statdata.json";
            if (!File.Exists(jsonfile))
                File.Create(jsonfile);

            using (StreamWriter sw = new StreamWriter(jsonfile))
            {
                sw.Write(
                    JsonSerializer.Serialize(songStats)
                );
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_eventAggregator == null)
                return;

            songStats = new ObservableCollection<KeyValuePair<string, int>>();

            _eventAggregator.Subscribe<ApplicationShutdownEvent>(HandleShutdownEvent);
            _eventAggregator.Subscribe<SongChangedEvent>(HandleGeneralEvent);
        }

        private void HandleGeneralEvent<T>(T _event) where T : IEvent
        {
            Debug.WriteLine($"{Name} has recieved {_event}");
            
            if (_event is SongChangedEvent eventparams)
            {
                string songTitle = eventparams.CurrentSong.Title;
                Debug.WriteLine($"SongChangedEvent detected, current song is {eventparams.CurrentSong.Title}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var existingEntry = songStats.FirstOrDefault(x => x.Key == songTitle);
                    if (existingEntry.Key != null)
                    {
                        songStats.Remove(existingEntry);
                        songStats.Add(new KeyValuePair<string, int>(songTitle, existingEntry.Value + 1));
                    }
                    else
                    {
                        songStats.Add(new KeyValuePair<string, int>(songTitle, 1));
                    }

                    (PluginControl as SongStatsUserControl)?.UpdateView(songStats);

                });

                WriteStats();
            }
        }

        private void HandleShutdownEvent(ApplicationShutdownEvent shutdownEvent)
        {
            Debug.WriteLine($"{Name} received application shutdown event.");
            WriteStats();
        }
    }
}
