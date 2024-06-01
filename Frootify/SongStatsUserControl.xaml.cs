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
    /// Interaction logic for SongStatsUserControl.xaml
    /// </summary>
    public partial class SongStatsUserControl : UserControl
    {
        public SongStatsUserControl(Dictionary<string, int> songStats)
        {
            InitializeComponent();
            SongPlayListView.ItemsSource = songStats;
        }

        public void UpdateView(Dictionary<string, int> songStats)
        {
            SongPlayListView.ItemsSource = songStats;
            SongPlayListView.Items.Refresh();
        }
    }
}
