using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using NAudio.Wave;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

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
                _device = new WaveOut();
                _player = new AudioFileReader(file);
            }
            public void PlayLooped()
            {
                backThread = new Thread(o =>
                {
                    try
                    {
                        _device.Init(_player);

                        _device.PlaybackStopped += (s, e) =>
                        {
                            _player.Seek(0, SeekOrigin.Begin);
                            _device.Play();
                        };

                        _device.Play();
                    }
                    catch (Exception e)
                    {
                        //ignore errors on thread abort
                    }
                });
                backThread.Start();
            }
            public void Stop()
            {
                
                backThread.Abort();
                if (_device != null)
                    _device.Stop();
                if (_player != null)
                {
                    _player.Dispose();
                    _player = null;
                }
                if (_device == null) return;

                _device.Dispose();
                _device = null;
            }

            WaveOut _device;
            AudioFileReader _player;

        }
        public AlarmNotificationWindow(DateTime time, string name, string trackPath)
        {

            Content = new NotifyAlarmUserControl(time, name);
            Topmost = true;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Width = 300;
            Height = 150;
            Left = Screen.PrimaryScreen.WorkingArea.Right - Width;
            Top = Screen.PrimaryScreen.WorkingArea.Bottom - Height;
            KeyDown += AlarmNotificationWindow_KeyDown;
            Background = new SolidColorBrush(Colors.Transparent);
            AudioPlayer player = new AudioPlayer(trackPath);
            player.PlayLooped();
            Closing += delegate
            {
                player.Stop();
            };
            
        }
        

        private void AlarmNotificationWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
                GetWindow(this).Close();
        }
    }
}
