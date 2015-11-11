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
    public class ClockSettingsStorage :JsonStrorage<ClockSettingsStorage>
    {
        protected override string ConfigFolder
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 
            }
        }

        protected override string ConfigName
        {
            get { return "alarms"; }
        }

        protected override void OnAfterLoad(ClockSettingsStorage obj)
        {
            if (String.IsNullOrEmpty(obj.AlarmTrackPath))
            {
                obj.AlarmTrackPath = System.IO.Path.Combine(ConfigFolder,"Sounds\\beepbeep.mp3");
                obj.Save();
            }
        }

        [JsonProperty]
        public string AlarmTrackPath { get; set; }
        [JsonProperty]
        public List<StoredAlarm> Alarms = new List<StoredAlarm>();

        public class StoredAlarm
        {
            public StoredAlarm(bool isNew = false)
            {
                if (isNew)
                    Id = (ClockSettingsStorage.Instance.Alarms.Count + 1).ToString();

            }
            [JsonProperty]
            public string Id { get; set; }
            [JsonProperty]
            public DateTime AlarmTime { get; set; }
            [JsonProperty]
            public string TrackPath { get; set; }
            [JsonProperty]
            public string Name { get; set; }
            [JsonProperty]
            public bool Fired { get; set; }
        }   
    }
}
