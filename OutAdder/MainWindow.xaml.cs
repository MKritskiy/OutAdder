using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using System.Text.RegularExpressions;
using NAudio.Wave;
using OutAdder.resources;
using Microsoft.Win32;

namespace OutAdder
{
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        AudioFileInfo ?currAudioFileInfo;
        AudioPlayer audioPlayer = new AudioPlayer();

        TextBoxTemplate startTxtTemplate;
        TextBoxTemplate endTxtTemplate;

        //Recorder
        WaveInEvent recorder = new WaveInEvent();
        BufferedWaveProvider bufferedWaveProvider;
        WaveOutEvent playerMic = new WaveOutEvent();

        public MainWindow()
        {
            InitializeComponent();

            recorder.WaveFormat = new WaveFormat(44100, 16, 1);
            // set up our signal chain
            bufferedWaveProvider = new BufferedWaveProvider(recorder.WaveFormat);

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);

            startTxtTemplate = new TextBoxTemplate(StartPos.Text);
            endTxtTemplate = new TextBoxTemplate(EndPos.Text);

            //Output devices
            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                OutDevicesList.Items.Add(caps.ProductName);
            }
            //Input devices
            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                var caps = WaveIn.GetCapabilities(n);
                InDevicesList.Items.Add(caps.ProductName);
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (currAudioFileInfo != null)
            {
                if (audioPlayer.IsPlaying)
                {
                    TimeSpan ts = currAudioFileInfo.CurrentTime;
                    audioPlayer.CurrentTime = String.Format("{0:00}:{1:00}"/*:{2:00}"*/,
                        ts.Minutes, ts.Seconds/*, ts.Milliseconds/10*/);
                    ClockTxtBlock.Text = currAudioFileInfo.Duration + " / " + audioPlayer.CurrentTime;
                }
                if (!audioPlayer.IsEndingTimeRealised && (int)audioPlayer.EndingTime.TotalSeconds <= (int)currAudioFileInfo.CurrentTime.TotalSeconds)
                {
                    PauseButton(sender, e);
                    audioPlayer.IsEndingTimeRealised = true;
                }
            }
        }

        private void RecorderOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {
            bufferedWaveProvider.AddSamples(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
        }

        private void PlayButton(object sender, EventArgs e)
        {
            if (currAudioFileInfo != null && !audioPlayer.IsPlaying)
            {
                dispatcherTimer.Start();
                audioPlayer.Play(); 
            }
        }
        private void PauseButton(object sender, EventArgs e)
        {
            if (currAudioFileInfo != null)
            {
                audioPlayer.Pause();
                dispatcherTimer.Stop();
            }
        }
        private void ResetButton(object sender, EventArgs e)
        {
            if (currAudioFileInfo != null)
            {
                currAudioFileInfo.Position = 0;

                audioPlayer.Stop();

                ClockTxtBlock.Text = currAudioFileInfo.Duration + " / 00:00";

                dispatcherTimer.Stop();
            }
        }

        private void AddFileButton(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".mp3"; // Default file extension
            dlg.Filter = "Text documents (.mp3)|*.mp3"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document                
                currAudioFileInfo = new AudioFileInfo(dlg.FileName);
                Thread.Sleep(1000);

                audioPlayer.Init(currAudioFileInfo);
                audioPlayer.Volume = 1;

                SongNameText.Text = currAudioFileInfo.GetName();
                ImageBlock.Source = currAudioFileInfo.GetIcon();

                ResetButton(sender, e);
            }
        }

        private void MusicVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            audioPlayer.Volume = (float)(e.NewValue/100);
        }

        private void StartEndPos_Button_Click(object sender, RoutedEventArgs e)
        {
            if (currAudioFileInfo != null)
            {
                TimeSpan ts;
                string temp = "00:" + StartPos.Text;

                if (TimeSpan.TryParse(temp, out ts))
                {
                    audioPlayer.CurrentTime = String.Format("{0:00}:{1:00}"/*:{2:00}*/,
                        ts.Minutes, ts.Seconds/*, ts.Milliseconds / 10*/);
                }

                ClockTxtBlock.Text = $"{currAudioFileInfo.Duration} / {audioPlayer.CurrentTime}";

                currAudioFileInfo.CurrentTime = ts;
                temp = "00:" + EndPos.Text;
                if (TimeSpan.TryParse(temp, out ts))
                {
                    audioPlayer.EndingTime = ts;
                    audioPlayer.IsEndingTimeRealised = false;
                }
            }
        }

        private void StartPos_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"(\d|:)");
            if (!regex.IsMatch(e.Text) || StartPos.Text.Length > 5)
            {
                e.Handled = true;
                return;
            }
            startTxtTemplate.AddTimeNum(e.Text);
            StartPos.Text = startTxtTemplate.TimeText;
            e.Handled = true;
        }

        private void StartPos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                startTxtTemplate.SubTimeNum();
                StartPos.Text = startTxtTemplate.TimeText;
                e.Handled = true;
            }
        }
        private void EndPos_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"(\d|:)");
            if (!regex.IsMatch(e.Text) || EndPos.Text.Length > 5)
            {
                e.Handled = true;
                return;
            }
            endTxtTemplate.AddTimeNum(e.Text);
            EndPos.Text = endTxtTemplate.TimeText;
            e.Handled = true;
        }

        private void EndPos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                endTxtTemplate.SubTimeNum();
                EndPos.Text = endTxtTemplate.TimeText;
                e.Handled = true;
            }
        }

        private void DevicesConfirmButton_Click(object sender, RoutedEventArgs e)
        {

            audioPlayer.Stop();
            playerMic.Stop();
            recorder.StopRecording();

            audioPlayer = new AudioPlayer() { DeviceNumber = OutDevicesList.SelectedIndex - 1 };
            playerMic = new WaveOutEvent() { DeviceNumber=OutDevicesList.SelectedIndex - 1 };
            recorder = new WaveInEvent() { DeviceNumber = InDevicesList.SelectedIndex - 1 };
            recorder.WaveFormat = new WaveFormat(44100, 16, 1);


            recorder.DataAvailable += RecorderOnDataAvailable;
            playerMic.Init(bufferedWaveProvider);
            playerMic.Play();
            recorder.StartRecording();



            if (currAudioFileInfo != null)
            {
                var tempPos = currAudioFileInfo.Position;
                currAudioFileInfo = new AudioFileInfo(currAudioFileInfo.FileName);
                audioPlayer.Init(currAudioFileInfo);
                currAudioFileInfo.Position = tempPos;
            }

            if (audioPlayer.IsPlaying)
                audioPlayer.Play();
            
        }
        
    }

}
