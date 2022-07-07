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
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;

namespace OutAdder
{
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        Stopwatch watch = new Stopwatch();
        OutAdder.resources.AudioFileInfo currAudioFile = new resources.AudioFileInfo();
        MediaPlayer player = new MediaPlayer();
        string currentTime = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            ImageBlock.Source = currAudioFile.GetIcon();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (watch.IsRunning)
            {
                TimeSpan ts = player.Position/*watch.Elapsed*/;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}", 
                    ts.Minutes, ts.Seconds, ts.Milliseconds/10);
                ClockTxtBlock.Text = currAudioFile.Duration + " / " + currentTime;

            }
        }

        private void PlayButton(object sender, EventArgs e)
        {
            player.Play();
            watch.Start();
            dispatcherTimer.Start();
        }
        private void PauseButton(object sender, EventArgs e)
        {
            player?.Pause();
            if (watch.IsRunning)                
                watch.Stop();
        }
        private void ResetButton(object sender, EventArgs e)
        {
            player.Stop();
            watch.Reset();
            ClockTxtBlock.Text = currAudioFile.Duration + " / 00:00:00";
        }

        private void OutLinesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddFileButton(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".mp3"; // Default file extension
            dlg.Filter = "Text documents (.mp3)|*.mp3"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            
            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                player.Open(new Uri(dlg.FileName, UriKind.Absolute));
                player.Volume = 1;
                Thread.Sleep(1000);
                currAudioFile.SetName(dlg.FileName);
                currAudioFile.SetIcon(dlg.FileName, false);
                currAudioFile.Duration = String.Format("{0:00}:{1:00}:{2:00}",
                    player.NaturalDuration.TimeSpan.Minutes,
                    player.NaturalDuration.TimeSpan.Seconds, player.NaturalDuration.TimeSpan.Milliseconds);
                SongNameText.Text = currAudioFile.GetName();
                ImageBlock.Source = currAudioFile.GetIcon();
                ResetButton(sender, e);
            }
        }

        private void MusicVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = e.NewValue/100;
        }

        private void StartEndPos_Button_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan ts;
            TimeSpan.TryParse(StartPos.Text, out ts);
            currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                    ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            ClockTxtBlock.Text = currAudioFile.Duration + " / " + currentTime;
            player.Position = ts;
        }
    }

}
