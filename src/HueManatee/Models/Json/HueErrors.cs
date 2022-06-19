using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueErrors
    {
        [JsonProperty("error")]
        public HueError Error { get; set; }
    }
}
