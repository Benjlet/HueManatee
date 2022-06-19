using Newtonsoft.Json;
using System.Collections.Generic;

namespace HueManatee.Json
{
    internal class HueLightState
    {
        [JsonProperty("on")]
        internal bool On { get; set; }

        [JsonProperty("bri")]
        internal int Brightness { get; set; }

        [JsonProperty("hue")]
        internal int Hue { get; set; }

        [JsonProperty("sat")]
        internal int Sat { get; set; }

        [JsonProperty("effect")]
        internal string Effect { get; set; }

        [JsonProperty("xy")]
        internal List<double> XY { get; set; }

        [JsonProperty("ct")]
        internal int ColorTemperature { get; set; }

        [JsonProperty("alert")]
        internal string Alert { get; set; }

        [JsonProperty("colormode")]
        internal string ColorMode { get; set; }

        [JsonProperty("mode")]
        internal string Mode { get; set; }

        [JsonProperty("reachable")]
        internal bool Reachable { get; set; }
    }
}
