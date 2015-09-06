using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Wox.Plugin.SimpleClock.Views
{
    class AlarmNotificationWindow : Window
    {
        public class AudioPlayer
        {
            private Thread backThread;
            public AudioPlayer(string file)
            {
                
                Open(file);
            }

            public void Open(string file)
            {
                device = new NAudio.Wave.WaveOut();
                player = new NAudio.Wave.AudioFileReader(file);
            }
            public void PlayLooped()
            {
                backThread = new Thread(o =>
                {
                    try
                    {
                        device.Init(player);

                        device.PlaybackStopped += (s, e) =>
                        {
                            player.Seek(0, System.IO.SeekOrigin.Begin);
                            device.Play();
                        };

                        device.Play();
                    }
                    catch(Exception e)
                    {

                    }
                });
                backThread.Start();
            }
            public void Stop()
            {
                
                backThread.Abort();
                if (device != null)
                    device.Stop();
                if (player != null)
                {
                    player.Dispose();
                    player = null;
                }
                if (device != null)
                {
                    device.Dispose();
                    device = null;
                }
            }

            NAudio.Wave.WaveOut device;
            NAudio.Wave.AudioFileReader player;

        }
        public AlarmNotificationWindow(DateTime time, string name, string trackPath)
        {

            
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Width = 300;
            this.Height = 150;
            this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
            this.KeyDown += AlarmNotificationWindow_KeyDown;
            this.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            AudioPlayer player = new AudioPlayer(trackPath);
            player.PlayLooped();
            this.Closing += delegate
            {
                player.Stop();
            };
            Content = new NotifyAlarmUserControl(time, name);
        }
        

        private void AlarmNotificationWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Window.GetWindow(this).Close();
        }
    }
}
