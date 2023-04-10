using HueManatee.Abstractions;
using HueManatee.Exceptions;
using HueManatee.Json;
using HueManatee.Request;
using HueManatee.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HueManatee
{
    /// <summary>
    /// A registration client for the Philips Hue Bridge, generating a UserName.
    /// </summary>
    public class BridgeRegistrationClient : IBridgeRegistrationClient
    {
        private readonly IHttpClientWrapper _httpClient;

        /// <summary>
        /// Initialises a new <see cref="BridgeRegistrationClient"/> instance where calls to the Hue Bridge are managed using a new <see cref="HttpClient"/> using the Bridge IP address.
        /// This is the registration client for the Philips Hue Bridge, generating a UserName.
        /// </summary>
        /// <param name="bridgeIpAddress">The IP address of the Philips Hue Bridge.</param>
        /// <exception cref="ArgumentException"/>
        public BridgeRegistrationClient(string bridgeIpAddress)
        {
            if (string.IsNullOrWhiteSpace(bridgeIpAddress))
            {
                throw new ArgumentException("The IP address of the Philips Hue Bridge is required. Use 'discovery.meethue' to find this address.");
            }

            _httpClient = new BridgeClientHttpWrapper(new HttpClient()
            {
                BaseAddress = new Uri(bridgeIpAddress)
            });
        }

        /// <summary>
        /// Initialises a new <see cref="BridgeRegistrationClient"/> instance where calls to the Hue Bridge are managed using the supplied <see cref="HttpClient"/>.
        /// This is the registration client for the Philips Hue Bridge, generating a UserName.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for calls to the Philips Hue Bridge. The BaseAddress should be set to the Philips Hue Bridge IP address.</param>
        public BridgeRegistrationClient(HttpClient httpClient)
        {
            if (httpClient == null)
            {
                throw new ArgumentException("An HttpClient instance is required.");
            }

            _httpClient = new BridgeClientHttpWrapper(httpClient);
        }

        internal BridgeRegistrationClient(IHttpClientWrapper httpClientHandler)
        {
            _httpClient = httpClientHandler;
        }

        /// <inheritdoc/>
        /// <exception cref="BridgeClientException"/>
        /// <exception cref="ArgumentException"/>
        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest?.DeviceType))
            {
                throw new ArgumentException("DeviceType is required to register a device with the Philips Hue Bridge.");
            }

            var request = BridgeClientMapper.MapRegisterRequest(registerRequest);
            var registerResponse = await _httpClient.PostAsync<List<HueRegisterResult>>("api", request).ConfigureAwait(false);

            return BridgeClientMapper.MapRegisterResponse(registerResponse);
        }
    }
}