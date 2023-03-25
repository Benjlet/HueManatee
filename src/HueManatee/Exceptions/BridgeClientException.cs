using System;

namespace HueManatee.Exceptions
{
    /// <summary>
    /// An exception from the <see cref="BridgeClient"/>.
    /// </summary>
    public class BridgeClientException : Exception
    {
        /// <summary>
        /// Intitialises a default <see cref="BridgeClientException"/>.
        /// </summary>
        public BridgeClientException()
        {
        }

        /// <summary>
        /// Intitialises a default <see cref="BridgeClientException"/> with the supplied message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public BridgeClientException(string message) : base(message)
        {
        }

        /// <summary>
        /// Intitialises a default <see cref="BridgeClientException"/> with the supplied message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception details.</param>
        public BridgeClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
