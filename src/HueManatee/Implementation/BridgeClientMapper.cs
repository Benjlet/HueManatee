using HueManatee.Response;
using HueManatee.Request;
using System.Collections.Generic;
using System.Linq;
using HueManatee.Json;
using HueManatee.Exceptions;

namespace HueManatee
{
    internal class BridgeClientMapper
    {
        internal static ChangeLightResponse MapStateResponse(List<HueLightUpdateResult> response)
        {
            var successMessages = new Dictionary<string, string>();

            foreach (var successResponses in response?.Where(d => d?.Success != null))
            {
                var successKey = successResponses.Success.Keys?.FirstOrDefault();
                var successValue = successResponses.Success.Values?.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(successKey) && !string.IsNullOrWhiteSpace(successValue))
                {
                    successMessages.Add(successKey, successValue);
                }
            }

            return new ChangeLightResponse()
            {
                Changes = successMessages,
                Errors = response?.Where(d => !string.IsNullOrWhiteSpace(d?.Error?.Description))?.Select(d => d.Error.Description)
            };
        }

        internal static HueStateRequest MapStateRequest(ChangeLightRequest state)
        {
            if (state == null)
            {
                throw new BridgeClientException("State change request is null.");
            }

            var request = new HueStateRequest()
            {
                ColorTemperature = state.ColorTemperature,
                Brightness = state.Brightness,
                Saturation = state.Saturation,
                Effect = state.Effect?.ToString()?.ToLower(),
                Hue = state.Hue,
                On = state.On
            };

            if (state.Color != null)
            {
                var rgb = new RgbColor(state.Color.Value);

                request.Hue ??= rgb.GetHue();
                request.Saturation ??= rgb.GetSaturation();
                request.Brightness ??= rgb.GetBrightness();
            }

            return request;
        }

        internal static Group MapGroupResponse(string key, HueGroup group) => new()
        {
            Id = key,
            Name = group.Name,
            Lights = group.Lights,
            Sensors = group.Sensors,
            Type = group.Type
        };

        internal static IEnumerable<Group> MapGroupsResponse(Dictionary<string, HueGroup> response) =>
            response?.Select(group => MapGroupResponse(group.Key, group.Value));

        internal static IEnumerable<Light> MapLightsResponse(Dictionary<string, HueLight> response) => 
            response?.Select(light => MapLightResponse(light.Key, light.Value));

        internal static Light MapLightResponse(string key, HueLight light)
        {
            if (light == null)
            {
                return new()
                {
                    Id = key
                };
            }

            return new()
            {
                Id = key,
                Name = light.Name,
                ModelId = light.ModelId,
                ProductName = light.ProductName,
                Manufacturer = light.ManufacturerName,
                State = light.State != null ? new LightState()
                {
                    ColorTemperature = light.State.ColorTemperature,
                    Brightness = light.State.Brightness,
                    Saturation = light.State.Sat,
                    Effect = light.State.Effect,
                    Hue = light.State.Hue,
                    On = light.State.On,
                    X = light.State.XY?.FirstOrDefault(),
                    Y = light.State.XY?.LastOrDefault()
                } : null
            };
        }

        internal static HueRegisterRequest MapRegisterRequest(RegisterRequest request) => new()
        {
            DeviceType = request.DeviceType
        };

        internal static RegisterResponse MapRegisterResponse(List<HueRegisterResult> response) => new()
        {
            UserName = response?.FirstOrDefault(d => d?.Success != null)?.Success?.UserName,
            Errors = response?.Where(d => !string.IsNullOrWhiteSpace(d?.Error?.Description))?.Select(d => d.Error.Description)
        };
    }
}
