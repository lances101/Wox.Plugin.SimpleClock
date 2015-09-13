using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock.Commands.Alarm
{
    public class AlarmEditCommand : CommandHandlerBase
    {
        public AlarmEditCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }

        public override string CommandAlias
        {
            get
            {
                return "edit";
            }
        }

        public override string CommandDescription
        {
            get
            {
                return "Edits an existing alarm";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Edit alarm";
            }
        }

        public override string GetIconPath()
        {
            return "Images\\alarm-green.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            var id = args[CommandDepth];
            var alarm = ClockSettingsStorage.Instance.Alarms.FirstOrDefault(a => a.Id == id);
            if (alarm == null)
            {
                throw new ArgumentException(String.Format("Alarm with id {0} was not found", id));                   
            }
            if (args.Count <= CommandDepth + 1)
            {
                throw new ArgumentException("No date provided");
            }

            DateTime time;
            try
            {
                time = DateTime.ParseExact(args[CommandDepth + 1], "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch(FormatException e)
            {
                throw new ArgumentException("Date invalid: " + e.Message);
            }

            if (time < DateTime.Now) time = time.AddDays(1);

            var name = "Alarm";
            if (args.Count > CommandDepth + 2)
            {
                name = String.Join(" ", args.Skip(CommandDepth+2).ToArray());
            }

            alarm.AlarmTime = time;
            alarm.Name = name == "Alarm" ? alarm.Name : name;

            ClockSettingsStorage.Instance.Save();
            ForcedTitle = "Alarm edited";
            ForcedSubtitle = String.Format("\"{0}\" was reset to fire at {1}", name, time.ToString());
            return false;
        }

        protected override List<Result> CommandQuery(Query query, ref List<Result> results)
        {
            var args = query.ActionParameters;

            var alarms = ClockSettingsStorage.Instance.Alarms.Where(r => !r.Fired);
            if (alarms.Count() == 0)
            {
                results.Add(new Result()
                {
                    Title = "There are no alarms set",
                    IcoPath = GetIconPath(),
                });
                return results;
            }
            results.Add(new Result()
            {
                Title = String.IsNullOrEmpty(ForcedTitle) ? "Choose an alarm to edit" : ForcedTitle,
                SubTitle = String.IsNullOrEmpty(ForcedSubtitle) ? "" : ForcedSubtitle,
                IcoPath = GetIconPath(),
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
                        args.Clear();
                        
                        args.Add(alarm.Id);
                        args.Add(alarm.AlarmTime.ToString("HH:mm"));
                        args.Add(alarm.Name);
                        RequeryCurrentCommand(args);
                        return false;
                    }
                });
            }
            return results;
        }
    }
}
