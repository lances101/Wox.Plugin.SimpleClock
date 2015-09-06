using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.SimpleClock.Commands
{
    public class AlarmDeleteCommand : CommandHandlerBase
    {
        public AlarmDeleteCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }

        public override string CommandAlias
        {
            get
            {
                return "delete";
            }
        }

        public override string CommandDescription
        {
            get
            {
                return "Deletes an existing alarm";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Delete alarm";
            }
        }
        public override string GetIconPath()
        {
            return "Images\\alarm-red.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            var id = args[commandDepth];
            var alarm = ClockSettingsStorage.Instance.Alarms.FirstOrDefault(a => a.Id == id);
            if (alarm == null)
            {
                throw new ArgumentException(String.Format("Alarm with id {0} was not found", id));
            }
            ClockSettingsStorage.Instance.Alarms.RemoveAll(r => r.Id == id);
            ClockSettingsStorage.Instance.Save();
            _forcedTitle = "Alarm deleted";
            _forcedSubtitle = String.Format("\"{0}\" was deleted", id);
            return false;
        }

        public override List<Result> Query(Query query)
        {
            var res = new List<Result>();
            var args = query.ActionParameters;
            var alarms = ClockSettingsStorage.Instance.Alarms.Where(r => !r.Fired);
            if (alarms.Count() == 0)
            {
                res.Add(new Result()
                {
                    Title = "There are no alarms set",
                    IcoPath = GetIconPath(),
                });
                return res;
            }
            res.Add(new Result()
            {
                Title = String.IsNullOrEmpty(_forcedTitle) ? "Choose an alarm to edit" : _forcedTitle,
                SubTitle = String.IsNullOrEmpty(_forcedSubtitle) ? "" : _forcedSubtitle,
                IcoPath = GetIconPath(),
                Action = e =>
                {
                    ExecuteCommand(args);
                    RequeryCurrentCommand();
                    return false;
                }
            });
            foreach (var alarm in alarms)
            {
                res.Add(new Result()
                {
                    Title = alarm.Id,
                    SubTitle = String.Format("Alarm \"{0}\" is set for {1}", alarm.Name, alarm.AlarmTime.ToString("dd/MM/yyyy HH:mm")),
                    IcoPath = GetIconPath(),
                    Action = e =>
                    {
                        args.Add(alarm.Id);
                        ExecuteCommand(args);
                        return false;
                    }
                });
            }
            return res;
        }
    }
}
