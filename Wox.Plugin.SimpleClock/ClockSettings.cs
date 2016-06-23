using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wox.Plugin.SimpleClock
{
    public class ClockSettings
    {
        [JsonProperty]
        public string AlarmTrackPath { get; set; }
        
        [JsonProperty]
        public List<StoredAlarm> Alarms = new List<StoredAlarm>();

        public class StoredAlarm
        {
            public StoredAlarm(bool isNew = false)
            {
                if (isNew)
                    Id = (ClockSettingsWrapper.Settings.Alarms.Count + 1).ToString();

            }
            [JsonProperty]
            public string Id { get; set; }
            [JsonProperty]
            public DateTime AlarmTime { get; set; }
            [JsonProperty]
            public string Name { get; set; }
            [JsonProperty]
            public bool Fired { get; set; }
        }
    }
}
