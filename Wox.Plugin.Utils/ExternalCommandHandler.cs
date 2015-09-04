using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin.Utils.Alarms;

namespace Wox.Plugin.Utils
{
    class ExternalCommandHandler : CommandHandlerBase
    {
        public ExternalCommandHandler(PluginInitContext context, CommandHandlerBase parent) : base(context, null)
        {
            _subCommands.Add(new AlarmCommand(context, this));
        }
        public override string CommandAlias
        {
            get
            {
                return "";
            }
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
    }
}
