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
    public class BridgeRegistrationClient
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly BridgeClientMapper _mapper;

        /// <summary>
        /// Initialises a new <see cref="BridgeRegistrationClient"/> instance where calls to the Hue Bridge are managed using a new <see cref="HttpClient"/> using the Bridge IP address.
        /// This is the registration client for the Philips Hue Bridge, generating a UserName.
        /// </summary>
        /// <param name="bridgeIpAddress">The IP address of the Philips Hue Bridge.</param>
        public BridgeRegistrationClient(string bridgeIpAddress)
        {
            if (string.IsNullOrWhiteSpace(bridgeIpAddress))
            {
                throw new ArgumentNullException("The IP address of the Philips Hue Bridge is required. Use 'discovery.meethue' to find this address.");
            }

            _httpClient = new BridgeClientHttpWrapper(new HttpClient()
            {
                BaseAddress = new Uri(bridgeIpAddress)
            });

            _mapper = new BridgeClientMapper();
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
                throw new ArgumentNullException("An HttpClient instance is required.");
            }

            _httpClient = new BridgeClientHttpWrapper(httpClient);
            _mapper = new BridgeClientMapper();
        }

        /// <summary>
        /// Initialises a new <see cref="BridgeRegistrationClient"/> instance where calls to the Hue Bridge are managed using the supplied <see cref="IHttpClientFactory"/>.
        /// This is the registration client for the Philips Hue Bridge, generating a UserName.
        /// </summary>
        /// <param name="httpClientFactory">Factory abstraction for creating HTTP clients; the default HttpClient from this instance will get created.</param>
        /// <param name="namedHttpClient">Creates the HttpClient using the named instance from the factory.</param>
        public BridgeRegistrationClient(IHttpClientFactory httpClientFactory, string namedHttpClient = "")
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException("An HttpClientFactory instance is required.");
            }

            _httpClient = new BridgeClientHttpWrapper(!string.IsNullOrWhiteSpace(namedHttpClient)
                ? httpClientFactory.CreateClient(namedHttpClient)
                : httpClientFactory.CreateClient());

            _mapper = new BridgeClientMapper();
        }

        internal BridgeRegistrationClient(IHttpClientWrapper httpClientHandler)
        {
            _httpClient = httpClientHandler;
            _mapper = new BridgeClientMapper();
        }

        /// <summary>
        /// 
        /// Registers a device reference against the Philips Hue Bridge.
        /// 
        /// The first call may instruct you to press the Link button on the Bridge before re-attempting this same call.
        /// The UserName in the <see cref="RegisterResponse"/> can then be used for calls to the main <see cref="BridgeClient"/>.
        /// 
        /// </summary>
        /// <param name="registerRequest">Registration details for the Hue Bridge.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="BridgeClientException"/>
        /// <returns>A <see cref="RegisterResponse"/> containing a username and/or error details.</returns>
        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest?.DeviceType))
            {
                throw new ArgumentException("DeviceType is required to register a device with the Philips Hue Bridge.");
            }

            var request = _mapper.MapRegisterRequest(registerRequest);
            var registerResponse = await _httpClient.PostAsync<List<HueRegisterResult>>("api", request);
            var result =  _mapper.MapRegisterResponse(registerResponse);

            return result;
        }
    }
}