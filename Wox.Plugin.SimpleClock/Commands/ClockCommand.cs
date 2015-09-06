using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock.Commands
{
    public class ClockCommand : CommandHandlerBase
    {
        public ClockCommand(PluginInitContext context, CommandHandlerBase parent) : base(context, parent)
        {
            _subCommands.Add(new AlarmCommand(context, this));
            _subCommands.Add(new AlarmStopwatchCommand(context, this));
        }

        public override string CommandAlias
        {
            get { return ""; }
        }

        public override string CommandDescription
        {
            get
            {
                return "";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "";

            }
        }

        public override string GetIconPath()
        {
            return "";
        }


    }
}
