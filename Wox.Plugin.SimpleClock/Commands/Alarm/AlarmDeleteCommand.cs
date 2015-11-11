using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock.Commands.Alarm
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
            return "Images\\alarm_delete.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            var id = args[CommandDepth];
            var alarm = ClockSettingsStorage.Instance.Alarms.FirstOrDefault(a => a.Id == id);
            if (alarm == null)
            {
                throw new ArgumentException(String.Format("Alarm with id {0} was not found", id));
            }
            var name = alarm.Name;
            ClockSettingsStorage.Instance.Alarms.RemoveAll(r => r.Id == id);
            ClockSettingsStorage.Instance.Save();
            ForcedTitle = "Alarm deleted";
            ForcedSubtitle = String.Format("Alarm \"{0}\" with id {1} was deleted", name, id);
            return false;
        }

        protected override List<Result> CommandQuery(Query query, ref  List<Result> results)
        {
            var args = query.ActionParameters;
            var alarms = ClockSettingsStorage.Instance.Alarms.Where(r => !r.Fired);
            if (alarms.Count() == 0)
            {
                results.Add(new Result()
                {
                    Title = String.IsNullOrEmpty(ForcedTitle) ? "There are no alarms set" : ForcedTitle,
                    SubTitle = String.IsNullOrEmpty(ForcedSubtitle) ? "" : ForcedSubtitle,
                    IcoPath = GetIconPath(),
                });
                return results;
            }
            results.Add(new Result()
            {
                Title = String.IsNullOrEmpty(ForcedTitle) ? "Choose an alarm to edit" : ForcedTitle,
                SubTitle = String.IsNullOrEmpty(ForcedSubtitle) ? "" : ForcedSubtitle,
                IcoPath = GetIconPath(),
                Score = int.MaxValue,
                Action = e =>
                {
                    Execute(args);
                    RequeryCurrentCommand();
                    return false;
                }
            });
            foreach (var alarm in alarms)
            {
                results.Add(new Result()
                {
                    Title = alarm.Id,
                    SubTitle = String.Format("Alarm \"{0}\" is set for {1}", alarm.Name, alarm.AlarmTime.ToString("dd/MM/yyyy HH:mm")),
                    IcoPath = GetIconPath(),
                    Action = e =>
                    {
                        var comArgs = new List<string>(args);
                        comArgs.Add(alarm.Id);
                        Execute(comArgs);
                        RequeryCurrentCommand(null, true);
                        return false;
                    },
                    
                    
                });
            }
            return results;
        }
    }
}
