using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils
{
    public abstract class CommandHandlerBase
    {
        protected PluginInitContext _context;
        protected List<CommandHandlerBase> _subCommands;
        protected CommandHandlerBase _parentCommand;
        protected int _childDepth = 0;
        public CommandHandlerBase(PluginInitContext context, CommandHandlerBase parent)
        {
            _context = context;
            _parentCommand = parent;
            var temp = this;
            while(temp != null)
            {
                temp = parent;
                _childDepth++;
            }
        }
        public abstract string CommandAlias { get; }
        public abstract string CommandTitle { get; }
        public abstract string CommandDescription { get; }
        public virtual List<Result> Query(Query query)
        {
            var args = query.ActionParameters;
            List<Result> result = new List<Result>();
            foreach (var subcommand in _subCommands)
            {
                string error = "";
                result.Add(new Result()
                {
                    Title = subcommand.CommandTitle,
                    SubTitle = subcommand.CommandDescription,
                    Action = e =>
                    {
                        if (subcommand.IsCommandValid(args, out error))
                        {
                            subcommand.ExecuteCommand(args);
                            return true;
                        }
                        else
                        {
                            //do something with the error
                            return false;
                        }

                    }

                });
            }

            if (args.Count > _childDepth)
            {
                return result.Where(r => query.ActionParameters[1].StartsWith(r.Title)).ToList();
            }
            return result;
        }
        public abstract bool IsCommandValid(List<string> args, out string error);
        public virtual void ExecuteCommand(List<string> args)
        {
            string path = "";
            var temp = this;
            while(temp != null)
            {
                path.Insert(0, temp.CommandAlias + " ");
            }
            _context.API.ChangeQuery(String.Format("{0} {1} ", _context.CurrentPluginMetadata.ActionKeyword, path));
        }
    }
}
