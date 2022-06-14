using Newtonsoft.Json;

namespace HueManatee
{
    internal class HueLightCapabilities
    {
        [JsonProperty("certified")]
        internal bool Certified { get; set; }

        [JsonProperty("control")]
        internal HueLightControl Control { get; set; }

        [JsonProperty("streaming")]
        internal HueLightStreaming Streaming { get; set; }
    }
}
