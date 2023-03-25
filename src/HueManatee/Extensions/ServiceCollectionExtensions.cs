using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace HueManatee.Extensions
{
    /// <summary>
    /// Extensions for the HueManatee library.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="BridgeClient"/> client the service collection, the base address set to the supplied <paramref name="ipAddress"/>.
        /// Certificate validation can be ignored if <paramref name="disableCertificateValidation"/> is set to <see langword="true"/>.
        /// </summary>
        /// <param name="services">The service collection to add the <see cref="BridgeClient"/> client to.</param>
        /// <param name="ipAddress">The IP address for your Philips Hue Bridge.</param>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="disableCertificateValidation"><see langword="false"/> by default. Set to <see langword="true"/> if certificate errors should be ignored.</param>
        public static void AddBridgeClient(this IServiceCollection services, string ipAddress, string userName, bool disableCertificateValidation = false)
        {
            services.AddHttpClient("BridgeHttpClient", c =>
            {
                c.BaseAddress = new Uri(ipAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

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

            services.AddScoped(c =>
            {
                var clientFactory = c.GetRequiredService<IHttpClientFactory>();
                return new BridgeClient(clientFactory, userName, "BridgeHttpClient");
            });
        }

        /// <summary>
        /// Adds the <see cref="BridgeRegistrationClient"/> client the service collection, the base address set to the supplied <paramref name="ipAddress"/>.
        /// Certificate validation can be ignored if <paramref name="disableCertificateValidation"/> is set to <see langword="true"/>.
        /// </summary>
        /// <param name="services">The service collection to add the <see cref="BridgeRegistrationClient"/> client to.</param>
        /// <param name="ipAddress">The IP address for your Philips Hue Bridge.</param>
        /// <param name="disableCertificateValidation"><see langword="false"/> by default. Set to <see langword="true"/> if certificate errors should be ignored.</param>
        public static void AddBridgeRegistrationClient(this IServiceCollection services, string ipAddress, bool disableCertificateValidation = false)
        {
            services.AddHttpClient("BridgeRegistrationHttpClient", c =>
            {
                c.BaseAddress = new Uri(ipAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

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

            services.AddScoped(c =>
            {
                var clientFactory = c.GetRequiredService<IHttpClientFactory>();
                return new BridgeRegistrationClient(clientFactory, "BridgeRegistrationHttpClient");
            });
        }
    }
}