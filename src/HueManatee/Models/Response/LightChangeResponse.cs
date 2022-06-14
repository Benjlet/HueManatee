using System.Collections.Generic;

namespace HueManatee
{
    /// <summary>
    /// The response from updating the state of a light.
    /// </summary>
    public class LightChangeResponse
    {
        /// <summary>
        /// Key/value pairs of processed light state changes and their updated values.
        /// </summary>
        public Dictionary<string, string> Changes { get; set; }

        /// <summary>
        /// Any error messages returned from the light state change.
        /// </summary>
        public List<string> Errors { get; set; }
    }
}
