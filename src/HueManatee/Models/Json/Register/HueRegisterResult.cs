using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueRegisterResult
    {
        [JsonProperty("error")]
        internal HueError Error { get; set; }

        [JsonProperty("success")]
        internal HueRegisterSuccess Success { get; set; }
    }
}