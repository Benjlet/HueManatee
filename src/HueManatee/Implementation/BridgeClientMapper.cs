using HueManatee.Response;
using HueManatee.Request;
using System.Collections.Generic;
using System.Linq;
using HueManatee.Json;

namespace HueManatee
{
    internal class BridgeClientMapper
    {
        internal ChangeLightResponse MapStateResponse(List<HueLightUpdateResult> response)
        {
            var successMessages = new Dictionary<string, string>();

            foreach (var success in response?.Where(d => d?.Success != null)?.Select(d => d?.Success))
                successMessages.Add(success?.Keys?.FirstOrDefault(), success?.Values?.FirstOrDefault());

            return new ChangeLightResponse()
            {
                Changes = successMessages,
                Errors = response?.Where(d => d?.Error != null)?.Select(d => d?.Error?.Description)?.ToList()
            };
        }

        internal HueStateRequest MapStateRequest(ChangeLightRequest state)
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
                var rgb = new RgbColor(state.Color.Value);

                request.Hue = rgb.GetHue();
                request.Saturation = rgb.GetSaturation();
                request.Brightness ??= rgb.GetBrightness();
            }

            return request;
        }

        internal IEnumerable<Group> MapGroupResponse(Dictionary<string, HueGroup> response) => 
            response?.Select(group => MapGroupResponse(group.Key, group.Value));

        internal Group MapGroupResponse(string key, HueGroup group) => new()
        {
            Id = key,
            Name = group.Name,
            Lights = group.Lights,
            Sensors = group.Sensors,
            Type = group.Type
        };

        internal IEnumerable<Light> MapLightResponse(Dictionary<string, HueLight> response) => 
            response?.Select(light => MapLightResponse(light.Key, light.Value));

        internal Light MapLightResponse(string key, HueLight light) => new()
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
                Effect = light.State.Effect,
                Hue = light.State.Hue,
                On = light.State.On,
                X = light.State.XY?.FirstOrDefault(),
                Y = light.State.XY?.LastOrDefault()
            }
        };

        internal HueRegisterRequest MapRegisterRequest(RegisterRequest request) => new()
        {
            DeviceType = request.DeviceType
        };

        internal RegisterResponse MapRegisterResponse(List<HueRegisterResult> response)
        {
            return new RegisterResponse()
            {
                UserName = response?.FirstOrDefault(d => d?.Success != null)?.Success?.UserName,
                Errors = response?.Where(d => d?.Error != null)?.Select(d => d?.Error?.Description)?.ToList()
            };
        }
    }
}
