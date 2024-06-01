using Frootify.PluginSystem;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for OtherSettingsUserControl.xaml
    /// </summary>
    public partial class PluginSettings : UserControl
    {
        private IReadOnlyList<IPlugin> Plugins { get; }

        public PluginSettings(IReadOnlyList<IPlugin> plugins)
        {
            InitializeComponent();
            Plugins = plugins;
            PluginComboBox.ItemsSource = Plugins;
        }

        private void PluginComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PluginComboBox.SelectedItem is IPlugin selectedPlugin)
            {
                PluginContentControl.Content = selectedPlugin.PluginControl;
            }
        }
    }
}
