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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frootify
{
    /// <summary>
    /// Interaction logic for WPFNotification.xaml
    /// </summary>
    public partial class WPFNotification : Window
    {
        private MainWindow instance { get; set; }
        public WPFNotification(Notification notification, MainWindow main)
        {
            InitializeComponent();

            instance = main;
            PlayPauseImage.Source = MainWindow._biPause;
            PreviousImage.Source = Utils.ConvertBitmapToBitmapImage(MainWindow._previousimg);
            SkipImage.Source = Utils.ConvertBitmapToBitmapImage(MainWindow._skipimg);

            DataContext = notification;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PositionWindowBottomRight();
            StartFadeOutAnimation();
        }

        private void PositionWindowBottomRight()
        {
            var workingArea = SystemParameters.WorkArea;
            this.Left = workingArea.Right - this.Width - 10; // 10 is for margin from the edge
            this.Top = 10; // 10 is for margin from the edge
        }

        private void StartFadeOutAnimation()
        {
            var storyboard = new Storyboard();
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                BeginTime = TimeSpan.FromSeconds(5)
            };
            fadeOutAnimation.Completed += FadeOutCompleted;

            Storyboard.SetTarget(fadeOutAnimation, this);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(Window.OpacityProperty));

            storyboard.Children.Add(fadeOutAnimation);
            storyboard.Begin();
        }

        private void FadeOutCompleted(object sender, EventArgs e)
        {
            Close();
        }

        private void SkipBackward_Click(object sender, RoutedEventArgs e)
        {
            instance.SkipBackward();
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            instance.PlayPause();
            PlayPauseImage.Source = instance._audioPlayer.isPlaying ? MainWindow._biPause : MainWindow._biPlay;
        }

        private void SkipForward_Click(object sender, RoutedEventArgs e)
        {
            instance.SkipForward();
        }
    }
}
