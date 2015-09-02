
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wox.Plugin.Utils
{
    public class AlarmCommandHandler : CommandHandlerBase
    {
        public class AlarmSetCommandHandler : CommandHandlerBase
        {
            public AlarmSetCommandHandler(PluginInitContext context, CommandHandlerBase parent) : base(context, parent) { }

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

            public override void ExecuteCommand(List<string> args)
            {
                throw new NotImplementedException();
            }

            public override bool IsCommandValid(List<string> args, out string error)
            {
                error = "";
                return false;
            }

            public override List<Result> Query(Query query)
            {
                return new List<Result>();
            }
        }

        public AlarmCommandHandler(PluginInitContext context): base(context, null)
        {
            _subCommands = new List<CommandHandlerBase>();
            _subCommands.Add(new AlarmSetCommandHandler(context, this));
        }

        public override string CommandAlias
        {
            get{ return "alarm"; }
        }

        public override string CommandDescription
        {
            get
            {
                return "Allows to set an alarm";
            }
        }

        public override string CommandTitle
        {
            get
            {
                return "Alarm Clock";
            }
        }

        public override bool IsCommandValid(List<string> args, out string error)
        {
            error = "";
            return true;
        }

      
    }
    
}
