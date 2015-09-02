using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Utils
{
    public abstract class CommandHandlerBase
    {
        protected PluginInitContext _context;
        public CommandHandlerBase(PluginInitContext context)
        {
            _context = context;
        }
        public abstract string CommandAlias { get; }
        public abstract string CommandDescription { get; }
        public abstract List<Result> Query(Query query);
    }
}
