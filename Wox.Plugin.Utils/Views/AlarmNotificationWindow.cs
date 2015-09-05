using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using Timer = System.Timers.Timer;
namespace Wox.Plugin.Views
{
    class AlarmNotificationWindow : Window
    {
        
        public AlarmNotificationWindow(DateTime time, string name)
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
            Timer beepTimer = new Timer(1000);
            beepTimer.Elapsed += delegate
             {
                 System.Media.SystemSounds.Beep.Play();
             };

            this.Closing += delegate
            {
                beepTimer.Stop();
            };

            beepTimer.Start();

            Content = new NotifyAlarmUserControl(time, name);
        }
        

        private void AlarmNotificationWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                Window.GetWindow(this).Close();
        }
    }
}
