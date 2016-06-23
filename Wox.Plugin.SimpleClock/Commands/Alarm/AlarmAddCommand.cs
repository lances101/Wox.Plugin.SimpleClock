using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock.Commands.Alarm
{
    public class AlarmAddCommand : CommandHandlerBase
    {
        public AlarmAddCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }

        public override string CommandAlias
        {
            get
            {
                return "add";
            }
        }

        public override string CommandDescription
        {
            get
            {
                return "Adds a new alarm";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Add new alarm";
            }
        }

        public override string GetIconPath()
        {
            return "Images\\alarm_add.png";
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

            ClockSettingsWrapper.Settings.Alarms.Add(new AlarmSettings.StoredAlarm(true)
            {
                AlarmTime = time,
                Name = name
            });
            ClockSettingsWrapper.Storage.Save();
            RequeryPlugin(args);

            ForcedTitle = "Alarm set!";
            ForcedSubtitle = String.Format("\"{0}\" will fire at {1}", name, time.ToString());
            
            return false;
        }
        
        protected override List<Result> CommandQuery(Query query, ref 
            List<Result> results)
        {
            var args = query.ActionParameters;
            var parsedTime = new DateTime(); 
            var dateCorrect = (query.ActionParameters.Count <= CommandDepth) ? false : 
                DateTime.TryParseExact(args[CommandDepth], "HH:mm", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTime);
            var nameCorrect = (query.ActionParameters.Count <= CommandDepth + 1)? false : 
                !String.IsNullOrEmpty(query.ActionParameters[CommandDepth+1]);
            results.Add(new Result()
            {
                Title = !String.IsNullOrEmpty(ForcedTitle) ? ForcedTitle : "You are adding a new alarm" ,
                SubTitle = !String.IsNullOrEmpty(ForcedSubtitle) ? ForcedSubtitle : 
                    String.Format("Accepts: {0}, {1}. ",
                        !dateCorrect? "time as HH:MM" : "parsed time " + parsedTime.ToShortTimeString(),
                        !nameCorrect? "name as any string" : "parsed name " + String.Join(" ", query.ActionParameters.Skip(2).ToArray())),
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
