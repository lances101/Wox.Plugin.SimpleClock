
using System;
using System.Collections.Generic;
using System.Linq;
using Wox.Plugin.SimpleClock.Views;
using Wox.Plugin.Boromak;
using Wox.Plugin.SimpleClock.Commands.Alarm;

namespace Wox.Plugin.SimpleClock.Commands
{

    public class AlarmCommand : CommandHandlerBase
    {
        private ClockSettingsStorage _storage;
        public AlarmCommand(PluginInitContext context, CommandHandlerBase parent): base(context, parent)
        {
            SubCommands.Add(new AlarmSetCommand(context, this));
            SubCommands.Add(new AlarmTimerCommand(context, this));
            SubCommands.Add(new AlarmListCommand(context, this));
            SubCommands.Add(new AlarmEditCommand(context, this));
            SubCommands.Add(new AlarmDeleteCommand(context, this));
            SubCommands.Add(new AlarmStopwatchCommand(context, this));

            _storage = ClockSettingsStorage.Instance;
            if (String.IsNullOrEmpty(_storage.AlarmTrackPath))
            {
                _storage.AlarmTrackPath = System.IO.Path.Combine(context.CurrentPluginMetadata.PluginDirectory, "Sounds\\beepbeep.mp3");
                _storage.Save();
            }
            System.Timers.Timer alarmTimer = new System.Timers.Timer(5000);
            alarmTimer.Elapsed += AlarmTimer_Elapsed;
            alarmTimer.Start();
        }

        List<ClockSettingsStorage.StoredAlarm> _alarms;
        private void AlarmTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_alarms == null)
                _alarms = _storage.Alarms;
            var toFire = _alarms.Where(r => !r.Fired).Where(r => r.AlarmTime < DateTime.Now);
            
            if (!toFire.Any()) return;

            var alarmToFire = toFire.First();

            alarmToFire.Fired = true;
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => {
                var window = new AlarmNotificationWindow(alarmToFire.AlarmTime, alarmToFire.Name, _storage.AlarmTrackPath);
                window.Show();
                window.Focus();
            }));
            _storage.Save();
        }

        public override string CommandAlias
        {
            get{ return "alarm"; }
        }

        public override string CommandDescription
        {
            get
            {
                return "Allows to set an alarm";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Alarm";
                
            }
        }
        
        public override string GetIconPath()
        {
            return "Images\\alarm_full.png";
        }
      

    }
    
}
