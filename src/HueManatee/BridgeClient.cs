using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Drawing;

namespace HueManatee
{
    /// <summary>
    /// The main BridgeClient for integration with the Philips Hue Bridge.
    /// </summary>
    public class BridgeClient
    {
        private readonly IHttpClientWrapper _httpClient;

        /// <summary>
        /// Initialises a new <see cref="BridgeClient"/> instance where an <see cref="HttpClient"/> instance is created from the supplied <see cref="IHttpClientFactory"/>.
        /// This is the main BridgeClient client for integration with the Philips Hue Bridge.
        /// </summary>
        /// <param name="httpClientFactory">The client factory to create an <see cref="HttpClient"/> from.</param>
        public BridgeClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = new HttpClientWrapper(httpClientFactory.CreateClient());
        }

        /// <summary>
        /// Initialises a new <see cref="BridgeClient"/> instance where calls to the Hue Bridge are managed using the supplied <see cref="HttpClient"/>.
        /// This is the main BridgeClient client for integration with the Philips Hue Bridge.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for calls to the Philips Hue Bridge.</param>
        public BridgeClient(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
        }

        internal BridgeClient(IHttpClientWrapper httpClientHandler)
        {
            _httpClient = httpClientHandler;
        }

        /// <summary>
        /// Gets <see cref="Light"/> details of all lights visible to the user.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns>A collection of Philips Hue <see cref="Light"/> details.</returns>
        public async Task<IEnumerable<Light>> GetLights(string userName)
        {
            var response = await _httpClient.GetAsync<Dictionary<string, HueLight>>($"api/{userName}/lights");
            return response?.Select(light => MapToLight(light.Key, light.Value));
        }

        /// <summary>
        /// Gets <see cref="Light"/> details of a specific light that is visible to the user.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="id">The ID of the light to turn off.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns>Philips Hue <see cref="Light"/> details.</returns>
        public async Task<Light> GetLightData(string userName, string id)
        {
            var response = await _httpClient.GetAsync<HueLight>($"api/{userName}/lights/{id}");
            return MapToLight(id, response);
        }

        /// <summary>
        /// Turns the light (linked with the <paramref name="id"/>) off.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="id">The ID of the light to turn off.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns><see cref="LightChangeResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<LightChangeResponse> TurnLightOff(string userName, string id)
        {
            return await ChangeLightState(userName, id, new LightChangeRequest()
            {
                On = false
            });
        }

        /// <summary>
        /// Turns the light (linked with the <paramref name="id"/>) on. 
        /// Adjusts no other values, so may not look very 'on' if set to a low brightness.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="id">The ID of the light to turn on.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns><see cref="LightChangeResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<LightChangeResponse> TurnLightOn(string userName, string id)
        {
            return await ChangeLightState(userName, id, new LightChangeRequest()
            {
                On = true
            });
        }

        /// <summary>
        /// Starts a light color loop for the target light at its current hue and saturation. The light will be turned on, if it is not already.
        /// Use the 'ChangeLightState' call if you require for more specific control.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="id">The ID of the light to change.</param>
        /// <returns><see cref="LightChangeResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<LightChangeResponse> StartColorLoop(string userName, string id)
        {
            return await ChangeLightState(userName, id, new LightChangeRequest()
            {
                On = true,
                Effect = LightEffect.ColorLoop
            });
        }

        /// <summary>
        /// Stops a light color loop for the target light by setting its color loop value to 'none'.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="id">The ID of the light to change.</param>
        /// <returns><see cref="LightChangeResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<LightChangeResponse> StopColorLoop(string userName, string id)
        {
            return await ChangeLightState(userName, id, new LightChangeRequest()
            {
                Effect = LightEffect.None
            });
        }

        /// <summary>
        /// Changes the light to the supplied color. The light will be switched on, if not already.
        /// Supplying a brightness value will overwrite the auto-calculated brightness for achieving the color.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="id">The ID of the light to change.</param>
        /// <param name="color">The requested color of the light.</param>
        /// <param name="brightness">The requested brightness of the light.</param>
        /// <returns><see cref="LightChangeResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<LightChangeResponse> ChangeLightColor(string userName, string id, Color color, int? brightness = null)
        {
            return await ChangeLightState(userName, id, new LightChangeRequest()
            {
                On = true,
                Color = color,
                Brightness = brightness
            });
        }

        /// <summary>
        /// Changes the state of the light, such as color or brightness.
        /// Supplying a Color in the <paramref name="state"/> will automatically set the Hue, Saturation, and Brightness to achieve the color.
        /// Supplying Brightness in the <paramref name="state"/> will override any calculated Brightness values.
        /// </summary>
        /// <param name="userName">A registered user to the Philips Hue Bridge.</param>
        /// <param name="id">The ID of the light to change.</param>
        /// <param name="state">Updates to make to the light state, such as increasing brightness.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns><see cref="LightChangeResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        public async Task<LightChangeResponse> ChangeLightState(string userName, string id, LightChangeRequest state)
        {
            var request = new HueStateRequest()
            {
                ColorTemperature = state.ColorTemperature,
                Brightness = state.Brightness,
                Saturation = state.Saturation,
                Effect = state.Effect != null ? ((LightEffect)state.Effect).ToString().ToLower() : null,
                Hue = state.Hue,
                On = state.On
            };

            if (state.Color != null)
            {
                var rgb = new RGB(state.Color.Value);

                request.Hue = rgb.GetHue();
                request.Saturation = rgb.GetSaturation();
                request.Brightness ??= rgb.GetBrightness();
            }

            var response = await _httpClient.PutAsync<List<HueLightUpdateResult>>($"api/{userName}/lights/{id}/state", request);

            var successMessages = new Dictionary<string, string>();

            foreach (var success in response?.Where(d => d?.Success != null)?.Select(d => d?.Success))
                successMessages.Add(success?.Keys?.FirstOrDefault(), success?.Values?.FirstOrDefault());

            return new LightChangeResponse()
            {
                Changes = successMessages,
                Errors = response?.Where(d => d?.Error != null)?.Select(d => d?.Error?.Description)?.ToList()
            };
        }

        /// <summary>
        /// Registers a device reference against the Philips Hue Bridge.
        /// The first call may instruct you to press the Link button on the Bridge before re-attempting this same call.
        /// The UserName in the <see cref="RegisterResponse"/> can then be used for other calls to the Hue Bridge that require a UserName.
        /// </summary>
        /// <param name="request">Registration details for the Hue Bridge.</param>
        /// <exception cref="JsonException"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="TaskCanceledException"/>
        /// <returns>A <see cref="RegisterResponse"/> containing a username and/or error details.</returns>
        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            var response = await _httpClient.PostAsync<List<HueRegisterResult>>("api", new HueRegisterRequest()
            {
                DeviceType = request.DeviceType
            });

            return new RegisterResponse()
            {
                UserName = response?.FirstOrDefault(d => d?.Success != null)?.Success?.UserName,
                Errors = response?.Where(d => d?.Error != null)?.Select(d => d?.Error?.Description)?.ToList()
            };
        }

        private Light MapToLight(string key, HueLight light) => new()
        {
            Id = key,
            Name = light.Name,
            ModelId = light.ModelId,
            ProductName = light.ProductName,
            Manufacturer = light.ManufacturerName,
            State = light.State == null ? null : new LightState()
            {
                ColorTemperature = light.State.ColorTemperature,
                Brightness = light.State.Brightness,
                Saturation = light.State.Sat,
                Hue = light.State.Hue,
                On = light.State.On,
                X = light.State.XY?.FirstOrDefault(),
                Y = light.State.XY?.LastOrDefault()
            }
        };
    }
}