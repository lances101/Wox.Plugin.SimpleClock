using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
namespace Wox.Plugin.Utils.Commands.Alarms
{
    public class AlarmStorage
    {
        private static AlarmStorage _storage;
        public static AlarmStorage Instance {
            get
            {
                if (_storage == null)
                {
                    _storage = new AlarmStorage();
                    _storage.LoadAlarms();
                }
                return _storage;
                
            }
        }
        
        private void LoadAlarms()
        {
            if (!Directory.Exists("Storage"))
                Directory.CreateDirectory("Storage");
            if(!File.Exists("Storage/alarms.json"))
            {
                Alarms = new List<StoredAlarm>();
                return;
            }
            var json = File.ReadAllText("Storage/alarms.json");
            Alarms = JsonConvert.DeserializeObject<List<StoredAlarm>>(json)?? new List<StoredAlarm>();
        }
        public void SaveAlarms()
        {
            if (!Directory.Exists("Storage"))
                Directory.CreateDirectory("Storage");
            File.WriteAllText("Storage/alarms.json", JsonConvert.SerializeObject(Alarms));
        }

        public List<StoredAlarm> Alarms = new List<StoredAlarm>();
        public class StoredAlarm
        {
            public StoredAlarm(bool isNew = false)
            {
                if (isNew)
                    Id = (AlarmStorage.Instance.Alarms.Count + 1).ToString();

            }
            public string Id { get; set; }
            public DateTime AlarmTime { get; set; }
            public string TrackPath { get; set; }
            public string Name { get; set; }
            public bool Fired { get; set; }
        }   
    }
}
