using Newtonsoft.Json;

namespace HueManatee.Json
{
    internal class HueRegisterSuccess
    {
        [JsonProperty("username")]
        internal string UserName { get; set; }
    }
}