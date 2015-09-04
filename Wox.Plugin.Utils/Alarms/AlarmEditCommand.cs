using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils.Alarms
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

        public override bool ExecuteCommand(List<string> args)
        {
            _forcedTitle = "";
            _forcedSubtitle = "";
            if (args.Count > _commandDepth)
            {
                try
                {
                    var id = args[_commandDepth];
                    var alarm = AlarmStorage.Instance.Alarms.FirstOrDefault(a => a.Id == id);
                    if (alarm == null)
                    {
                        _forcedSubtitle = String.Format("Alarm with id {0} was not found", id);
                        RequeryWithArguments(args);
                        return false;
                    }
                    if (args.Count <= _commandDepth + 1)
                    {
                        _forcedSubtitle = "No date provided";
                        RequeryWithArguments(args);
                        return false;
                    }
                    
                    var time = DateTime.ParseExact(args[_commandDepth + 1], "HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    if (time < DateTime.Now) time = time.AddDays(1);

                    var name = "Alarm";
                    if (args.Count > _commandDepth + 2)
                    {
                        name = String.Join(" ", args.Skip(_commandDepth+2).ToArray());
                    }

                    alarm.AlarmTime = time;
                    alarm.Name = name == "Alarm" ? alarm.Name : name;

                    AlarmStorage.Instance.SaveAlarms();
                    _forcedTitle = "Alarm edited";
                    _forcedSubtitle = String.Format("\"{0}\" was reset to fire at {1}", name, time.ToString());
                    RequeryCurrentCommand();
                }
                catch (FormatException e)
                {
                    _forcedTitle = "An error has occured";
                    _forcedSubtitle = e.Message;
                    RequeryWithArguments(args);
                }
            }
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
            else
            {
                res.Add(new Result()
                {
                    Title = String.IsNullOrEmpty(_forcedTitle) ? "Choose an alarm to edit" : _forcedTitle,
                    SubTitle = String.IsNullOrEmpty(_forcedSubtitle) ? "" : _forcedSubtitle,
                    IcoPath = GetIconPath(),
                    Action = e =>
                    {
                        ExecuteCommand(args);
                        RequeryCurrentCommand();
                        return false;
                    }
                });
                foreach (var alarm in alarms)
                {
                    res.Add(new Result()
                    {
                        Title = alarm.Id,
                        SubTitle = String.Format("Alarm \"{0}\" is set for {1}", alarm.Name, alarm.AlarmTime.ToString("dd/MM/yyyy HH:mm")),
                        IcoPath = GetIconPath(),
                        Action = e =>
                        {
                            args.Add(alarm.Id);
                            args.Add(alarm.AlarmTime.ToString("HH:mm"));
                            args.Add(alarm.Name);
                            RequeryWithArguments(args);
                            return false;
                        }
                    });
                }
            }
            return res;
        }
    }
}
