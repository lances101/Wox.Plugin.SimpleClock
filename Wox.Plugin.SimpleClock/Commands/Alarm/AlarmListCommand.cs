using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock.Commands.Alarm
{
    public class AlarmListCommand : CommandHandlerBase
    {
        public AlarmListCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }

        public override string CommandAlias
        {
            get
            {
                return "list";
            }
        }

        public override string CommandDescription
        {
            get
            {
                return "Shows all existing alarms";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Show all programmed alarms";
            }
        }
        public override string GetIconPath()
        {
            return "Images\\alarm-list.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            RequeryCurrentCommand();
            return false;
        }

        protected override List<Result> CommandQuery(Query query, ref List<Result> results)
        {
            var args = query.ActionParameters;
            var alarms = ClockSettingsStorage.Instance.Alarms.Where(r => !r.Fired);
            if (alarms.Count() == 0)
            results.Add(new Result()
            {
                Title = "There are no alarms set",
                IcoPath = GetIconPath(),
            });
            foreach (var alarm in alarms)
            {
                results.Add(new Result()
                {
                    Title = alarm.Name,
                    SubTitle = String.Format("Programmed for {0}", alarm.AlarmTime.ToString("dd/MM/yyyy HH:mm")),
                    IcoPath = "Images\\alarm-blue.png",
                  
                });
            }
            return results;
        }
    }
}
