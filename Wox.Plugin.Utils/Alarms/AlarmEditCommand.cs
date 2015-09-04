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
            if (args.Count > _commandDepth)
            {
                try
                {
                    var id = args[_commandDepth];
                    var alarm = AlarmStorage.Instance.Alarms.FirstOrDefault(a => a.Id == id);
                    if (alarm == null)
                    {
                        _lastError = String.Format("Alarm with id {0} was not found", id);
                        RequeryWithArguments(args);
                        return false;
                    }
                    if (args.Count <= _commandDepth + 1)
                    {
                        _lastError = "No date provided";
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
                    _context.API.ShowMsg("Alarm changed", String.Format("\"{0}\" will now fire at {1}", name, time.ToString("dd/MM/yyyy HH:mm")));
                    return true;
                    
                }
                catch (FormatException e)
                {
                    _lastError = "Provided date is invalid";
                    RequeryWithArguments(args);
                }
            }
            SetQueryToCurrentCommand();
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
                    Title = String.IsNullOrEmpty(_lastError) ? "Choose an alarm to edit" : "An error has occured when trying to edit",
                    SubTitle = String.IsNullOrEmpty(_lastError) ? "" : _lastError,
                    IcoPath = GetIconPath(),
                    Action = e =>
                    {
                        ExecuteCommand(args);
                        SetQueryToCurrentCommand();
                        return false;
                    }
                });
                foreach (var alarm in AlarmStorage.Instance.Alarms)
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
