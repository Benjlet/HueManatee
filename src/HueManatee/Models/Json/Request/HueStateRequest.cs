using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueStateRequest
    {
        [JsonProperty("on", NullValueHandling = NullValueHandling.Ignore)]
        internal bool? On { get; set; } = null;

        [JsonProperty("bri", NullValueHandling = NullValueHandling.Ignore)]
        internal int? Brightness { get; set; } = null;

        [JsonProperty("sat", NullValueHandling = NullValueHandling.Ignore)]
        internal int? Saturation { get; set; } = null;

        [JsonProperty("hue", NullValueHandling = NullValueHandling.Ignore)]
        internal int? Hue { get; set; } = null;

        [JsonProperty("effect", NullValueHandling = NullValueHandling.Ignore)]
        internal string Effect { get; set; } = null;

        [JsonProperty("ct", NullValueHandling = NullValueHandling.Ignore)]
        internal int? ColorTemperature { get; set; } = null;
    }
}
