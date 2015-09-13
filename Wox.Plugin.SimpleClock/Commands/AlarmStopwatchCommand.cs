using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock.Commands
{
    public class AlarmStopwatchCommand : CommandHandlerBase
    {
        public AlarmStopwatchCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public override string CommandAlias
        {
            get
            {
                return "stopwatch";
            }
        }

        public override string CommandDescription
        {
            get
            {
                return "Allows you to start and stop a stopwatch";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Stopwatch" + (stopwatch.IsRunning? " - Running" : "");
            }
        }
        public override string GetIconPath()
        {
            return "Images\\stopwatch-white.png";
        }

        protected override bool CommandExecution(List<string> args)
        {
            switch (args[commandDepth])
            {
                case "start":
                    stopwatch.Start();
                    break;
                case "stop":
                    stopwatch.Stop();
                    break;

                case "reset":
                    stopwatch.Reset();
                    break;


                case "restart":
                    stopwatch.Reset();
                    stopwatch.Start();
                    break;
                default:
                    RequeryCurrentCommand();
                    return false;
            }
            RequeryWithArguments(args);
            return false;
        }

        protected override List<Result> CommandQuery(Query query, ref List<Result> results)
        {
            var args = query.ActionParameters;
            results.Add(new Result()
            {
                Title = "Stopwatch Stats - " + stopwatch.Elapsed.ToString(),
                SubTitle = stopwatch.IsRunning? "Running" : "Stopped",
                IcoPath = GetIconPath(),
                Action = e =>
                {
                    Execute(args);
                    return false;
                }
            });

            results.Add(new Result()
            {
                Title = "Start",
                SubTitle = "Starts the stopwatch",
                Action = act =>
                {
                    args.Add("start");
                    Execute(args);
                    return false;
                },
            });

            results.Add(new Result()
            {
                Title = "Stop",
                SubTitle = "Stops the stopwatch",
                Action = act =>
                {
                    args.Add("stop");
                    Execute(args);
                    return false;
                },
            });
            results.Add(new Result()
            {
                Title = "Restart",
                SubTitle = "Restarts the stopwatch",
                Action = act =>
                {
                    args.Add("restart");
                    Execute(args);
                    return false;
                },
            });

            results.Add(new Result()
            {
                Title = "Reset",
                SubTitle = "Resets the stopwatch",
                Action = act =>
                {
                    args.Add("reset");
                   
                    Execute(args);
                    return false;
                },
            });
            return results;
        }
    }
}
