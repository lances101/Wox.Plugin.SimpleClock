using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Wox.Plugin.Boromak;
namespace Wox.Plugin.SimpleClock
{
    public class ClockPlugin : IPlugin, ISettingProvider
    {
        private PluginInitContext context;
        private CommandHandlerBase _initialCommandHandler;
        
        public Control CreateSettingPanel()
        {
            return new Views.SettingsControl(context.CurrentPluginMetadata.PluginDirectory);
        }

        public void Init(PluginInitContext context)
        {
            this.context = context;
            _initialCommandHandler = new Commands.ClockCommand(context, null);
        }
        public List<Result> Query(Query query)
        {
            return _initialCommandHandler.Query(query);
        }
        
        
    }
}
