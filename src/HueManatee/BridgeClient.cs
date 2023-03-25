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
    /// The main BridgeClient for integration with the Philips Hue Bridge.
    /// </summary>
    public class BridgeClient
    {
        private readonly IHttpClientWrapper _httpClient;
        private readonly BridgeClientMapper _mapper;
        private readonly string _userName;

        /// <summary>
        /// Initialises a new <see cref="BridgeClient"/> instance where calls to the Hue Bridge are managed using a new <see cref="HttpClient"/> using the Bridge IP address.
        /// This is the main BridgeClient client for integration with the Philips Hue Bridge.
        /// </summary>
        /// <param name="bridgeIpAddress">The IP address of the Philips Hue Bridge.</param>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <exception cref="ArgumentException"/>
        public BridgeClient(string bridgeIpAddress, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("A registered UserName is required - you can generate this using the online 'developers.meethue' guide or via the BridgeRegistrationClient.");
            }

            if (string.IsNullOrWhiteSpace(bridgeIpAddress))
            {
                throw new ArgumentException("The IP address of the Philips Hue Bridge is required. Use 'discovery.meethue' to find this address.");
            }

            _httpClient = new BridgeClientHttpWrapper(new HttpClient()
            {
                BaseAddress = new Uri(bridgeIpAddress)
            });

            _mapper = new BridgeClientMapper();
            _userName = userName;
        }

        /// <summary>
        /// Initialises a new <see cref="BridgeClient"/> instance where calls to the Hue Bridge are managed using the supplied <see cref="HttpClient"/>.
        /// This is the main BridgeClient client for integration with the Philips Hue Bridge.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for calls to the Philips Hue Bridge. The BaseAddress should be set to the Philips Hue Bridge IP address.</param>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <exception cref="ArgumentException"/>
        public BridgeClient(HttpClient httpClient, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("A registered UserName is required - you can generate this using the online 'developers.meethue' guide or via the BridgeClientRegistration client.");
            }

            if (httpClient == null)
            {
                throw new ArgumentException("An HttpClient instance is required.");
            }

            _httpClient = new BridgeClientHttpWrapper(httpClient);
            _mapper = new BridgeClientMapper();
            _userName = userName;
        }

        /// <summary>
        /// Initialises a new <see cref="BridgeClient"/> instance where calls to the Hue Bridge are managed using the supplied <see cref="IHttpClientFactory"/>.
        /// This is the main BridgeClient client for integration with the Philips Hue Bridge.
        /// </summary>
        /// <param name="httpClientFactory">Factory abstraction for creating HTTP clients; the default HttpClient from this instance will get created.</param>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="namedHttpClient">Creates the HttpClient using the named instance from the factory.</param>
        /// <exception cref="ArgumentException"/>
        public BridgeClient(IHttpClientFactory httpClientFactory, string userName, string namedHttpClient = "")
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("A registered UserName is required - you can generate this using the online 'developers.meethue' guide or via the BridgeClientRegistration client.");
            }

            if (httpClientFactory == null)
            {
                throw new ArgumentException("An HttpClientFactory instance is required.");
            }

            _httpClient = new BridgeClientHttpWrapper(!string.IsNullOrWhiteSpace(namedHttpClient)
                ? httpClientFactory.CreateClient(namedHttpClient)
                : httpClientFactory.CreateClient());

            _mapper = new BridgeClientMapper();
            _userName = userName;
        }

        internal BridgeClient(IHttpClientWrapper httpClientHandler)
        {
            _httpClient = httpClientHandler;
            _mapper = new BridgeClientMapper();
        }

        /// <summary>
        /// Gets <see cref="Light"/> details of all lights visible to the user.
        /// </summary>
        /// <exception cref="BridgeClientException"/>
        /// <returns>A collection of Philips Hue <see cref="Light"/> details.</returns>
        public async Task<IEnumerable<Light>> GetLightData()
        {
            var response = await _httpClient.GetAsync<Dictionary<string, HueLight>>($"api/{_userName}/lights").ConfigureAwait(false);

            return _mapper.MapLightsResponse(response);
        }

        /// <summary>
        /// Gets <see cref="Light"/> details of a specific light that is visible to the user.
        /// </summary>
        /// <param name="id">The ID of the light being queried.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="BridgeClientException"/>
        /// <returns>Philips Hue <see cref="Light"/> details.</returns>
        public async Task<Light> GetLightData(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Light ID is required to get specific light data. Get all light details to identify individual IDs.");
            }

            var response = await _httpClient.GetAsync<HueLight>($"api/{_userName}/lights/{id}").ConfigureAwait(false);

            return _mapper.MapLightResponse(id, response);
        }

        /// <summary>
        /// Gets <see cref="Group"/> details of all groups visible to the user.
        /// </summary>
        /// <exception cref="BridgeClientException"/>
        /// <returns>A collection of <see cref="Group"/> details.</returns>
        public async Task<IEnumerable<Group>> GetGroupData()
        {
            var response = await _httpClient.GetAsync<Dictionary<string, HueGroup>>($"api/{_userName}/groups/").ConfigureAwait(false);

            return _mapper.MapGroupsResponse(response);
        }

        /// <summary>
        /// Gets <see cref="Group"/> details of a specific group that is visible to the user.
        /// </summary>
        /// <param name="id">The ID of the group being queried.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="BridgeClientException"/>
        /// <returns>Specific <see cref="Group"/> details.</returns>
        public async Task<Group> GetGroupData(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Group ID is required to get specific group data. Get all group details to identify individual IDs.");
            }

            var response = await _httpClient.GetAsync<HueGroup>($"api/{_userName}/groups/{id}").ConfigureAwait(false);

            return _mapper.MapGroupResponse(id, response);
        }

        /// <summary>
        /// 
        /// Changes the state of the light, such as color or brightness.
        /// 
        /// Supplying a Color in the <paramref name="state"/> will automatically set the Hue, Saturation, and Brightness to achieve the color.
        /// Supplying Brightness in the <paramref name="state"/> will overwrite any calculated Brightness values.
        /// 
        /// </summary>
        /// <param name="id">The ID of the light to change.</param>
        /// <param name="state">Updates to make to the light state, such as increasing brightness.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="BridgeClientException"/>
        /// <returns><see cref="ChangeLightResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<ChangeLightResponse> ChangeLight(string id, ChangeLightRequest state)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Light ID is required to change the state of a specific light. Get all light details to identify individual IDs.");
            }

            var request = _mapper.MapStateRequest(state);
            var response = await _httpClient.PutAsync<List<HueLightUpdateResult>>($"api/{_userName}/lights/{id}/state", request).ConfigureAwait(false);

            return _mapper.MapStateResponse(response);
        }

        /// <summary>
        /// 
        /// Changes the state of the light group, such as the color or brightness of the lights in the group.
        /// 
        /// Supplying a Color in the <paramref name="state"/> will automatically set the Hue, Saturation, and Brightness to achieve the color.
        /// Supplying Brightness in the <paramref name="state"/> will overwrite any calculated Brightness values.
        /// 
        /// </summary>
        /// <param name="id">The ID of the light group.</param>
        /// <param name="state">Updates to make to the state of all lights in the group, such as increasing brightness.</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="BridgeClientException"/>
        /// <returns><see cref="ChangeLightResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<ChangeLightResponse> ChangeGroup(string id, ChangeLightRequest state)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Group ID is required to change the state of a specific group. Get all group details to identify individual IDs.");
            }

            var request = _mapper.MapStateRequest(state);
            var response = await _httpClient.PutAsync<List<HueLightUpdateResult>>($"api/{_userName}/groups/{id}/action", request).ConfigureAwait(false);

            return _mapper.MapStateResponse(response);
        }
    }
}