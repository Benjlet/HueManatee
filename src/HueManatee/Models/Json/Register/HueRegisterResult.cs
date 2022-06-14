using Newtonsoft.Json;

namespace HueManatee
{
    internal class HueRegisterResult
    {
        [JsonProperty("error")]
        internal HueError Error { get; set; }

        [JsonProperty("success")]
        internal HueRegisterSuccess Success { get; set; }
    }
}