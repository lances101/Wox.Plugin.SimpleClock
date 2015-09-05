using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils
{
    public class Main : IPlugin
    {
        private PluginInitContext context;
        private Commands.CommandHandlerBase _initialCommandHandler;
        private List<Result> _results = new List<Result>();
        public void Init(PluginInitContext context)
        {
            this.context = context;
            _initialCommandHandler = new ExternalCommandHandler(context, null);
            

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

            results.AddRange(_initialCommandHandler.Query(query));
           

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
