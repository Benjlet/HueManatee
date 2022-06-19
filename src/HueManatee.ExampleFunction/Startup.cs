using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using HueManatee.ExampleFunction;
using HueManatee.Extensions;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace HueManatee.ExampleFunction
{
    public class Startup : FunctionsStartup
    {
        private const string HueBridgeUserNameConfig = "HueBridgeUserName";
        private const string HueBridgeIpAddressConfig = "HueBridgeIpAddress";
        private const string HueBridgeIgnoreCertsConfig = "HueBridgeIgnoreCerts";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var ipAddress = Environment.GetEnvironmentVariable(HueBridgeIpAddressConfig);
            var ignoreCerts = bool.Parse(Environment.GetEnvironmentVariable(HueBridgeIgnoreCertsConfig) ?? string.Empty);
            var userName = Environment.GetEnvironmentVariable(HueBridgeUserNameConfig);

            builder.Services.AddBridgeClient(ipAddress, userName, ignoreCerts);
        }
    }
}
