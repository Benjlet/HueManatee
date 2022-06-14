using Newtonsoft.Json;
using System.Collections.Generic;

namespace HueManatee
{
    internal class HueLightControl
    {
        [JsonProperty("mindimlevel")]
        internal int MinimumDimLevel { get; set; }

        [JsonProperty("maxlumen")]
        internal int MaximumLumen { get; set; }

        [JsonProperty("colorgamuttype")]
        internal string ColorGamutType { get; set; }

        [JsonProperty("colorgamut")]
        internal List<List<double>> ColorGamut { get; set; }

        [JsonProperty("ct")]
        internal HueLightColorTemperature ColorTemperature { get; set; }
    }
}
