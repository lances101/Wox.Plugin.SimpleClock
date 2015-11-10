using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Wox.Plugin.Boromak;
using Wox.Plugin.Features;

namespace Wox.Plugin.SimpleClock
{
    public class ClockPlugin : IPlugin, ISettingProvider, IInstantQuery
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
            _initialCommandHandler = new Commands.AlarmCommand(context, null);
        }
        public List<Result> Query(Query query)
        {
            return _initialCommandHandler.Query(query);
        }


        public bool IsInstantQuery(string query)
        {
            return true;
        }
    }
}
