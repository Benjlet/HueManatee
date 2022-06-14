using Newtonsoft.Json;

namespace HueManatee
{
    internal class HueLightStartup
    {
        [JsonProperty("mode")]
        internal string Mode { get; set; }

        [JsonProperty("configured")]
        internal bool Configured { get; set; }
    }
}
