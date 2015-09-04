using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils.Alarms
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

        public override bool ExecuteCommand(List<string> args)
        {
            RequeryCurrentCommand();
            return false;
        }
       
        public override List<Result> Query(Query query)
        {
            var res = new List<Result>();
            var args = query.ActionParameters;
            var alarms = AlarmStorage.Instance.Alarms.Where(r => !r.Fired);
            if (alarms.Count() == 0)
            res.Add(new Result()
            {
                Title = "There are no alarms set",
                IcoPath = GetIconPath(),
            });
            foreach (var alarm in alarms)
            {
                res.Add(new Result()
                {
                    Title = alarm.Name,
                    SubTitle = String.Format("Programmed for {0}", alarm.AlarmTime.ToString("dd/MM/yyyy HH:mm")),
                    IcoPath = "Images\\alarm-blue.png",
                  
                });
            }
            return res;
        }
    }
}
