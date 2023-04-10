using HueManatee.Request;
using HueManatee.Response;
using System.Threading.Tasks;

namespace HueManatee
{
    /// <summary>
    /// A registration client for the Philips Hue Bridge, generating a UserName.
    /// </summary>
    public interface IBridgeRegistrationClient
    {
        /// <summary>
        /// 
        /// Registers a device reference against the Philips Hue Bridge.
        /// 
        /// The first call may instruct you to press the Link button on the Bridge before re-attempting this same call.
        /// The UserName in the <see cref="RegisterResponse"/> can then be used for calls to the main <see cref="BridgeClient"/>.
        /// 
        /// </summary>
        /// <param name="registerRequest">Registration details for the Hue Bridge.</param>
        /// <returns>A <see cref="RegisterResponse"/> containing a username and/or error details.</returns>
        Task<RegisterResponse> Register(RegisterRequest registerRequest);
    }
}