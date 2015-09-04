using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils
{
    public abstract class CommandHandlerBase
    {
        protected PluginInitContext _context;
        protected List<CommandHandlerBase> _subCommands = new List<CommandHandlerBase>();
        protected CommandHandlerBase _parentCommand;
        protected int _commandDepth = -1;
        protected string _forcedTitle;
        protected string _forcedSubtitle;
        
        public CommandHandlerBase(PluginInitContext context, CommandHandlerBase parent)
        {
            _context = context;
            _parentCommand = parent;
            var temp = this;
            while(temp != null)
            {
                temp = temp._parentCommand;
                _commandDepth++;
            }
        }
        public abstract string CommandAlias { get; }
        public abstract string CommandTitle { get; }
        public abstract string CommandDescription { get; }
        protected virtual string CommandIconPath { get; }
        public virtual string GetIconPath()
        {
            var path = "";
            if (_parentCommand != null)
                return this._parentCommand.GetIconPath();
            return _context.CurrentPluginMetadata.IcoPath;
            
        }
        public virtual List<Result> Query(Query query)
        {
            var args = query.ActionParameters;
            List<Result> results = new List<Result>();
            if (args.Count - _commandDepth <= 0)
            {
                FillResultsWithSubcommands(args, results);
            }
            else
            {
                var specificHandler = _subCommands.FirstOrDefault(r => r.CommandAlias == args[_commandDepth].ToLower());
                if (specificHandler != null)
                {
                    results.AddRange(specificHandler.Query(query));
                }
                else
                {
                    FillResultsWithSubcommands(args, results, args[_commandDepth].ToLower());
                }
            }
            return results;
        }

        private void FillResultsWithSubcommands(List<string> args, List<Result> results, string filterAlias = "")
        {
            foreach (var subcommand in _subCommands)
            {
                if (filterAlias != "" && !subcommand.CommandAlias.Contains(filterAlias)) continue;

                results.Add(new Result()
                {
                    Title = subcommand.CommandTitle,
                    SubTitle = subcommand.CommandDescription,
                    IcoPath = subcommand.GetIconPath(),
                    Action = e =>
                    {
                        return subcommand.ExecuteCommand(args);
                    }

                });
            }
        }

        
        public virtual bool ExecuteCommand(List<string> args)
        {
            RequeryCurrentCommand();
            return false;

        }
        protected void RequeryWithArguments(List<string> args)
        {
            _context.API.ChangeQuery(String.Format("{0} {1} ", _context.CurrentPluginMetadata.ActionKeyword, String.Join(" ", args.ToArray()), true));
        }
        public void RequeryCurrentCommand()
        {
            _context.API.ChangeQuery(String.Format("{0} {1}", _context.CurrentPluginMetadata.ActionKeyword, GetCommandPath()), true);
        }

        private string GetCommandPath()
        {
            string path = "";
            var temp = this;
            while (temp != null)
            {
                if(!String.IsNullOrEmpty(temp.CommandAlias))
                    path = path.Insert(0, temp.CommandAlias + " ");
                temp = temp._parentCommand;
            }

            return path;
        }
    }
}
