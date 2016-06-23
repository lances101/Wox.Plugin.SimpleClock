using System;
using System.Collections.Generic;
using System.Globalization;
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
            return "Images\\alarm_edit.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            var id = args[CommandDepth];
            var alarm = ClockSettingsWrapper.Settings.Alarms.FirstOrDefault(a => a.Id == id);
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

            ClockSettingsWrapper.Storage.Save();
            ForcedTitle = "Alarm edited";
            ForcedSubtitle = String.Format("\"{0}\" was reset to fire at {1}", name, time);
            return false;
        }

        protected override List<Result> CommandQuery(Query query, ref List<Result> results)
        {
            var args = query.ActionParameters;
            var alarms = ClockSettingsWrapper.Settings.Alarms.Where(r => !r.Fired);
            if (!alarms.Any())
            {
                results.Add(new Result()
                {
                    Title = "There are no alarms set",
                    IcoPath = GetIconPath(),
                });
                return results;
            }

            if (query.ActionParameters.Count > CommandDepth)
            {
                var id = query.ActionParameters[CommandDepth];
                if (id != null)
                {
                    var parsedTime = new DateTime();
                    var dateCorrect = (query.ActionParameters.Count <= CommandDepth + 1) ? false :
                        DateTime.TryParseExact(args[CommandDepth + 1], "HH:mm", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTime);
                    var nameCorrect = (query.ActionParameters.Count <= CommandDepth + 2) ? false :
                        !String.IsNullOrEmpty(query.ActionParameters[CommandDepth + 2]);

                    var alarm = alarms.First(o => o.Id == id);
                    ForcedTitle = String.Format("You are editing alarm {0} with id {1} ", alarm.Name, alarm.Id);
                    ForcedSubtitle = !String.IsNullOrEmpty(ForcedSubtitle)? ForcedSubtitle: 
                        String.Format("Accepts: {0}, {1}. ",
                            !dateCorrect ? "time as HH:MM" : 
                                "new time " + parsedTime.ToShortTimeString(),
                            !nameCorrect? "name as any string": 
                                "new name " + String.Join(" ", query.ActionParameters.Skip(3).ToArray()));
                }
            }
            else
            {
                foreach (var alarm in alarms)
                {
                    results.Add(new Result()
                    {
                        Title = alarm.Name,
                        SubTitle =
                            String.Format("Set for {0}", alarm.AlarmTime.ToString("dd/MM/yyyy HH:mm")),
                        IcoPath = GetIconPath(),
                        Action = e =>
                        {
                            RequeryCurrentCommand(new List<string>
                            {
                                alarm.Id,
                                alarm.AlarmTime.ToString("HH:mm"),
                                alarm.Name
                            });
                            return false;
                        }
                    });
                }
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

            return results;
        }
    }
}
