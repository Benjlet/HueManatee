using System.Collections.Generic;

namespace HueManatee.Response
{
    /// <summary>
    /// Details of the light group.
    /// </summary>
    public class Group
    {
        /// <summary>
        /// The ID of the light group.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the light group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The IDs of the lights in the light group.
        /// </summary>
        public IEnumerable<string> Lights { get; set; }

        /// <summary>
        /// The IDs of the sensors in the light group.
        /// </summary>
        public IEnumerable<string> Sensors { get; set; }

        /// <summary>
        /// The group type, such as a Zone or Room.
        /// </summary>
        public string Type { get; set; }
    }
}
