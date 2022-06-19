using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueRegisterRequest
    {
        [JsonProperty("devicetype")]
        internal string DeviceType { get; set; }
    }
}