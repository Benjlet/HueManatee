namespace HueManatee
{
    /// <summary>
    /// Used to request a UserName from the Philips Hue Bridge, linked with the DeviceType.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// The identifier for linking a new device with the Philips Hue Bridge.
        /// </summary>
        public string DeviceType { get; set; }
    }
}