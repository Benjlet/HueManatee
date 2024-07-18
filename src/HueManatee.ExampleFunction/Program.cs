using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HueManatee.ExampleFunction
{
    public class Program
    {
        private const string BridgeHttpClient = nameof(BridgeHttpClient);

        public static async Task Main(string[] args)
        {
            IHost host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<Configuration>();

                    services.AddHttpClient(BridgeHttpClient, (provider, client) =>
                    {
                        Configuration config = provider.GetRequiredService<Configuration>();
                        client.BaseAddress = new Uri(config.HueBridgeIpAddress);
                    });

                    services.AddScoped<IBridgeClient>(provider =>
                    {
                        Configuration config = provider.GetRequiredService<Configuration>();
                        IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

                        return new BridgeClient(
                            httpClientFactory.CreateClient(BridgeHttpClient),
                            config.HueBridgeUserName);
                    });

                    services.AddScoped<IBridgeRegistrationClient>(provider =>
                    {
                        Configuration config = provider.GetRequiredService<Configuration>();
                        IHttpClientFactory httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

                        return new BridgeRegistrationClient(
                            httpClientFactory.CreateClient(BridgeHttpClient));
                    });
                })
                .Build();

            await host.RunAsync();
        }
    }
}
