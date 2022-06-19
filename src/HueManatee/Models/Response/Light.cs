namespace HueManatee.Response
{
    /// <summary>
    /// Details of the Philips Hue Light.
    /// </summary>
    public class Light
    {
        /// <summary>
        /// The ID of the light, used for referencing a specific light in change requests.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The state of the light, including details like its brightness and hue.
        /// </summary>
        public LightState State { get; set; }

        /// <summary>
        /// The name of the light.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The model ID of the light.
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// The manufacturer of the light.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// The product name for the light.
        /// </summary>
        public string ProductName { get; set; }
    }
}
