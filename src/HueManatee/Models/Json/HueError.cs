using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueError
    {
        [JsonProperty("type")]
        internal int Type { get; set; }

        [JsonProperty("address")]
        internal string Address { get; set; }

        [JsonProperty("description")]
        internal string Description { get; set; }
    }
}