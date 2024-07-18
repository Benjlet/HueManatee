using Microsoft.Extensions.Configuration;

namespace HueManatee.ExampleFunction
{
    public class Configuration
    {
        private readonly IConfiguration _configuration;

        public string HueBridgeIpAddress => _configuration["HUE_BRIDGE_IP_ADDRESS"];
        public string HueBridgeUserName => _configuration["HUE_BRIDGE_USERNAME"];

        public Configuration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
