// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="BasicCamera.cs">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera.Tasks;
    using Microsoft.Devices;
    using Windows.Media.Capture;
    using Windows.Media.Devices;

    /// <summary>
    /// Basic class that provides camera initialization methods and common properties.
    /// </summary>
    /// <remarks>
    /// Applications should extend this class for their needs, because it doesn't expose
    /// any photo or video recording methods.
    /// </remarks>
    public abstract class BasicCamera
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicCamera"/> class.
        /// </summary>
        /// <param name="cameraType">Type of the camera device to use.</param>
        /// <param name="captureUse">The desired usage of the camera capture device.</param>
        protected BasicCamera(CameraType cameraType, CaptureUse captureUse)
        {
            this.CameraController = new CameraController(cameraType, captureUse);
            this.TaskEngine       = new CameraTaskEngine();

            this.CameraController.FocusChanged += this.CameraControllerOnFocusChanged;
        }

        #endregion // Constructor

        #region Events

        /// <summary>
        /// Occurs when camera focus state changes.
        /// </summary>
        public event EventHandler<MediaCaptureFocusChangedEventArgs> FocusChanged;

        #endregion // Events

        #region Properties

        /// <summary>
        /// Gets the type of the camera.
        /// </summary>
        public CameraType CameraType
        {
            get
            {
                return this.CameraController.CameraType;
            }
        }

        /// <summary>
        /// Gets the desired usage of the camera capture device.
        /// </summary>
        public CaptureUse CaptureUse
        {
            get
            {
                return this.CameraController.CaptureUse;
            }
        }

        /// <summary>
        /// Gets the collection of supported camera video preview resolutions.
        /// </summary>
        public IEnumerable<Size> SupportedPreviewResolutions
        {
            get
            {
                return this.CameraController.AvailablePreviewEncodings.Select(encoding => new Size(encoding.Width, encoding.Height)).Distinct();
            }
        }

        /// <summary>
        /// Gets the collection of supported camera photo resolutions.
        /// </summary>
        public IEnumerable<Size> SupportedPhotoResolutions
        {
            get
            {
                return this.CameraController.AvailablePhotoEncodings.Select(encoding => new Size(encoding.Width, encoding.Height)).Distinct();
            }
        }

        /// <summary>
        /// Gets the collection of supported camera video resolutions.
        /// </summary>
        public IEnumerable<Size> SupportedVideoResolutions
        {
            get
            {
                return this.CameraController.AvailableVideoEncodings.Select(encoding => new Size(encoding.Width, encoding.Height)).Distinct();
            }
        }

        /// <summary>
        /// Gets the video preview resolution from the current camera device.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync"/>
        /// before accessing it.
        /// </remarks>
        public Size PreviewSize
        {
            get
            {
                return this.CameraController.PreviewResolution;
            }
        }

        /// <summary>
        /// Gets the video preview brush from the current camera device.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync"/>
        /// before accessing it.
        /// </remarks>
        public Brush PreviewBrush
        {
            get
            {
                return this.CameraController.PreviewBrush;
            }
        }

        /// <summary>
        /// Gets the value indicating whether the current camera device supports focus operations.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync"/>
        /// before accessing it.
        /// </remarks>
        public bool FocusSupported
        {
            get
            {
                return this.CameraController.FocusSupported;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current camera device supports focus regions.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync"/>
        /// before accessing it.
        /// </remarks>
        public bool FocusAtPointSupported
        {
            get
            {
                return this.CameraController.FocusAtPointSupported;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current camera device supports continuous automatic focus.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync"/>
        /// before accessing it.
        /// </remarks>
        public bool ContinuousAutoFocusSupported
        {
            get
            {
                return this.CameraController.ContinuousAutoFocusSupported;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current camera device has flash.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync"/>
        /// before accessing it.
        /// </remarks>
        public bool FlashSupported
        {
            get
            {
                return this.CameraController.FlashSupported;
            }
        }

        /// <summary>
        /// Gets the camera wrapper.
        /// </summary>
        protected CameraController CameraController { get; private set; }

        /// <summary>
        /// Gets the camera tasks execution engine.
        /// </summary>
        protected CameraTaskEngine TaskEngine { get; private set; }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Initializes camera device.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public virtual Task LoadCameraAsync()
        {
            Tracing.Trace("BasicCamera::LoadCameraAsync");
            return this.TaskEngine.EnqueueTaskAsync(new LoadCameraTask(this.CameraController));
        }

        /// <summary>
        /// Unloads the camera device.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public virtual Task UnloadCameraAsync()
        {
            Tracing.Trace("BasicCamera::UnloadCameraAsync");

            // Since the camera will be unloaded, there's no need to have
            // any tasks (except for the stopcapture ones) in the queue.
            this.ClearTasksQueue();

            return this.TaskEngine.EnqueueTaskAsync(new UnloadCameraTask(this.CameraController));
        }

        /// <summary>
        /// Starts the video preview using the <paramref name="previewSize" /> specified.
        /// </summary>
        /// <param name="previewSize">The desired video preview size.</param>
        /// <returns>Awaitable task.</returns>
        /// <seealso cref="PreviewBrush"/>
        /// <seealso cref="PreviewSize"/>
        public virtual Task StartPreviewAsync(Size previewSize)
        {
            Tracing.Trace("BasicCamera::StartPreviewAsync");
            return this.TaskEngine.EnqueueTaskAsync(new StartPreviewTask(this.CameraController, previewSize));
        }

        /// <summary>
        /// Stops the video preview.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public virtual Task StopPreviewAsync()
        {
            Tracing.Trace("BasicCamera::StopPreviewAsync");
            return this.TaskEngine.EnqueueTaskAsync(new StopPreviewTask(this.CameraController));
        }

        /// <summary>
        /// Focuses at the <paramref name="focusPoint"/> specified.
        /// </summary>
        /// <param name="focusPoint">Point of intereset to focus on.<br/>
        /// If <see langword="null"/>, the central point will be used.</param>
        /// <returns>Awaitable task.</returns>
        public virtual Task FocusAsync(Point? focusPoint)
        {
            Tracing.Trace("BasicCamera::FocusAsync");
            return this.TaskEngine.EnqueueTaskAsync(new FocusTask(this.CameraController, focusPoint));
        }

        /// <summary>
        /// Enables the continuous automatic focus for the current device.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public virtual Task EnableContinuousAutoFocusAsync()
        {
            Tracing.Trace("BasicCamera::EnableContinuousAutoFocusAsync");
            return this.TaskEngine.EnqueueTaskAsync(new EnableContinuousAutoFocusTask(this.CameraController));
        }

        /// <summary>
        /// Resets the current focus to its default value.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public virtual Task ResetFocusAsync()
        {
            Tracing.Trace("BasicCamera::ResetFocusAsync");
            return this.TaskEngine.EnqueueTaskAsync(new ResetFocusTask(this.CameraController));
        }

        /// <summary>
        /// Cancels the the onging focus operations, if any.
        /// </summary>
        public virtual void CancelFocus()
        {
            Tracing.Trace("BasicCamera::CancelFocus");
            this.TaskEngine.CancelCurrentTask(t => t is FocusTask || t is ResetFocusTask);
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Tries to cancel the current and remove all pending tasks from the execution queue.
        /// </summary>
        private void ClearTasksQueue()
        {
            bool nonStopCaptureTaskFound = false;

            // We don't want to cancel StopCapture tasks, if they're at the beginning of the
            // execution queue, because cancelling them would potentially leave camera in
            // an invalid state.
            this.TaskEngine.CancelPendingTasks(t =>
            {
                if (t.Type == CameraTaskType.StopCapture && !nonStopCaptureTaskFound)
                {
                    return false;
                }

                nonStopCaptureTaskFound = true;
                return true;
            });

            // Try to cancel the current task.
            this.TaskEngine.CancelCurrentTask(t => true);
        }

        /// <summary>
        /// The <see cref="CameraController.FocusChanged"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="MediaCaptureFocusChangedEventArgs"/> instance containing the event data.</param>
        private void CameraControllerOnFocusChanged(object sender, MediaCaptureFocusChangedEventArgs e)
        {
            EventHandler<MediaCaptureFocusChangedEventArgs> handler = this.FocusChanged;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        #endregion // Private methods
    }
}
