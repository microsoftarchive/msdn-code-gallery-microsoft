// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ICapturedPhoto.cs">
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
    using System.Threading.Tasks;
    using Windows.Graphics.Imaging;

    /// <summary>
    /// Represents the captured photo.
    /// </summary>
    public interface ICapturedPhoto : IDisposable
    {
        /// <summary>
        /// Gets the image height in pixels.
        /// </summary>
        uint Height { get; }

        /// <summary>
        /// Gets the image width in pixels.
        /// </summary>
        uint Width { get; }

        /// <summary>
        /// Gets the frame rotation.
        /// </summary>
        BitmapRotation Rotation { get; }

        /// <summary>
        /// Gets the frame flip.
        /// </summary>
        BitmapFlip Flip { get; }

        /// <summary>
        /// Returns the internal frame pixel data as a byte array.
        /// </summary>
        /// <returns>Byte array containing the raw pixel data.</returns>
        Task<byte[]> DetachPixelDataAsync();
    }
}
