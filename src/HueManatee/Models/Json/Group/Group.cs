using Newtonsoft.Json;
using System.Collections.Generic;

namespace HueManatee.Json
{
    internal class HueGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lights")]
        public List<string> Lights { get; set; }

        [JsonProperty("sensors")]
        public List<string> Sensors { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
