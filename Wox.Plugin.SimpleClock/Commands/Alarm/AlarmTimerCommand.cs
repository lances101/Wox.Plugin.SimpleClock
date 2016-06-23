using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock.Commands.Alarm
{
    public class AlarmTimerCommand : CommandHandlerBase
    {
        public AlarmTimerCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }

        public override string CommandAlias
        {
            get
            {
                return "timer";
            }
        }

        public override string CommandDescription
        {
            get
            {
                return "Adds a timer to fire after a specific amount of time";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Add a timer";
            }
        }

        public override string GetIconPath()
        {
            return "Images\\alarm_timer.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            TimeSpan timeSpan;
            try
            {
                timeSpan = TimeSpan.Parse(args[CommandDepth]);
            }
            catch(FormatException e)
            {
                throw new ArgumentException("Timespan invalid: " + e.Message);
            }
            var time = DateTime.Now.Add(timeSpan);

            var name = "Timer";
            if (args.Count > CommandDepth + 1)
            {
                name = String.Join(" ", args.Skip(CommandDepth + 1).ToArray());
            }

            ClockSettingsWrapper.Settings.Alarms.Add(new AlarmSettings.StoredAlarm(true)
            {
                AlarmTime = time,
                Name = name
            });
            ClockSettingsWrapper.Storage.Save();
            ForcedTitle = "Timer set!";
            ForcedSubtitle = String.Format("\"{0}\" will fire at {1}", name, time.ToString());
        
            return false;
        }

        protected override List<Result> CommandQuery(Query query, ref List<Result> results)
        {
            var args = query.ActionParameters;
            var parsedTime = new TimeSpan();
            var dateCorrect = (query.ActionParameters.Count <= CommandDepth) ? false :
                TimeSpan.TryParse(query.ActionParameters[CommandDepth], out parsedTime);
            var nameCorrect = (query.ActionParameters.Count <= CommandDepth + 1) ? false :
                !String.IsNullOrEmpty(query.ActionParameters[CommandDepth + 1]);

            results.Add(new Result()
            {
                Title = !String.IsNullOrEmpty(ForcedTitle) ? ForcedTitle : "You are adding a new timer",
                SubTitle = !String.IsNullOrEmpty(ForcedSubtitle) ? ForcedSubtitle :
                    String.Format("Accepts: {0}, {1}. ",
                        !dateCorrect ? "time as HH:MM:SS" : "parsed time " + String.Format("{0} hours {1} minutes {2} seconds", parsedTime.Hours, parsedTime.Minutes, parsedTime.Seconds),
                        !nameCorrect ? "name as any string" : "parsed name " + String.Join(" ", query.ActionParameters.Skip(2).ToArray())),
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
