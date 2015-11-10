using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
namespace Wox.Plugin.SimpleClock
{
    /// <summary>
    /// Used for alarm storage. When the singleton instance is created
    /// the alarms are loaded from the XML file
    /// </summary>
    public class ClockSettingsStorage
    {
        private static ClockSettingsStorage _storage;
        public static ClockSettingsStorage Instance {
            get
            {
                if (_storage == null)
                {
                    _storage = new ClockSettingsStorage();
                    _storage.Load();
                }
                return _storage;
                
            }
        }

        /// <summary>
        /// Multiple path combine not implemented in .NET 3.5, so this looks dirty
        /// </summary>
        private static readonly string configFolderPath =
            Path.Combine(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Wox"),
                "Wox.Plugin.SimpleClock");

        private static readonly string configFilePath = Path.Combine(configFolderPath, "alarms.json");
        
        private void Load()
        {
            
            if (!Directory.Exists(configFolderPath))
                Directory.CreateDirectory(configFolderPath);
            if(!File.Exists(configFilePath))
            {
                Alarms = new List<StoredAlarm>();
                return;
            }
            var json = File.ReadAllText(configFilePath);
            Alarms = JsonConvert.DeserializeObject<List<StoredAlarm>>(json)?? new List<StoredAlarm>();
        }
        public void Save()
        {
            if (!Directory.Exists(configFolderPath))
                Directory.CreateDirectory(configFolderPath);
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(Alarms));
        }


        public string AlarmTrackPath { get; set; }
        public List<StoredAlarm> Alarms = new List<StoredAlarm>();
        public class StoredAlarm
        {
            public StoredAlarm(bool isNew = false)
            {
                if (isNew)
                    Id = (ClockSettingsStorage.Instance.Alarms.Count + 1).ToString();

            }
            public string Id { get; set; }
            public DateTime AlarmTime { get; set; }
            public string TrackPath { get; set; }
            public string Name { get; set; }
            public bool Fired { get; set; }
        }   
    }
}
