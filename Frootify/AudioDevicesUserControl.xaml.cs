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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frootify
{
    /// <summary>
    /// Interaction logic for AudioDevicesUserControl.xaml
    /// </summary>
    public partial class AudioDevicesUserControl : UserControl
    {
        public delegate void ApplyButtonClickedEventHandler(object sender, DevicePair selectedDevice);
        public event ApplyButtonClickedEventHandler ApplyButtonClicked;

        public AudioDevicesUserControl(List<DevicePair> devices)
        {
            InitializeComponent();
            DataContext = devices;

            DevicesComboBox.ItemsSource = devices;

            Debug.WriteLine("AudioDevicesUserControl initialized with " + devices.Count + " devices.");
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesComboBox.SelectedItem is DevicePair selectedDevice)
            {
                Debug.WriteLine("Selected device: " + selectedDevice.Name);
                ApplyButtonClicked?.Invoke(this, selectedDevice);
            }
            else
            {
                Debug.WriteLine("No device selected.");
            }
        }
    }
}
