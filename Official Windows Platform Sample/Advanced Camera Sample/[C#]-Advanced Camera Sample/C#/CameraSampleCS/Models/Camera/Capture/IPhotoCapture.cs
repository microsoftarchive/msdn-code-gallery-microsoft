// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="IPhotoCapture.cs">
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
    using Windows.Media.Capture;
    using Windows.Storage.Streams;

    /// <summary>
    /// Defines a photo capture wrapper.
    /// </summary>
    /// <remarks>
    /// The <c>photo captured</c> events are not defined in this interface,
    /// because different capture wrappers may return different captured frames.<br/>
    /// For example, <see cref="CapturedFrame"/>, <see cref="RandomAccessStream"/>,
    /// or <see cref="CapturedPhoto"/> may be returned depending on the capture API used.
    /// </remarks>
    public interface IPhotoCapture
    {
        /// <summary>
        /// Occurs when the current photo capture sequence is started.
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Occurs when the current photo capture sequence is stopped.
        /// </summary>
        event EventHandler Stopped;

        /// <summary>
        /// Initializes the photo capture on the current camera device.
        /// </summary>
        /// <param name="parameters">Capture parameters.</param>
        /// <returns>Awaitable task.</returns>
        Task InitializeAsync(CaptureParameters parameters);

        /// <summary>
        /// Starts the current photo capture.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        Task StartAsync();

        /// <summary>
        /// Stops the current photo capture, if started.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        Task StopAsync();

        /// <summary>
        /// Unloads the internal sequence objects.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        Task UnloadAsync();
    }
}
