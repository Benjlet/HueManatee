using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using HueManatee.ExampleFunction;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace HueManatee.ExampleFunction
{
    public class Startup : FunctionsStartup
    {
        private const string HueBridgeIpAddressConfig = "HueBridgeIpAddress";
        private const string HueBridgeIgnoreCertsConfig = "HueBridgeIgnoreCerts";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var ipAddress = Environment.GetEnvironmentVariable(HueBridgeIpAddressConfig);

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new UriFormatException($"'{HueBridgeIpAddressConfig}' config is missing or empty.");

            var ignoreCerts = bool.Parse(Environment.GetEnvironmentVariable(HueBridgeIgnoreCertsConfig) ?? string.Empty);

            builder.Services.AddBridgeClient(ipAddress, ignoreCerts);
        }
    }
}
