using System;

namespace Windows7.Location
{
    /// <summary>
    /// Represents the desired report accuracy
    /// </summary>
    public enum DesiredAccuracy : uint
    {
        /// <summary>
        /// The sensor should use the accuracy for which it can optimize power and other such cost considerations.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The sensor should deliver the highest accuracy report possible.
        /// This includes using services that might charge money, or consuming higher levels of battery power or connection bandwidth.
        /// </summary>
        High = 1
    }
}
