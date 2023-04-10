using HueManatee.Request;
using HueManatee.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HueManatee
{
    /// <summary>
    /// The main client for integration with the Philips Hue Bridge.
    /// </summary>
    public interface IBridgeClient
    {
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
        /// <returns><see cref="ChangeLightResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        Task<ChangeLightResponse> ChangeGroup(string id, ChangeLightRequest state);

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
        /// <returns><see cref="ChangeLightResponse"/>: the acknowledgement and/or error message(s) returned from the Hue Bridge.</returns>
        Task<ChangeLightResponse> ChangeLight(string id, ChangeLightRequest state);

        /// <summary>
        /// Gets <see cref="Group"/> details of all groups visible to the user.
        /// </summary>
        /// <returns>A collection of <see cref="Group"/> details.</returns>
        Task<IEnumerable<Group>> GetGroupData();

        /// <summary>
        /// Gets <see cref="Group"/> details of a specific group that is visible to the user.
        /// </summary>
        /// <param name="id">The ID of the group being queried.</param>
        /// <returns>Specific <see cref="Group"/> details.</returns>
        Task<Group> GetGroupData(string id);

        /// <summary>
        /// Gets <see cref="Light"/> details of all lights visible to the user.
        /// </summary>
        /// <returns>A collection of Philips Hue <see cref="Light"/> details.</returns>
        Task<IEnumerable<Light>> GetLightData();

        /// <summary>
        /// Gets <see cref="Light"/> details of a specific light that is visible to the user.
        /// </summary>
        /// <param name="id">The ID of the light being queried.</param>
        /// <returns>Philips Hue <see cref="Light"/> details.</returns>
        Task<Light> GetLightData(string id);
    }
}