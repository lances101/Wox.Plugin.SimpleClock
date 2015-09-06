using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wox.Plugin.SimpleClock.Views
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private string pluginDirectory;
        public SettingsControl()
        {
            InitializeComponent();
        }

        public SettingsControl(string pluginDirectory)
        {
            this.pluginDirectory = pluginDirectory;
            InitializeComponent();
            if (String.IsNullOrEmpty(AlarmTrackProperty))
            {
                AlarmTrackProperty = System.IO.Path.Combine(pluginDirectory, "Sounds\\beepbeep.mp3");
            }
            tbxAudioFilePath.Text = AlarmTrackProperty;
        }

        public string AlarmTrackProperty
        {
            get
            {
                return ClockSettingsStorage.Instance.AlarmTrackPath;
            }
            set
            {
                if (!File.Exists(value))
                {
                    MessageBox.Show("Error when setting alarm track", "File not found");
                    tbxAudioFilePath.Text = AlarmTrackProperty;
                    return;
                }
                ClockSettingsStorage.Instance.AlarmTrackPath = value;
                ClockSettingsStorage.Instance.Save();
            }
        } 

        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.FileName = "Audio track";
            ofd.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            ofd.Filter = "Audio Files (mp3/wav)|*.mp3;*.wav|All Files|*.*";
            ofd.CheckPathExists = true;
            var res = ofd.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                tbxAudioFilePath.Text = ofd.FileName;
                AlarmTrackProperty = tbxAudioFilePath.Text;
            }
            
        }

        private void tbxAudioFilePath_LostFocus(object sender, RoutedEventArgs e)
        {
            AlarmTrackProperty = tbxAudioFilePath.Text;
        }
    }
}
