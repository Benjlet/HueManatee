using System.Drawing;

namespace HueManatee
{
    /// <summary>
    /// Used to request changes in state to a Philips Hue light, such as brightness or color.
    /// </summary>
    public class LightChangeRequest
    {
        /// <summary>
        /// The light's 'on' state.
        /// Set to <see langword="true"/> to turn the light on; <see langword="false"/> to turn the light off.
        /// <see langword="null"/> by default, meaning no changes to the light's current 'on' state.
        /// </summary>
        public bool? On { get; set; } = null;

        /// <summary>
        /// The light's brightness (0 - 254). Setting this value will overwrite the brightness calculated from the Color field, if that has also been supplied.
        /// <see langword="null"/> by default, meaning no changes to the light's current brightness.
        /// </summary>
        public int? Brightness { get; set; } = null;

        /// <summary>
        /// The light's hue (0 - 65535).
        /// If the Color field has been supplied, this value is ignored.
        /// <see langword="null"/> by default, meaning no changes to the light's current hue.
        /// </summary>
        public int? Hue { get; set; } = null;

        /// <summary>
        /// The light's saturation (0 - 254).
        /// If the Color field has been supplied, this value is ignored.
        /// <see langword="null"/> by default, meaning no changes to the light's current saturation.
        /// </summary>
        public int? Saturation { get; set; } = null;

        /// <summary>
        /// The name of the light effect to use - none or a color loop.
        /// <see langword="null"/> by default, meaning no changes to the light's current effect.
        /// </summary>
        public LightEffect? Effect { get; set; } = null;

        /// <summary>
        /// The color to set the light to.
        /// <see langword="null"/> by default, meaning no changes to the light's current color.
        /// </summary>
        public Color? Color { get; set; } = null;

        /// <summary>
        /// The light's white color temperature: 154 (cold) - 500 (warm).
        /// <see langword="null"/> by default, meaning no changes to the light's current color temperature.
        /// </summary>
        public int? ColorTemperature { get; set; } = null;
    }
}