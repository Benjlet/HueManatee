using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace HueManatee
{
    /// <summary>
    /// Extensions for the HueManatee library.
    /// </summary>
    public static class HueManateeExtensions
    {
        /// <summary>
        /// Adds the <see cref="BridgeClient"/> client the service collection, the base address set to the supplied <paramref name="ipAddress"/>.
        /// Certificate validation can be ignored if <paramref name="disableCertificateValidation"/> is set to <see langword="true"/>.
        /// </summary>
        /// <param name="services">The service collection to add the <see cref="BridgeClient"/> client to.</param>
        /// <param name="ipAddress">The IP address for your Philips Hue Bridge.</param>
        /// <param name="disableCertificateValidation"><see langword="false"/> by default. Set to <see langword="true"/> if certificate errors should be ignored.</param>
        public static void AddBridgeClient(this IServiceCollection services, string ipAddress, bool disableCertificateValidation = false)
        {
            services.AddHttpClient<BridgeClient>("", c =>
            {
                c.BaseAddress = new Uri(ipAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new System.Net.Http.HttpClientHandler();
                
                if (disableCertificateValidation)
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) =>
                    {
                        return true;
                    };
                };
                
                return handler;
            });

            services.AddSingleton<BridgeClient>();
        }

        internal static bool ApproximatelyEquals(this double a, double b) => Math.Abs(a - b) <= float.Epsilon;
    }
}