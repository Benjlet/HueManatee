using Newtonsoft.Json;
using System.Collections.Generic;

namespace HueManatee.Json
{
    internal class HueLightUpdateResult
    {
        [JsonProperty("error")]
        internal HueError Error { get; set; }

        [JsonProperty("success")]
        internal Dictionary<string, string> Success { get; set; }
    }
}
