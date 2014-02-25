using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Media.Imaging;

namespace timecapsule_test
{
    public partial class RecordControl : UserControl
    {
        Microphone microphone = Microphone.Default;
        byte[] buffer;
        MemoryStream stream;
        SoundEffect sound;
        int counter;
        DispatcherTimer dt2 = new DispatcherTimer();
        BitmapImage Icon = new BitmapImage();
        bool start = false;

        public RecordControl()
        {
            InitializeComponent();
            Icon.SetSource(Application.GetResourceStream(new Uri(@"Icons/download.png", UriKind.Relative)).Stream);

            // Timer to simulate the XNA Game Studio game loop (Microphone is from XNA Game Studio)
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(50);
            dt.Tick += delegate { try { FrameworkDispatcher.Update(); } catch { } };
            dt.Start();
            dt2.Interval = TimeSpan.FromSeconds(5);
            dt2.Tick += new EventHandler(timer_Tick);
            microphone.BufferReady += new EventHandler<EventArgs>(microphone_BufferReady);
        }

        void microphone_BufferReady(object sender, EventArgs e)
        {
            microphone.GetData(buffer);
            stream.Write(buffer, 0, buffer.Length);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!start)
                return;
            while (start && counter < 10)
            {
                counter++;
            }
            if (microphone.State == MicrophoneState.Started)
            {
                microphone.Stop();
                dt2.Stop();
                start = false;
                Switch.Content = "Record";
            }
            counter = 0;
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (!start)
            {
                try
                {
                    stream = new MemoryStream();
                    microphone.BufferDuration = TimeSpan.FromMilliseconds(1000);
                    buffer = new byte[microphone.GetSampleSizeInBytes(microphone.BufferDuration)];
                    microphone.Start();
                    start = true;
                    Switch.Content = "Stop";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                dt2.Start();
            }
            else
            {
                if (microphone.State == MicrophoneState.Started)
                {
                    microphone.Stop();
                    dt2.Stop();
                    start = false;
                    Switch.Content = "Record";
                }

            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            sound = new SoundEffect(stream.ToArray(), microphone.SampleRate, AudioChannels.Mono);
            sound.Play();
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            Image image = new Image { Source = Icon };
            ((Grid)this.Parent).Children.Add(image);
            ((Grid)this.Parent).Children.Remove(this);
        }
    }
}
