using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Wox.Infrastructure.Storage;

namespace Wox.Plugin.SimpleClock
{
    public class ClockSettingsWrapper
    {
        private static ClockSettings _settings;
        private static PluginJsonStorage<ClockSettings> _storage;
        public static ClockSettings Settings
        {
            get
            {
                if (_settings == null)                   
                    _settings = Storage.Load();
                return _settings;
            }
        }

        public static PluginJsonStorage<ClockSettings> Storage
        {
            get
            {
                if(_storage == null)
                    _storage = new PluginJsonStorage<ClockSettings>();
                return _storage;
                
            }
        }
       
    }
}
