using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using HueManatee.ExampleFunction;
using System;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace HueManatee.ExampleFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var ipAddress = Environment.GetEnvironmentVariable("HueBridgeIpAddress");
            var userName = Environment.GetEnvironmentVariable("HueBridgeUserName");

            builder.Services.AddScoped<IBridgeClient, BridgeClient>((configure) => new BridgeClient(ipAddress, userName));
        }
    }
}
