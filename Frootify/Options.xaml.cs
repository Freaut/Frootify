using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frootify
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public List<DevicePair> Devices { get; set; }
        private AudioPlayer audioPlayer { get; set; }
        private MainWindow instance { get; set; }

        public Options(AudioPlayer audioPlayer, MainWindow instance)
        {
            Devices = audioPlayer.GetDevices();
            Debug.WriteLine("Devices count: " + Devices.Count);
            this.audioPlayer = audioPlayer;
            this.instance = instance;

            InitializeComponent();
            DataContext = this;

            Debug.WriteLine("Initialized component and set DataContext.");

            if (MainContentControl != null)
            {
                var audioDevicesUserControl = new AudioDevicesUserControl(Devices);
                audioDevicesUserControl.ApplyButtonClicked += ChangeOutputDevice_Clicked;

                MainContentControl.Content = audioDevicesUserControl;
                Debug.WriteLine("Set initial content to AudioDevicesUserControl.");
            }
            else
            {
                Debug.WriteLine("MainContentControl is null during initialization.");
            }

            this.instance = instance;
        }

        private void ChangeOutputDevice_Clicked(object sender, DevicePair selectedDevice)
        {
            Debug.WriteLine("Apply button clicked. Selected device: " + selectedDevice.Name);

            instance.ChangeOutputDevice(selectedDevice);
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainContentControl == null)
            {
                Debug.WriteLine("MainContentControl is null in SelectionChanged event.");
                return;
            }

            if (MenuListBox.SelectedItem is ListBoxItem selectedItem)
            {
                Debug.WriteLine("Selected item: " + selectedItem.Content.ToString());
                switch (selectedItem.Content.ToString())
                {
                    case "Audio Devices":
                        var audioDevicesUserControl = new AudioDevicesUserControl(Devices);
                        audioDevicesUserControl.ApplyButtonClicked += ChangeOutputDevice_Clicked;
                        MainContentControl.Content = audioDevicesUserControl;

                        Debug.WriteLine("Switched content to AudioDevicesUserControl.");
                        break;
                    case "Other Settings":
                        MainContentControl.Content = new OtherSettingsUserControl();
                        Debug.WriteLine("Switched content to OtherSettingsUserControl.");
                        break;
                        // Add more cases for other tabs here
                }
            }
            else
            {
                Debug.WriteLine("Selected item is null.");
            }
        }

    }
}
