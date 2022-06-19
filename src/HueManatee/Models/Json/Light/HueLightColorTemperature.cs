using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueLightColorTemperature
    {
        [JsonProperty("min")]
        internal int Min { get; set; }

        [JsonProperty("max")]
        internal int Max { get; set; }
    }
}
