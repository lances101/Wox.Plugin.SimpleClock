using System;
using System.Collections.Generic;
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

namespace Wox.Plugin.Views
{
    /// <summary>
    /// Interaction logic for NotifyAlarm.xaml
    /// </summary>
    public partial class NotifyAlarmUserControl : UserControl
    {
        private DateTime _time;
        private string _name;

        public NotifyAlarmUserControl()
        {
            InitializeComponent();
        }

        public NotifyAlarmUserControl(DateTime time, string name)
        {
            InitializeComponent();
            _time = time;
            _name = name;
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            txtblkAlarmTime.Text = _time.ToString("dd/MM/yyyy HH:mmm");
            txtblkAlarmName.Text = _name;
        }

        private void button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

       
    }
}
