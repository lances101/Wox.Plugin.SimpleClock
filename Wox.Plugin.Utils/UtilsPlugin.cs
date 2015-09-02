using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils
{
    public class Main : IPlugin
    {
        private PluginInitContext context;
        private CommandHandlerBase[] _commandHandlers;
        private List<Result> _results = new List<Result>();
        public void Init(PluginInitContext context)
        {
            this.context = context;
            _commandHandlers = new[]
            {
                new AlarmCommandHandler(context)
            };

            foreach (var handler in _commandHandlers)
            {
                _results.Add(new Result()
                {
                    Title = handler.CommandAlias,
                    SubTitle = handler.CommandDescription,
                    IcoPath = "Images\\utils.png",
                    Action = e =>
                    {
                        context.API.ChangeQuery(String.Format("{0} {1} ", context.CurrentPluginMetadata.ActionKeyword, handler.CommandAlias ), true);
                        return false;
                    }
                });
            }
        }
        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();
            
            var args = query.ActionParameters;
            var str = "";
            for(int i = 0; i < args.Count; i++)
            {
                str += String.Format("{0}-{1}({2})|", i, args[i], args[i].Length);
            }

            if (args.Count == 0)
            {
                results = new List<Result>(_results);
            }
            else
            {
                var specificHandler = _commandHandlers.FirstOrDefault(r => r.CommandAlias == args[0].ToLower());
                if (specificHandler != null)
                {
                    results.AddRange(specificHandler.Query(query));
                }
                else
                {
                    results = _results.Where(r => r.Title.ToLower().StartsWith(args[0].ToLower())).ToList();
                }
            }


            results.Add(new Result()
            {
                Title = "Debugging",
                SubTitle = "Args are " + str,
                Action = e =>
                {
                    return false;
                }
            });
            return results;
        }
        
        
    }
}
