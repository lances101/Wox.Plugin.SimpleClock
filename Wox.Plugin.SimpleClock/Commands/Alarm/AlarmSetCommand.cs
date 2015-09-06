using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock.Commands.Alarm
{
    public class AlarmSetCommand : CommandHandlerBase
    {
        public AlarmSetCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }

        public override string CommandAlias
        {
            get
            {
                return "set";
            }
        }

        public override string CommandDescription
        {
            get
            {
                return "Sets a new alarm";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Set new alarm";
            }
        }

        public override string GetIconPath()
        {
            return "Images\\alarm-green.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            DateTime time;
            try
            {
                time = DateTime.ParseExact(args[commandDepth], "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch(FormatException e)
            {
                throw new ArgumentException("Date Invalid:" + e.Message);
            }


            if (time < DateTime.Now) time = time.AddDays(1);

            var name = "Alarm";
            if (args.Count > commandDepth + 1)
            {
                name = String.Join(" ", args.Skip(commandDepth + 1).ToArray());
            }

            ClockSettingsStorage.Instance.Alarms.Add(new ClockSettingsStorage.StoredAlarm(true)
            {
                AlarmTime = time,
                Name = name
            });
            ClockSettingsStorage.Instance.Save();
            RequeryWithArguments(args);

            _forcedTitle = "Alarm set!";
            _forcedSubtitle = String.Format("\"{0}\" will fire at {1}", name, time.ToString());
            
            return false;
        }

        public override List<Result> Query(Query query)
        {
            var res = new List<Result>();
            var args = query.ActionParameters;
            res.Add(new Result()
            {
                Title = String.IsNullOrEmpty(_forcedTitle) ? "You are setting a new alarm" : _forcedTitle,
                SubTitle = String.IsNullOrEmpty(_forcedSubtitle) ? "Accepts: time as HH:MM, name as any string" : _forcedSubtitle,
                IcoPath = GetIconPath(),
                Action = e =>
                {
                    return ExecuteCommand(args);
                }
            });

            return res;
        }
    }
}
