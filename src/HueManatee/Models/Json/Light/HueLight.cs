using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueLight
    {
        [JsonProperty("state")]
        internal HueLightState State { get; set; }

        [JsonProperty("type")]
        internal string Type { get; set; }

        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("modelid")]
        internal string ModelId { get; set; }

        [JsonProperty("manufacturername")]
        internal string ManufacturerName { get; set; }

        [JsonProperty("productname")]
        internal string ProductName { get; set; }

        [JsonProperty("capabilities")]
        internal HueLightCapabilities Capabilities { get; set; }

        [JsonProperty("config")]
        internal HueLightConfig Config { get; set; }

        [JsonProperty("uniqueid")]
        internal string UniqueId { get; set; }

        [JsonProperty("swversion")]
        internal string SoftwareVersion { get; set; }

        [JsonProperty("swconfigid")]
        internal string SoftwareConfigId { get; set; }

        [JsonProperty("swupdate")]
        internal HueLightSoftwareUpdate SoftwareUpdate { get; set; }

        [JsonProperty("productid")]
        internal string ProductId { get; set; }
    }
}
