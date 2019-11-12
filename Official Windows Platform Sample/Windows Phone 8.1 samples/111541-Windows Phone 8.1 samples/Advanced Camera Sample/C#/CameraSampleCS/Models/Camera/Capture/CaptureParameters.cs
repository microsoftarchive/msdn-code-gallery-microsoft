// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CaptureParameters.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace CameraSampleCS.Models.Camera.Capture
{
    using System;
    using CameraSampleCS.Models.Camera;
    using Windows.Media.MediaProperties;

    /// <summary>
    /// Contains photo capture parameters.
    /// </summary>
    public class CaptureParameters
    {
        #region Properties

        /// <summary>
        /// Gets or sets the desired image encoding properties.
        /// </summary>
        public ImageEncodingProperties ImageEncoding { get; set; }

        /// <summary>
        /// Gets or sets the flash mode.
        /// </summary>
        public FlashMode FlashMode { get; set; }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Determines whether the specified <see cref="CaptureParameters"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="CaptureParameters"/> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true"/>, if the specified <see cref="CaptureParameters"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(CaptureParameters other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.FlashMode != other.FlashMode)
            {
                return false;
            }

            if (this.ImageEncoding == null)
            {
                return other.ImageEncoding == null;
            }

            if (!object.ReferenceEquals(this.ImageEncoding, other.ImageEncoding))
            {
                return true;
            }

            if (this.ImageEncoding.Height != other.ImageEncoding.Height
                || this.ImageEncoding.Width != other.ImageEncoding.Width
                || !string.Equals(this.ImageEncoding.Type, other.ImageEncoding.Type, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(this.ImageEncoding.Subtype, other.ImageEncoding.Subtype, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true"/>, if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is CaptureParameters && this.Equals((CaptureParameters)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)this.FlashMode;
                hashCode = (hashCode * 397) ^ (this.ImageEncoding != null ? this.ImageEncoding.GetHashCode() : 0);

                return hashCode;
            }
        }

        #endregion // Public methods
    }
}
