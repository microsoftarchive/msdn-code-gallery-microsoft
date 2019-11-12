// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="PhotoCaptureBase.cs">
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
    using CameraSampleCS.Models.Camera;

    /// <summary>
    /// Simple basic class for <see cref="IPhotoCapture"/> implementations.
    /// </summary>
    public abstract class PhotoCaptureBase : IPhotoCapture
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoCaptureBase"/> class.
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraController"/> is <see langword="null"/>.</exception>
        protected PhotoCaptureBase(CameraController cameraController)
        {
            if (cameraController == null)
            {
                throw new ArgumentNullException("cameraController");
            }

            this.CameraController = cameraController;
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the current camera controller.
        /// </summary>
        protected CameraController CameraController { get; private set; }

        #endregion // Properties

        #region Events

        /// <summary>
        /// Occurs when the current photo capture sequence is started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when the current photo capture sequence is stopped.
        /// </summary>
        public event EventHandler Stopped;

        #endregion // Events

        #region Public methods

        /// <summary>
        /// Initializes the photo capture on the current camera device.
        /// </summary>
        /// <param name="parameters">Capture parameters.</param>
        /// <returns>Awaitable task.</returns>
        public abstract Task InitializeAsync(CaptureParameters parameters);

        /// <summary>
        /// Starts the current photo capture.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public abstract Task StartAsync();

        /// <summary>
        /// Stops the current photo capture, if needed.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public abstract Task StopAsync();

        /// <summary>
        /// Unloads the internal sequence objects.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public abstract Task UnloadAsync();

        #endregion // Public methods

        #region Protected methods

        /// <summary>
        /// Sets the <paramref name="flashMode"/> on the current flash control.
        /// </summary>
        /// <param name="flashMode">Flash mode.</param>
        protected void SetFlashMode(FlashMode flashMode)
        {
            if (!this.CameraController.FlashSupported)
            {
                return;
            }

            switch (flashMode)
            {
                case FlashMode.Off:
                    this.CameraController.MediaCapture.VideoDeviceController.FlashControl.Enabled = false;
                    this.CameraController.MediaCapture.VideoDeviceController.FlashControl.Auto    = false;
                    break;
                case FlashMode.On:
                    this.CameraController.MediaCapture.VideoDeviceController.FlashControl.Enabled = true;
                    this.CameraController.MediaCapture.VideoDeviceController.FlashControl.Auto    = false;
                    break;
                case FlashMode.Auto:
                    this.CameraController.MediaCapture.VideoDeviceController.FlashControl.Enabled = true;
                    this.CameraController.MediaCapture.VideoDeviceController.FlashControl.Auto    = true;
                    break;
            }
        }

        /// <summary>
        /// Notifies the <see cref="Started"/> event listeners.
        /// </summary>
        protected void NotifyStarted()
        {
            EventHandler handler = this.Started;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Notifies the <see cref="Stopped"/> event listeners.
        /// </summary>
        protected void NotifyStopped()
        {
            EventHandler handler = this.Stopped;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion // Protected methods
    }
}
