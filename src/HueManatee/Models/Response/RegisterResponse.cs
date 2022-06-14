﻿using System.Collections.Generic;

namespace HueManatee
{
    /// <summary>
    /// The response from registering a new device with the Philips Hue Bridge.
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// The assigned username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Any error messages returned from registration.
        /// </summary>
        public List<string> Errors { get; set; }
    }
}