using Newtonsoft.Json;
using System;

namespace HueManatee.Json
{
    internal class HueLightSoftwareUpdate
    {
        [JsonProperty("state")]
        internal string State { get; set; }

        [JsonProperty("lastinstall")]
        internal DateTime LastInstall { get; set; }
    }
}
