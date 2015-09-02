
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils
{
    public class AlarmCommandHandler : CommandHandlerBase
    {
        public AlarmCommandHandler(PluginInitContext context): base(context)
        {
            
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
        public override List<Result> Query(Query query)
        {
            var args = query.ActionParameters;
            List<Result> subCommands = new List<Result>();
            subCommands.Add(new Result()
            {
                Title = "set",
                SubTitle = "Set an alarm for a specific time",
                Action = e =>
                {
                    if(args.Count > 2)
                    {
                        _context.API.ShowMsg("Settings alarm for " + args[2]);
                        return false;
                    }
                    _context.API.ChangeQuery(String.Format("{0} alarm set ", _context.CurrentPluginMetadata.ActionKeyword), true);
                    return false; 
                }
            }
            );
            subCommands.Add(new Result()
            {
                Title = "in",
                SubTitle = "Set an alarm to fire after an amount of time",
                Action = e =>
                {
                    _context.API.ChangeQuery(String.Format("{0} alarm set ", _context.CurrentPluginMetadata.ActionKeyword), true);
                    return false;
                }
            }
            );
            if (args.Count > 1)
            {
                return subCommands.Where(r => query.ActionParameters[1].StartsWith(r.Title)).ToList();
            }
            return subCommands;
        }

        
    }
}
