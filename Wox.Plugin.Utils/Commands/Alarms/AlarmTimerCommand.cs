using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils.Commands.Alarms
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
            return "Images\\stopwatch-green.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            TimeSpan timeSpan;
            try
            {
                timeSpan = TimeSpan.Parse(args[_commandDepth]);
            }
            catch(FormatException e)
            {
                throw new ArgumentException("Timespan invalid: " + e.Message);
            }
            var time = DateTime.Now.Add(timeSpan);

            var name = "Timer";
            if (args.Count > _commandDepth + 1)
            {
                name = String.Join(" ", args.Skip(_commandDepth + 1).ToArray());
            }

            AlarmStorage.Instance.Alarms.Add(new AlarmStorage.StoredAlarm(true)
            {
                AlarmTime = time,
                Name = name
            });
            AlarmStorage.Instance.SaveAlarms();
            _forcedTitle = "Timer set!";
            _forcedSubtitle = String.Format("\"{0}\" will fire at {1}", name, time.ToString());
        
            return false;
        }

        public override List<Result> Query(Query query)
        {
            var res = new List<Result>();
            var args = query.ActionParameters;
            res.Add(new Result()
            {
                Title = String.IsNullOrEmpty(_forcedTitle) ?"You are adding a new timer" : _forcedTitle,
                SubTitle = String.IsNullOrEmpty(_forcedSubtitle) ? "Waiting for parameter formatted as HH:MM:SS" : _forcedSubtitle,
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
