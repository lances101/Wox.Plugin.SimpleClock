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
                time = DateTime.ParseExact(args[CommandDepth], "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch(FormatException e)
            {
                throw new ArgumentException("Date Invalid:" + e.Message);
            }


            if (time < DateTime.Now) time = time.AddDays(1);

            var name = "Alarm";
            if (args.Count > CommandDepth + 1)
            {
                name = String.Join(" ", args.Skip(CommandDepth + 1).ToArray());
            }

            ClockSettingsStorage.Instance.Alarms.Add(new ClockSettingsStorage.StoredAlarm(true)
            {
                AlarmTime = time,
                Name = name
            });
            ClockSettingsStorage.Instance.Save();
            RequeryPlugin(args);

            ForcedTitle = "Alarm set!";
            ForcedSubtitle = String.Format("\"{0}\" will fire at {1}", name, time.ToString());
            
            return false;
        }

        protected override List<Result> CommandQuery(Query query, ref 
            List<Result> results)
        {
            var args = query.ActionParameters;
            results.Add(new Result()
            {
                Title = String.IsNullOrEmpty(ForcedTitle) ? "You are setting a new alarm" : ForcedTitle,
                SubTitle = String.IsNullOrEmpty(ForcedSubtitle) ? "Accepts: time as HH:MM, name as any string" : ForcedSubtitle,
                IcoPath = GetIconPath(),
                Action = e =>
                {
                    return Execute(args);
                }
            });
            
            return results;
        }
    }
}
