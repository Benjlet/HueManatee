namespace HueManatee.Response
{
    /// <summary>
    /// The state of the Philips Hue Light.
    /// </summary>
    public class LightState
    {
        /// <summary>
        /// Whether the light is on or not.
        /// </summary>
        public bool? On { get; set; }

        /// <summary>
        /// The brightness of the light.
        /// </summary>
        public int? Brightness { get; set; }

        /// <summary>
        /// The hue of the light.
        /// </summary>
        public int? Hue { get; set; }

        /// <summary>
        /// The name of the active effect.
        /// </summary>
        public string Effect { get; set; }

        /// <summary>
        /// The saturation of the light.
        /// </summary>
        public int? Saturation { get; set; }

        /// <summary>
        /// The X value of the light's XY color value.
        /// </summary>
        public double? X { get; set; }

        /// <summary>
        /// The Y value of the light's XY color value.
        /// </summary>
        public double? Y { get; set; }

        /// <summary>
        /// The white color temperature of the light.
        /// </summary>
        public int? ColorTemperature { get; set; }
    }
}
