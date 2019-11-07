// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="VariablePhotoCapture.cs">
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
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera;
    using Windows.Media.Capture;
    using Windows.Media.Capture.Core;
    using Windows.Media.Devices.Core;

    /// <summary>
    /// This class wraps the <see cref="VariablePhotoSequenceCapture" />.
    /// </summary>
    /// <remarks>
    /// It can set different parameters (exposure compensation, flash, and etc.) for different frames.
    /// </remarks>
    public class VariablePhotoCapture : PhotoCaptureBase
    {
        #region Fields

        /// <summary>
        /// Amount of frames to capture.
        /// </summary>
        private const int NumFrames = 1;

        /// <summary>
        /// VPS capture.
        /// </summary>
        private volatile VariablePhotoSequenceCapture photoSequence;

        /// <summary>
        /// Whether the current capture is started.
        /// </summary>
        private volatile bool started;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="VariablePhotoCapture"/> class.
        /// </summary>
        /// <param name="cameraController">Camera controller.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraController"/> is <see langword="null"/>.</exception>
        public VariablePhotoCapture(CameraController cameraController)
            : base (cameraController)
        {
            // Do nothing.
        }

        #endregion // Constructor

        #region Events

        /// <summary>
        /// Occurs when a new photo is captured.
        /// </summary>
        public event EventHandler<CapturedFrame> PhotoCaptured;

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
        /// <exception cref="InvalidOperationException">Photo sequence is already initialized.</exception>
        public override async Task InitializeAsync(CaptureParameters parameters)
        {
            if (parameters == null || parameters.ImageEncoding == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (this.photoSequence != null)
            {
                throw new InvalidOperationException("Variable photo capture is already initialized.");
            }

            this.UpdateCameraSettings(parameters);

            this.photoSequence = await this.CameraController.MediaCapture.PrepareVariablePhotoSequenceCaptureAsync(parameters.ImageEncoding);
        }

        /// <summary>
        /// Starts the current photo capture.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="InvalidOperationException">The current controller is not initialized.</exception>
        public override async Task StartAsync()
        {
            if (this.started)
            {
                Tracing.Trace("VariablePhotoCapture: Capture is already started.");
                return;
            }

            // Notify listeners before the actual frames are captured.
            this.NotifyStarted();

            this.photoSequence.PhotoCaptured += this.PhotoSequenceOnPhotoCaptured;
            this.photoSequence.Stopped       += this.PhotoSequenceOnStopped;

            await this.photoSequence.StartAsync();

            this.started = true;
        }

        /// <summary>
        /// Stops the current photo capture, if needed.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public override async Task StopAsync()
        {
            if (this.photoSequence == null || !this.started)
            {
                Tracing.Trace("VariablePhotoCapture: Capture is already stopped.");
                return;
            }

            // Status will be updated in the PhotoSequenceOnStopped handler.
            await this.photoSequence.StopAsync();
        }

        /// <summary>
        /// Unloads the internal sequence objects.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public override async Task UnloadAsync()
        {
            if (this.photoSequence == null)
            {
                return;
            }

            await this.StopAsync();

            try
            {
                await this.photoSequence.FinishAsync();
            }
            catch (Exception e)
            {
                Tracing.Trace("VariablePhotoCapture: Exception while finishing capture:\r\n{0}", e);
            }

            this.photoSequence = null;
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Updates the current <c>VPS</c> frame controllers.
        /// </summary>
        private void UpdateCameraSettings(CaptureParameters parameters)
        {
            this.SetFlashMode(parameters.FlashMode);

            // Disable photo confirmation thumbnails for VPS.
            if (this.CameraController.MediaCapture.VideoDeviceController.PhotoConfirmationControl.Supported)
            {
                this.CameraController.MediaCapture.VideoDeviceController.PhotoConfirmationControl.Enabled = false;
            }

            VariablePhotoSequenceController controller = this.CameraController.MediaCapture.VideoDeviceController.VariablePhotoSequenceController;
            controller.DesiredFrameControllers.Clear();

            for (int frameIndex = 0; frameIndex < VariablePhotoCapture.NumFrames; frameIndex++)
            {
                FrameController frame = new FrameController();
                controller.DesiredFrameControllers.Add(frame);
            }
        }

        /// <summary>
        /// The <see cref="VariablePhotoSequenceCapture.PhotoCaptured"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="VariablePhotoCapturedEventArgs"/> instance containing the event data.</param>
        private void PhotoSequenceOnPhotoCaptured(VariablePhotoSequenceCapture sender, VariablePhotoCapturedEventArgs e)
        {
            Tracing.Trace("VariablePhotoCapture: Photo captured. Time offset: {0}, frame index: {1}", e.CaptureTimeOffset, e.UsedFrameControllerIndex);

            EventHandler<CapturedFrame> handler = this.PhotoCaptured;
            if (handler != null)
            {
                handler.Invoke(this, e.Frame);
            }
        }

        /// <summary>
        /// The <see cref="VariablePhotoSequenceCapture.Stopped"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event data.</param>
        private void PhotoSequenceOnStopped(VariablePhotoSequenceCapture sender, object e)
        {
            this.photoSequence.PhotoCaptured -= this.PhotoSequenceOnPhotoCaptured;
            this.photoSequence.Stopped       -= this.PhotoSequenceOnStopped;

            this.started = false;

            this.NotifyStopped();
        }

        #endregion // Private methods
    }
}
