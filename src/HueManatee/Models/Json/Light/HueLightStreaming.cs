using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueLightStreaming
    {
        [JsonProperty("renderer")]
        internal bool Renderer { get; set; }

        [JsonProperty("proxy")]
        internal bool Proxy { get; set; }
    }
}
