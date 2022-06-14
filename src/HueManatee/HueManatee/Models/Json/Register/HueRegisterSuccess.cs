using Newtonsoft.Json;

namespace HueManatee
{
    internal class HueRegisterSuccess
    {
        [JsonProperty("username")]
        internal string UserName { get; set; }
    }
}