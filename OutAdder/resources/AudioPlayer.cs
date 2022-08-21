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
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace OutAdder.resources
{
    class AudioPlayer : NAudio.Wave.WaveOutEvent
    {
        public AudioPlayer() : base()
        {
            IsPlaying = false;
            CurrentTime = String.Empty;
            IsEndingTimeRealised = true;
        }
        public TimeSpan EndingTime { get; set; }
        public string CurrentTime { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsEndingTimeRealised { get; set; }
        public void Play()
        {
            IsPlaying = true;
            base.Play();
        }
        public void Pause()
        {
            IsPlaying = false;
            base.Pause();
        }
        public void Stop()
        {
            IsPlaying = false;
            base.Stop();
        }
    }
}
