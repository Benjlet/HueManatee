using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using HueManatee.ExampleFunction;
using HueManatee.Extensions;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace HueManatee.ExampleFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var ipAddress = Environment.GetEnvironmentVariable("HueBridgeIpAddress");
            var ignoreCerts = bool.Parse(Environment.GetEnvironmentVariable("HueBridgeIgnoreCerts") ?? string.Empty);
            var userName = Environment.GetEnvironmentVariable("HueBridgeUserName");

            builder.Services.AddBridgeRegistrationClient(ipAddress, ignoreCerts);
            builder.Services.AddBridgeClient(ipAddress, userName, ignoreCerts);
        }
    }
}
