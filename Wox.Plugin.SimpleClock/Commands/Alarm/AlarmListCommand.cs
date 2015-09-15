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
                var count = ClockSettingsStorage.Instance.Alarms.Count(r => !r.Fired);
                return "Show all programmed alarms" + (count > 0? String.Format(" - {0} set", count) : "") ;
            }
        }
        public override string GetIconPath()
        {
            return "Images\\alarm_list.png";
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
                    IcoPath = "Images\\alarm_full.png",
                  
                });
            }
            return results;
        }
    }
}
