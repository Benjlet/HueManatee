using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HueManatee
{
    /// <summary>
    /// The main HueManatee client for integration with the Philips Hue Bridge.
    /// </summary>
    public class HueManatee
    {
        private readonly HueManateeClient _service;

        /// <summary>
        /// Initialises a new <see cref="HueManatee"/> instance where an <see cref="HttpClient"/> instance is created from the supplied <see cref="IHttpClientFactory"/>.
        /// This is the main HueManatee client for integration with the Philips Hue Bridge.
        /// </summary>
        /// <param name="httpClientFactory">The client factory to create an <see cref="HttpClient"/> from.</param>
        public HueManatee(IHttpClientFactory httpClientFactory)
        {
            _service = new HueManateeClient(httpClientFactory.CreateClient());
        }

        /// <summary>
        /// Initialises a new <see cref="HueManatee"/> instance where calls to the Hue Bridge are managed using the supplied <see cref="HttpClient"/>.
        /// This is the main HueManatee client for integration with the Philips Hue Bridge.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use for calls to the Philips Hue Bridge.</param>
        public HueManatee(HttpClient httpClient)
        {
            _service = new HueManateeClient(httpClient);
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
            var response = await _service.Get<Dictionary<string, HueLight>>($"api/{userName}/lights");
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
            var response = await _service.Get<HueLight>($"api/{userName}/lights/{id}");
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
        /// <returns></returns>
        public async Task<LightChangeResponse> TurnLightOff(string userName, string id)
        {
            return await ChangeLightState(userName, id, new LightStateRequest()
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
        /// <returns></returns>
        public async Task<LightChangeResponse> TurnLightOn(string userName, string id)
        {
            return await ChangeLightState(userName, id, new LightStateRequest()
            {
                On = true
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
        /// <returns></returns>
        public async Task<LightChangeResponse> ChangeLightState(string userName, string id, LightStateRequest state)
        {
            var request = new HueStateRequest()
            {
                ColorTemperature = state.ColorTemperature,
                Brightness = state.Brightness,
                Saturation = state.Saturation,
                Effect = state.Effect,
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

            var response = await _service.Put<List<HueLightUpdateResult>>($"api/{userName}/lights/{id}/state", request);

            return new LightChangeResponse()
            {
                Changes = response?.FirstOrDefault(d => d?.Success != null)?.Success?.ToDictionary(s => s.Key, s => s.Value),
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
            var response = await _service.Post<List<HueRegisterResult>>("api", new HueRegisterRequest()
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