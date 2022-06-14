using Newtonsoft.Json;

namespace HueManatee
{
    internal class HueRegisterRequest
    {
        [JsonProperty("devicetype")]
        internal string DeviceType { get; set; }
    }
}