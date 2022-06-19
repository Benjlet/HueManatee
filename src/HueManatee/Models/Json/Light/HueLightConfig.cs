using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueLightConfig
    {
        [JsonProperty("archetype")]
        internal string Archetype { get; set; }

        [JsonProperty("function")]
        internal string Function { get; set; }

        [JsonProperty("direction")]
        internal string Direction { get; set; }

        [JsonProperty("startup")]
        internal HueLightStartup Startup { get; set; }
    }
}
