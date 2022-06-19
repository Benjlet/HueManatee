using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueLightStartup
    {
        [JsonProperty("mode")]
        internal string Mode { get; set; }

        [JsonProperty("configured")]
        internal bool Configured { get; set; }
    }
}
