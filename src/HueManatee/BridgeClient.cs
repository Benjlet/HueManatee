using HueManatee.Abstractions;
using HueManatee.Json;
using HueManatee.Request;
using HueManatee.Response;
using Newtonsoft.Json;
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
        private string _userName;
        private readonly IHttpClientWrapper _httpClient;
        private readonly BridgeClientMapper _mapper;

        /// <summary>
        /// Initialises a new <see cref="BridgeClient"/> instance where calls to the Hue Bridge are managed using the supplied <see cref="HttpClient"/>.
        /// This is the main BridgeClient client for integration with the Philips Hue Bridge.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for calls to the Philips Hue Bridge.</param>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        public BridgeClient(HttpClient httpClient, string userName = null)
        {
            _httpClient = new HttpClientWrapper(httpClient);
            _mapper = new BridgeClientMapper();
            _userName = userName;
        }

        internal BridgeClient(IHttpClientWrapper httpClientHandler)
        {
            _httpClient = httpClientHandler;
            _mapper = new BridgeClientMapper();
        }

        /// <summary>
        /// Sets the username to use for light calls that require a valid user.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        public void SetUserName(string userName)
        {
            _userName = userName;
        }

        /// <summary>
        /// Gets <see cref="Light"/> details of all lights visible to the user.
        /// </summary>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns>A collection of Philips Hue <see cref="Light"/> details.</returns>
        public async Task<IEnumerable<Light>> GetLightData()
        {
            ValidateUserName();
            var response = await _httpClient.GetAsync<Dictionary<string, HueLight>>($"api/{_userName}/lights");
            return _mapper.MapLightResponse(response);
        }

        /// <summary>
        /// Gets <see cref="Light"/> details of a specific light that is visible to the user.
        /// </summary>
        /// <param name="id">The ID of the light being queried.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns>Philips Hue <see cref="Light"/> details.</returns>
        public async Task<Light> GetLightData(string id)
        {
            ValidateUserName();
            var response = await _httpClient.GetAsync<HueLight>($"api/{_userName}/lights/{id}");
            return _mapper.MapLightResponse(id, response);
        }

        /// <summary>
        /// Gets <see cref="Group"/> details of all light groups visible to the user.
        /// </summary>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns>A collection of light <see cref="Group"/> details.</returns>
        public async Task<IEnumerable<Group>> GetGroupData()
        {
            ValidateUserName();
            var response = await _httpClient.GetAsync<Dictionary<string, HueGroup>>($"api/{_userName}/groups/");
            return _mapper.MapGroupResponse(response);
        }

        /// <summary>
        /// Gets <see cref="Group"/> details of a specific group that is visible to the user.
        /// </summary>
        /// <param name="id">The ID of the group being queried.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns>Light <see cref="Group"/> details.</returns>
        public async Task<Group> GetGroupData(string id)
        {
            ValidateUserName();
            var response = await _httpClient.GetAsync<HueGroup>($"api/{_userName}/groups/{id}");
            return _mapper.MapGroupResponse(id, response);
        }

        /// <summary>
        /// Changes the state of the light, such as color or brightness.
        /// Supplying a Color in the <paramref name="state"/> will automatically set the Hue, Saturation, and Brightness to achieve the color.
        /// Supplying Brightness in the <paramref name="state"/> will overwrite any calculated Brightness values.
        /// </summary>
        /// <param name="id">The ID of the light to change.</param>
        /// <param name="state">Updates to make to the light state, such as increasing brightness.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns><see cref="ChangeLightResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<ChangeLightResponse> ChangeLight(string id, ChangeLightRequest state)
        {
            ValidateUserName();
            var request = _mapper.MapStateRequest(state);
            var response = await _httpClient.PutAsync<List<HueLightUpdateResult>>($"api/{_userName}/lights/{id}/state", request);
            return _mapper.MapStateResponse(response);
        }

        /// <summary>
        /// Changes the state of the light group, such as the color or brightness of the lights in the group.
        /// Supplying a Color in the <paramref name="state"/> will automatically set the Hue, Saturation, and Brightness to achieve the color.
        /// Supplying Brightness in the <paramref name="state"/> will overwrite any calculated Brightness values.
        /// </summary>
        /// <param name="groupId">The ID of the light group.</param>
        /// <param name="state">Updates to make to the state of all lights in the group, such as increasing brightness.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns><see cref="ChangeLightResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<ChangeLightResponse> ChangeLightGroup(string groupId, ChangeLightRequest state)
        {
            ValidateUserName();
            var request = _mapper.MapStateRequest(state);
            var response = await _httpClient.PutAsync<List<HueLightUpdateResult>>($"api/{_userName}/groups/{groupId}/action", request);
            return _mapper.MapStateResponse(response);
        }

        /// <summary>
        /// Registers a device reference against the Philips Hue Bridge.
        /// The first call may instruct you to press the Link button on the Bridge before re-attempting this same call.
        /// The UserName in the <see cref="RegisterResponse"/> can then be used for other calls to the Hue Bridge that require a UserName.
        /// </summary>
        /// <param name="registerRequest">Registration details for the Hue Bridge.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns>A <see cref="RegisterResponse"/> containing a username and/or error details.</returns>
        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            var request = _mapper.MapRegisterRequest(registerRequest);
            var response = await _httpClient.PostAsync<List<HueRegisterResult>>("api", request);
            return _mapper.MapRegisterResponse(response);
        }

        private void ValidateUserName()
        {
            if (string.IsNullOrWhiteSpace(_userName))
                throw new UnauthorizedAccessException($"UserName has not been set. Provide this value via SetUserName or during initialisation.");
        }
    }
}