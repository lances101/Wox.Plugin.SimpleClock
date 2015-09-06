using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.SimpleClock.Commands
{
    /// <summary>
    /// Base class for commands
    /// Handles queries and command execution if not overriden
    /// </summary>
    public abstract class CommandHandlerBase
    {
        protected PluginInitContext _context;
        protected List<CommandHandlerBase> _subCommands = new List<CommandHandlerBase>();
        protected CommandHandlerBase _parentCommand;
        protected int commandDepth = -1;
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
                commandDepth++;
            }
        }
        public abstract string CommandAlias { get; }
        public abstract string CommandTitle { get; }
        public abstract string CommandDescription { get; }
        public virtual string CommandIconPath { get; }

        /// <summary>
        /// Get an icon for this command
        /// If this command does not have one, it will recursively search for it in its parents
        /// </summary>
        /// <returns>relative icon path</returns>
        public virtual string GetIconPath()
        {
            var path = "";
            if (_parentCommand != null)
                return this._parentCommand.GetIconPath();
            return _context.CurrentPluginMetadata.IcoPath;
            
        }
        /// <summary>
        /// If not overriden returns all subcommands of current command
        /// and sets result action to call subcommand
        /// </summary>
        /// <param name="query">query from parent command</param>
        /// <returns></returns>
        public virtual List<Result> Query(Query query)
        {
            var args = query.ActionParameters;
            List<Result> results = new List<Result>();
            if (args.Count - commandDepth <= 0)
            {
                FillResultsWithSubcommands(args, results);
            }
            else
            {
                var specificHandler = _subCommands.FirstOrDefault(r => r.CommandAlias == args[commandDepth].ToLower());
                if (specificHandler != null)
                {
                    results.AddRange(specificHandler.Query(query));
                }
                else
                {
                    FillResultsWithSubcommands(args, results, args[commandDepth].ToLower());
                }
            }
            return results;
        }

        /// <summary>
        /// Fills results with subcommands from parent
        /// </summary>
        /// <param name="args">arguments from query</param>
        /// <param name="results">list of results to fill</param>
        /// <param name="filterAlias">string to filter commands by name</param>
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

        /// <summary>
        /// Does a check so that the current command actually has a parameter for execution
        /// and then executes the CommandExecution function in a try/catch
        /// If the command threw an exception returns false and sets
        /// _forcedTitle and _forcedSubtitle properties
        /// </summary>
        /// <param name="args">list of arguments</param>
        /// <returns></returns>
        public bool ExecuteCommand(List<string> args)
        {
            bool shouldHide = false;
            _forcedTitle = "";
            _forcedSubtitle = "";
            if (args.Count > commandDepth)
            {
                try
                {
                    shouldHide = CommandExecution(args);
                }
                catch (ArgumentException e)
                {
                    _forcedTitle = "An error has occured";
                    _forcedSubtitle = e.Message;
                    RequeryWithArguments(args);
                    return false;
                }
            }
            RequeryCurrentCommand();
            return shouldHide;
        }

        /// <summary>
        /// If not overriden will requery with the current command without arguments
        /// Any parameter checks go here. If an argument was invalid you must 
        /// throw a ArgumentException with the error message
        /// </summary>
        /// <param name="args">query parameters</param>
        /// <returns type="boolean">should Wox hide after execution </returns>
        protected virtual bool CommandExecution(List<string> args)
        {
            RequeryCurrentCommand();
            return false;
        }

        /// <summary>
        /// Changes query using the provided argument strings
        /// </summary>
        protected void RequeryWithArguments(List<string> args)
        {
            _context.API.ChangeQuery(String.Format("{0} {1} ", _context.CurrentPluginMetadata.ActionKeyword, String.Join(" ", args.ToArray()), true));
        }

        /// <summary>
        /// Changes the query to the current referenced command
        /// </summary>
        public void RequeryCurrentCommand()
        {
            _context.API.ChangeQuery(GetCommandPath(), true);
        }

        /// <summary>
        /// Calculates the path to the current command
        /// </summary>
        /// <returns>path to this command without action keyword</returns>
        private string GetCommandPath()
        {
            
            string path = String.Empty;
            var temp = this;
            while (temp != null)
            {
                if (temp.commandDepth < 1) break;
                if(!String.IsNullOrEmpty(temp.CommandAlias))
                    path = path.Insert(0, temp.CommandAlias + " ");
                temp = temp._parentCommand;
            }
            path = path.Insert(0, _context.CurrentPluginMetadata.ActionKeyword + " ");
            return path;
        }
    }
}
