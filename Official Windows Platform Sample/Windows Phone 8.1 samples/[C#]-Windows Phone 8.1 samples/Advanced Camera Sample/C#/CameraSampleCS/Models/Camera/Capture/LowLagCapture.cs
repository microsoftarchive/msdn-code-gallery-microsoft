// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="LowLagCapture.cs">
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
    using Windows.Foundation;
    using Windows.Media.Capture;
    using CameraSampleCS.Helpers;

    /// <summary>
    /// This object captures a single photo using the <see cref="LowLagPhotoCapture"/> API.
    /// </summary>
    public class LowLagCapture : PhotoCaptureBase
    {
        #region Fields

        /// <summary>
        /// Current capture parameters (used for reinitialization).
        /// </summary>
        private volatile CaptureParameters captureParameters;

        /// <summary>
        /// Low lag photo sequence.
        /// </summary>
        private volatile LowLagPhotoCapture photoCapture;

        /// <summary>
        /// Ongoing photo capture operation.
        /// </summary>
        private volatile IAsyncOperation<CapturedPhoto> captureOperation;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LowLagCapture"/> class.
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraController"/> is <see langword="null"/>.</exception>
        public LowLagCapture(CameraController cameraController)
            : base(cameraController)
        {
            // Do nothing.
        }

        #endregion // Constructor

        #region Events

        /// <summary>
        /// Occurs when a new photo is captured.
        /// </summary>
        public event EventHandler<CapturedPhoto> PhotoCaptured;

        #endregion // Events

        #region Public methods

        /// <summary>
        /// Initializes the photo capture on the current camera device.
        /// </summary>
        /// <param name="parameters">Capture parameters.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parameters"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// Image encoding is not set in the <paramref name="parameters"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">Photo capture is already initialized.</exception>
        public override async Task InitializeAsync(CaptureParameters parameters)
        {
            if (parameters == null || parameters.ImageEncoding == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (this.photoCapture != null)
            {
                throw new InvalidOperationException("Low lag photo capture is already initialized.");
            }

            this.captureParameters = parameters;
            this.UpdateCameraSettings(this.captureParameters);

            this.photoCapture = await this.CameraController.MediaCapture.PrepareLowLagPhotoCaptureAsync(parameters.ImageEncoding);
        }

        /// <summary>
        /// Starts the current photo capture.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="InvalidOperationException">The current capture is not initialized.</exception>
        public override Task StartAsync()
        {
            return Task.Run(() =>
            {
                if (this.captureOperation != null)
                {
                    Tracing.Trace("LowLagCapture: Capture is already started.");
                    return;
                }

                // We should notify listeners before the actual frame is captured.
                this.NotifyStarted();

                // Save operation to a temporary variable, so it won't be null in the task we start next.
                IAsyncOperation<CapturedPhoto> operation = this.photoCapture.CaptureAsync();
                this.captureOperation = operation;

                Task.Run(async () =>
                {
                    try
                    {
                        CapturedPhoto photo = await operation;

                        // No need to cancel the completed operation.
                        this.captureOperation = null;

                        EventHandler<CapturedPhoto> handler = this.PhotoCaptured;
                        if (handler != null)
                        {
                            handler.Invoke(this, photo);
                        }
                    }
                    catch (Exception e)
                    {
                        Tracing.Trace("LowLagCapture: Exception while waiting for captured photo:\r\n{0}", e);
                    }

                    await this.StopAsync();
                });
            });
        }

        /// <summary>
        /// Stops the current photo capture, if needed.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public override Task StopAsync()
        {
            return Task.Run(() =>
            {
                if (this.captureOperation == null)
                {
                    return;
                }

                Tracing.Trace("LowLagCapture: Cancelling the ongoing photo capture operation.");
                this.captureOperation.Cancel();
                this.captureOperation = null;
            }).ContinueWith(t => this.NotifyStopped());
        }

        /// <summary>
        /// Unloads the internal sequence objects.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public override async Task UnloadAsync()
        {
            if (this.photoCapture == null)
            {
                return;
            }

            await this.StopAsync();

            try
            {
                await this.photoCapture.FinishAsync();
            }
            catch (Exception e)
            {
                Tracing.Trace("LowLagCapture: Exception while finishing capture:\r\n{0}", e);
            }

            this.photoCapture = null;
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Updates the current camera settings.
        /// </summary>
        private void UpdateCameraSettings(CaptureParameters parameters)
        {
            this.SetFlashMode(parameters.FlashMode);

            // Thumbnail will be created by the storage service.
            // Enable it, if you want a faster thumbnail generation, which will be sent with the CapturedPhoto.
            this.CameraController.MediaCapture.VideoDeviceController.LowLagPhoto.ThumbnailEnabled = false;

            // Also disable photo confirmation thumbnails.
            if (this.CameraController.MediaCapture.VideoDeviceController.PhotoConfirmationControl.Supported)
            {
                this.CameraController.MediaCapture.VideoDeviceController.PhotoConfirmationControl.Enabled = false;
            }
        }

        #endregion // Private methods
    }
}
