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
        public class ClockSettingsStorage : PluginJsonStorage<AlarmSettings>
        {
        };
        private static AlarmSettings _settings;
        private static ClockSettingsStorage _storage;
        public static AlarmSettings Settings
        {
            get
            {
                if (_settings == null)                   
                    _settings = Storage.Load();
                return _settings;
            }
        }

        public static ClockSettingsStorage Storage
        {
            get
            {
                if(_storage == null)
                    _storage = new ClockSettingsStorage();
                return _storage;
                
            }
        }
       
    }
}
