using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils.Alarms
{
    public class AlarmDeleteCommand : CommandHandlerBase
    {
        public AlarmDeleteCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }

        public override string CommandAlias
        {
            get
            {
                return "delete";
            }
        }

        public override string CommandDescription
        {
            get
            {
                return "Deletes an existing alarm";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Delete alarm";
            }
        }
        public override string GetIconPath()
        {
            return "Images\\alarm-red.png";
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
                    AlarmStorage.Instance.Alarms.RemoveAll(r => r.Id == id);
                    AlarmStorage.Instance.SaveAlarms();
                    _context.API.ShowMsg("Alarm deleted", String.Format("\"{0}\" was deleted", id));
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
                foreach (var alarm in alarms)
                {
                    if (alarm.Fired) continue;
                    res.Add(new Result()
                    {
                        Title = alarm.Id,
                        SubTitle = String.Format("Alarm \"{0}\" is set for {1}", alarm.Name, alarm.AlarmTime.ToString("dd/MM/yyyy HH:mm")),
                        IcoPath = GetIconPath(),
                        Action = e =>
                        {
                            args.Add(alarm.Id);
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
