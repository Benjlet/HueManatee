﻿namespace HueManatee.Request
{
    /// <summary>
    /// Used to request a UserName from the Philips Hue Bridge, linked with the DeviceType.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Intialises a new RegisterRequest with the supplied device type.
        /// </summary>
        /// <param name="deviceType">The identifier for linking a new device with the Philips Hue Bridge.</param>
        public RegisterRequest(string deviceType)
        {
            DeviceType = deviceType;
        }

        /// <summary>
        /// Initialises a new RegisterRequest.
        /// </summary>
        public RegisterRequest()
        {
        }

        /// <summary>
        /// The identifier for linking a new device with the Philips Hue Bridge.
        /// </summary>
        public string DeviceType { get; set; }
    }
}