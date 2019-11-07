// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="PhotoCamera.cs">
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

namespace CameraSampleCS.Models.Camera
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Windows.Media.Devices;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera.Capture;
    using CameraSampleCS.Models.Camera.Tasks;
    using Microsoft.Devices;
    using Microsoft.Phone.Controls;
    using Windows.Media.Capture;

    /// <summary>
    /// Sample photo camera that supports <c>LowLag</c> and variable photo capture.
    /// </summary>
    public class PhotoCamera : BasicCamera
    {
        #region Fields

        /// <summary>
        /// Current photo capture object.
        /// </summary>
        private IPhotoCapture photoCapture;

        /// <summary>
        /// Whether the <see cref="photoCapture"/> is started.
        /// </summary>
        private volatile bool captureStarted;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoCamera"/> class.
        /// </summary>
        /// <param name="cameraType">Type of the camera device to use.</param>
        internal PhotoCamera(CameraType cameraType)
            : base(cameraType, CaptureUse.Photo)
        {
            // Do nothing.
        }

        #endregion // Constructor

        #region Events

        /// <summary>
        /// Occurs when the current photo capture is started.
        /// </summary>
        public event EventHandler CaptureStarted;

        /// <summary>
        /// Occurs when the current photo capture is stopped.
        /// </summary>
        public event EventHandler CaptureStopped;

        /// <summary>
        /// Occurs when a new photo is captured.
        /// </summary>
        public event EventHandler<ICapturedPhoto> PhotoCaptured;

        #endregion // Events

        #region Properties

        /// <summary>
        /// Gets or sets the current camera orientation.
        /// </summary>
        public PageOrientation Orientation { get; set; }

        /// <summary>
        /// Gets the current photo capture mode.
        /// </summary>
        public CaptureMode CaptureMode { get; private set; }

        /// <summary>
        /// Gets the current photo capture parameters.
        /// </summary>
        public CaptureParameters CaptureParameters { get; private set; }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Initializes a new photo capture controller, if needed.
        /// </summary>
        /// <param name="captureMode">Photo capture mode.</param>
        /// <param name="parameters">Camera capture parameters.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotImplementedException"><paramref name="captureMode"/> is not supported.</exception>
        public async Task InitializePhotoCaptureAsync(CaptureMode captureMode, CaptureParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (parameters.ImageEncoding == null)
            {
                throw new ArgumentNullException("parameters", "Image encoding is not set.");
            }

            Tracing.Trace("PhotoCamera::InitializePhotoCaptureAsync");

            if (this.photoCapture != null && this.CaptureMode == captureMode && parameters.Equals(this.CaptureParameters))
            {
                Tracing.Trace("PhotoCamera: Capture is already initialized with the same parameters.");
                return;
            }

            this.CaptureMode       = captureMode;
            this.CaptureParameters = parameters;

            await this.DestroyPhotoCaptureAsync();

            this.photoCapture = this.CreatePhotoCapture(captureMode);

            await this.TaskEngine.EnqueueTaskAsync(new InitializePhotoCaptureTask(this.CameraController, this.photoCapture, parameters));
        }

        /// <summary>
        /// Starts the configured photo capture.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public Task StartPhotoCaptureAsync()
        {
            Tracing.Trace("PhotoCamera::StartPhotoCaptureAsync");
            return this.TaskEngine.EnqueueTaskAsync(new StartPhotoCaptureTask(this.CameraController, this.photoCapture));
        }

        /// <summary>
        /// Stops the current photo capture.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public Task StopPhotoCaptureAsync()
        {
            Tracing.Trace("PhotoCamera::StopPhotoCaptureAsync");
            return this.TaskEngine.EnqueueTaskAsync(new StopPhotoCaptureTask(this.CameraController, this.photoCapture));
        }

        /// <summary>
        /// Unloads the camera device.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public override async Task UnloadCameraAsync()
        {
            Tracing.Trace("PhotoCamera::UnloadCameraAsync");
            await this.DestroyPhotoCaptureAsync();
            await base.UnloadCameraAsync();
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Creates a new photo capture based on the <paramref name="captureMode"/> specified.
        /// </summary>
        /// <param name="captureMode">Capture mode to create photo capture for.</param>
        /// <returns>Photo capture created.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="captureMode"/> is not supported.</exception>
        private IPhotoCapture CreatePhotoCapture(CaptureMode captureMode)
        {
            Tracing.Trace("PhotoCamera: Creating new capture {0}", captureMode);

            IPhotoCapture capture;

            switch (captureMode)
            {
                case CaptureMode.LowLag:
                    capture = new LowLagCapture(this.CameraController);
                    break;
                case CaptureMode.Variable:
                    // VPS may not be supported on the current device.
                    if (!this.CameraController.MediaCapture.VideoDeviceController.VariablePhotoSequenceController.Supported)
                    {
                        throw new NotSupportedException("VPS is not supported.");
                    }

                    capture = new VariablePhotoCapture(this.CameraController);
                    break;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Capture mode {0} is not supported.", captureMode));
            }

            this.captureStarted = false;
            this.SubscribeToCaptureEvents(capture);

            return capture;
        }

        /// <summary>
        /// Stops and unloads the current <see cref="photoCapture"/>.
        /// </summary>
        private async Task DestroyPhotoCaptureAsync()
        {
            if (this.photoCapture == null)
            {
                return;
            }

            if (this.captureStarted)
            {
                Tracing.Trace("PhotoCamera: Stopping photo capture.");
                await this.photoCapture.StopAsync();
            }

            Tracing.Trace("PhotoCamera: Unloading photo capture.");
            await this.photoCapture.UnloadAsync();

            this.UnsubscribeFromCaptureEvents(this.photoCapture);
            this.photoCapture = null;
        }

        /// <summary>
        /// Subscribes to the <paramref name="capture"/> events.
        /// </summary>
        /// <param name="capture">Photo capture.</param>
        private void SubscribeToCaptureEvents(IPhotoCapture capture)
        {
            Tracing.Trace("PhotoCamera: Subscribing to capture events.");

            capture.Started += this.CaptureOnStarted;
            capture.Stopped += this.CaptureOnStopped;

            // Subscribe to a specific capture events.
            // See IPhotoCapture remarks section for details.
            LowLagCapture lowLagCapture = capture as LowLagCapture;
            if (lowLagCapture != null)
            {
                lowLagCapture.PhotoCaptured += this.LowLagCaptureOnPhotoCaptured;
                return;
            }

            VariablePhotoCapture variableCapture = capture as VariablePhotoCapture;
            if (variableCapture != null)
            {
                variableCapture.PhotoCaptured += this.VariableCaptureOnPhotoCaptured;
            }
        }

        /// <summary>
        /// Unsubscribes from the <paramref name="capture"/> events.
        /// </summary>
        /// <param name="capture">Photo capture.</param>
        private void UnsubscribeFromCaptureEvents(IPhotoCapture capture)
        {
            Tracing.Trace("PhotoCamera: Unsubscribing from capture events.");

            capture.Started -= this.CaptureOnStarted;
            capture.Stopped -= this.CaptureOnStopped;

            LowLagCapture lowLagCapture = capture as LowLagCapture;
            if (lowLagCapture != null)
            {
                lowLagCapture.PhotoCaptured -= this.LowLagCaptureOnPhotoCaptured;
                return;
            }

            VariablePhotoCapture variableCapture = capture as VariablePhotoCapture;
            if (variableCapture != null)
            {
                variableCapture.PhotoCaptured -= this.VariableCaptureOnPhotoCaptured;
            }
        }

        /// <summary>
        /// The <see cref="IPhotoCapture.Started"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CaptureOnStarted(object sender, EventArgs eventArgs)
        {
            Tracing.Trace("PhotoCamera: Capture started.");

            this.captureStarted = true;

            EventHandler handler = this.CaptureStarted;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The <see cref="IPhotoCapture.Stopped"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CaptureOnStopped(object sender, EventArgs eventArgs)
        {
            Tracing.Trace("PhotoCamera: Capture stopped.");

            this.captureStarted = false;

            EventHandler handler = this.CaptureStopped;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The <see cref="LowLagCapture.PhotoCaptured"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="capturedPhoto">Captured photo.</param>
        private void LowLagCaptureOnPhotoCaptured(object sender, CapturedPhoto capturedPhoto)
        {
            Tracing.Trace("PhotoCamera: Low lag photo captured.");
            this.NotifyPhotoCaptured(CapturedImageFrame.CreateFromCapturedFrame(capturedPhoto.Frame, this.Orientation, this.CameraType));
        }

        /// <summary>
        /// The <see cref="VariablePhotoCapture.PhotoCaptured"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="capturedFrame">Captured frame.</param>
        private void VariableCaptureOnPhotoCaptured(object sender, CapturedFrame capturedFrame)
        {
            Tracing.Trace("PhotoCamera: Variable frame captured.");
            this.NotifyPhotoCaptured(CapturedImageFrame.CreateFromCapturedFrame(capturedFrame, this.Orientation, this.CameraType));
        }

        /// <summary>
        /// Notifies the <see cref="PhotoCaptured"/> event listeners.
        /// </summary>
        /// <param name="frame">Captured frame.</param>
        private void NotifyPhotoCaptured(ICapturedPhoto frame)
        {
            EventHandler<ICapturedPhoto> handler = this.PhotoCaptured;
            if (handler != null)
            {
                handler.Invoke(this, frame);
            }
        }

        #endregion // Private methods
    }
}
