// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CameraController.cs">
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
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using CameraSampleCS.Helpers;
    using Microsoft.Devices;
    using Windows.Devices.Enumeration;
    using Windows.Foundation;
    using Windows.Media.Capture;
    using Windows.Media.Devices;
    using Windows.Media.MediaProperties;
    using Windows.Phone.Media.Capture;

    using AutoFocusRange  = Windows.Media.Devices.AutoFocusRange;
    using MediaStreamType = Windows.Media.Capture.MediaStreamType;
    using Size            = System.Windows.Size;

    /// <summary>
    /// Wrapper for the <see cref="MediaCapture"/> class.
    /// </summary>
    public class CameraController : BindableBase, IDisposable
    {
        #region Fields

        /// <summary>
        /// List of supported video formats to be used by the default format selector.
        /// </summary>
        /// <seealso cref="CreateMediaFormatSelector"/>
        private static readonly IEnumerable<string> SupportedFormats = new List<string> { "nv12", "rgb32" };

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraController"/> class.
        /// </summary>
        /// <param name="cameraType">Type of the camera device to use.</param>
        /// <param name="captureUse">The desired usage of the camera capture device.</param>
        /// <exception cref="ArgumentException"><paramref name="cameraType"/> is not supported.</exception>
        public CameraController(CameraType cameraType, CaptureUse captureUse)
        {
            this.CameraType = cameraType;
            this.CaptureUse = captureUse;

            this.ResetCaptureDevice(notifyProperties: false);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CameraController"/> class.
        /// </summary>
        ~CameraController()
        {
            this.Dispose(disposing: false);
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
        /// Gets the current media capture object.
        /// </summary>
        public MediaCapture MediaCapture { get; private set; }

        /// <summary>
        /// Gets the type of the current camera device.
        /// </summary>
        public CameraType CameraType { get; private set; }

        /// <summary>
        /// Gets the desired usage of the camera capture device.
        /// </summary>
        public CaptureUse CaptureUse { get; private set; }

        /// <summary>
        /// Gets the information about the current camera device.
        /// </summary>
        public DeviceInformation DeviceInformation { get; private set; }

        /// <summary>
        /// Gets the video port used for the <see cref="PreviewBrush"/>.
        /// </summary>
        public string PreviewVideoPort { get; private set; }

        /// <summary>
        /// Gets the collection of the available video preview encoding properties.
        /// </summary>
        public IReadOnlyList<VideoEncodingProperties> AvailablePreviewEncodings { get; private set; }

        /// <summary>
        /// Gets the collection of the available photo encoding properties.
        /// </summary>
        public IReadOnlyList<VideoEncodingProperties> AvailablePhotoEncodings { get; private set; }

        /// <summary>
        /// Gets the collection of the available video encoding properties.
        /// </summary>
        public IReadOnlyList<VideoEncodingProperties> AvailableVideoEncodings { get; private set; }

        /// <summary>
        /// Gets the current video preview format.
        /// </summary>
        public VideoEncodingProperties PreviewFormat { get; private set; }

        /// <summary>
        /// Gets the current video preview rotation.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync(System.Windows.Size)"/>
        /// before accessing it.
        /// </remarks>
        public VideoRotation Rotation { get; private set; }

        /// <summary>
        /// Gets the brush element, that shows the preview.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync(System.Windows.Size)"/>
        /// before accessing it.
        /// </remarks>
        public Brush PreviewBrush { get; private set; }

        /// <summary>
        /// Gets the current preview resolution.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync(System.Windows.Size)"/>
        /// before accessing it.
        /// </remarks>
        public Size PreviewResolution { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current camera device supports focus.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync(System.Windows.Size)"/>
        /// before accessing it.
        /// </remarks>
        public bool FocusSupported { get; private set; }

        /// <summary>
        /// Gets a value indicating whether current camera device supports focus at point.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync(System.Windows.Size)"/>
        /// before accessing it.
        /// </remarks>
        public bool FocusAtPointSupported { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current camera device supports continuous automatic focus.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync(System.Windows.Size)"/>
        /// before accessing it.
        /// </remarks>
        public bool ContinuousAutoFocusSupported { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the current camera device supports flash.
        /// </summary>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync(System.Windows.Size)"/>
        /// before accessing it.
        /// </remarks>
        public bool FlashSupported { get; private set; }

        /// <summary>
        /// Gets or sets the current flash mode.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Camera is not initialized.
        ///     <para>-or-</para>
        /// Flash is not supported by the current camera device.
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// The <see cref="FlashMode"/> given is not supported by the current controller.
        /// </exception>
        /// <seealso cref="InitializeAsync"/>
        /// <remarks>
        /// This property is populated only after the preview is started.<br/>
        /// Make sure you call the <see cref="StartPreviewAsync(System.Windows.Size)"/>
        /// before accessing it.
        /// </remarks>
        public FlashMode FlashMode
        {
            get
            {
                if (this.MediaCapture == null)
                {
                    throw new InvalidOperationException("Camera is not initialized.");
                }

                if (!this.FlashSupported)
                {
                    throw new InvalidOperationException("Flash is not supported by the current camera device.");
                }

                FlashMode mode = FlashMode.Off;
                bool auto      = this.MediaCapture.VideoDeviceController.FlashControl.Auto;
                bool enabled   = this.MediaCapture.VideoDeviceController.FlashControl.Enabled;

                if (auto && enabled)
                {
                    mode = FlashMode.Auto;
                }
                else if (!auto && enabled)
                {
                    mode = FlashMode.On;
                }

                return mode;
            }

            set
            {
                if (this.MediaCapture == null)
                {
                    throw new InvalidOperationException("Camera is not initialized.");
                }

                if (!this.FlashSupported)
                {
                    throw new InvalidOperationException("Flash is not supported by the current camera device.");
                }

                Tracing.Trace("CameraController: Setting flash mode to: {0}", value);

                switch (value)
                {
                    case FlashMode.Off:
                        this.MediaCapture.VideoDeviceController.FlashControl.Auto    = false;
                        this.MediaCapture.VideoDeviceController.FlashControl.Enabled = false;
                        break;
                    case FlashMode.On:
                        this.MediaCapture.VideoDeviceController.FlashControl.Auto    = false;
                        this.MediaCapture.VideoDeviceController.FlashControl.Enabled = true;
                        break;
                    case FlashMode.Auto:
                        this.MediaCapture.VideoDeviceController.FlashControl.Auto    = true;
                        this.MediaCapture.VideoDeviceController.FlashControl.Enabled = true;
                        break;
                    default:
                        throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Flash mode {0} is not supported.", value));
                }
            }
        }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Initializes the current <see cref="MediaCapture"/>.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="CameraType"/> is not supported.
        ///     <para>-or-</para>
        /// No suitable photo or video formats available.
        /// </exception>
        public async Task InitializeAsync()
        {
            Tracing.Trace("CameraController: Initializing camera device.");

            if (this.MediaCapture != null)
            {
                Tracing.Trace("CameraController: Camera device is already initialized.");
                return;
            }

            try
            {
                ////
                //// Initialize media capture.
                ////

                // Choose the device to be used.
                this.DeviceInformation = await DeviceHelper.GetCameraDeviceInfoAsync(this.CameraType);
                if (this.DeviceInformation == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} camera is not supported.", this.CameraType));
                }

                Tracing.Trace("CameraController: Camera device Id: {0}", this.DeviceInformation.Id);

                this.MediaCapture = new MediaCapture();
                this.MediaCapture.Failed                   += this.MediaCaptureFailed;
                this.MediaCapture.RecordLimitationExceeded += this.MediaCaptureRecordLimitationExceeded;

                await this.MediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    // Depending on the camera usage mode we may want to disable audio capture.
                    // StreamingCaptureMode = this.CaptureUse == CaptureUse.Photo ? StreamingCaptureMode.Video : StreamingCaptureMode.AudioAndVideo,

                    // BUG: There's a platform bug that doesn't allow usage of the LowLagPhotoSequence after a VariablePhotoSequence capture
                    //      is performed. If you are not planning to use LLPC and VPS together, feel free to use the commented version above.
                    StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo,
                    PhotoCaptureSource   = PhotoCaptureSource.Auto,
                    AudioDeviceId        = string.Empty,
                    VideoDeviceId        = this.DeviceInformation.Id
                });

                // Give a hint to a driver, so that 3A algorithm can do a better job for the target camera usage type.
                this.MediaCapture.VideoDeviceController.PrimaryUse = this.CaptureUse;

                this.AvailablePreviewEncodings = this.MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview).OfType<VideoEncodingProperties>().ToList().AsReadOnly();
                this.AvailablePhotoEncodings   = this.MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).OfType<VideoEncodingProperties>().ToList().AsReadOnly();
                this.AvailableVideoEncodings   = this.MediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord).OfType<VideoEncodingProperties>().ToList().AsReadOnly();

                this.NotifyPropertiesChanged();
            }
            catch (Exception e)
            {
                Tracing.Trace("CameraController: InitializeAsync: 0x{0:X8}\r\n{1}", e.HResult, e);
                this.ResetCaptureDevice(notifyProperties: true);

                throw;
            }

            Tracing.Trace("CameraController: Media capture initialized.");
        }

        /// <summary>
        /// Initializes video preview with the <paramref name="previewSize"/> specified.
        /// </summary>
        /// <param name="previewSize">Size of the preview.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="previewSize"/> is invalid.</exception>
        /// <exception cref="InvalidOperationException">No suitable video preview formats found.</exception>
        public Task StartPreviewAsync(Size previewSize)
        {
            if (previewSize.Height <= 0.0 || previewSize.Width <= 0.0)
            {
                throw new ArgumentOutOfRangeException("previewSize", previewSize, "Preview size is invalid.");
            }

            return this.StartPreviewAsync(CameraController.CreateMediaFormatSelector(previewSize));
        }

        /// <summary>
        /// Initializes video preview and updates the <see cref="PreviewBrush"/> accordingly.
        /// </summary>
        /// <param name="previewFormatSelector">
        /// Function that chooses the most suitable preview format from the provided collection.
        /// </param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="previewFormatSelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">No suitable video preview formats found.</exception>
        public async Task StartPreviewAsync(Func<IEnumerable<VideoEncodingProperties>, VideoEncodingProperties> previewFormatSelector)
        {
            if (previewFormatSelector == null)
            {
                throw new ArgumentNullException("previewFormatSelector");
            }

            Tracing.Trace("CameraController: Starting video preview.");

            if (!string.IsNullOrEmpty(this.PreviewVideoPort))
            {
                Tracing.Trace("CameraController: Video preview is already started.");
                return;
            }

            try
            {
                // Set the video preview format.
                VideoEncodingProperties previewFormat = await this.DoSetMediaFormatAsync(MediaStreamType.VideoPreview, previewFormatSelector);

                MediaCapturePreviewSink previewSink = new MediaCapturePreviewSink();

                VideoBrush videoBrush = new VideoBrush();
                videoBrush.SetSource(previewSink);

                MediaEncodingProfile profile = new MediaEncodingProfile { Audio = null, Video = previewFormat };
                await this.MediaCapture.StartPreviewToCustomSinkAsync(profile, previewSink);

                this.PreviewFormat     = previewFormat;
                this.PreviewResolution = new Size(previewFormat.Width, previewFormat.Height);
                this.PreviewBrush      = videoBrush;
                this.PreviewVideoPort  = previewSink.ConnectionPort;

                ////
                //// Update camera properties.
                ////

                this.Rotation                     = this.MediaCapture.GetPreviewRotation();
                this.FocusSupported               = this.MediaCapture.VideoDeviceController.FocusControl.Supported;
                this.FocusAtPointSupported        = this.MediaCapture.VideoDeviceController.RegionsOfInterestControl.AutoFocusSupported && this.MediaCapture.VideoDeviceController.RegionsOfInterestControl.MaxRegions > 0;
                this.ContinuousAutoFocusSupported = this.MediaCapture.VideoDeviceController.FocusControl.SupportedFocusModes.Contains(FocusMode.Continuous);
                this.FlashSupported               = this.MediaCapture.VideoDeviceController.FlashControl.Supported;

                if (this.FocusSupported)
                {
                    this.ConfigureAutoFocus(continuous: false);

                    if (this.MediaCapture.VideoDeviceController.FocusControl.FocusChangedSupported)
                    {
                        this.MediaCapture.FocusChanged += this.MediaCaptureFocusChanged;
                    }
                }

                this.NotifyPropertiesChanged();
            }
            catch (Exception e)
            {
                Tracing.Trace("CameraController: StartPreviewAsync: 0x{0:X8}\r\n{1}", e.HResult, e);
                throw;
            }

            Tracing.Trace("CameraController: Video preview started.");
        }

        /// <summary>
        /// Stops the video preview, if needed.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="InvalidOperationException">The current <see cref="CameraController"/> is not initialized.</exception>
        /// <exception cref="ObjectDisposedException">The current <see cref="CameraController"/> is disposed.</exception>
        public async Task StopPreviewAsync()
        {
            Tracing.Trace("CameraController: Stopping video preview.");

            if (string.IsNullOrEmpty(this.PreviewVideoPort))
            {
                Tracing.Trace("CameraController: Video preview is already stopped.");
                return;
            }

            try
            {
                await this.MediaCapture.StopPreviewAsync();

                this.MediaCapture.FocusChanged -= this.MediaCaptureFocusChanged;

                this.PreviewBrush      = null;
                this.PreviewResolution = Size.Empty;
                this.PreviewVideoPort  = string.Empty;
                this.PreviewFormat     = null;

                this.NotifyPropertiesChanged();
            }
            catch (Exception e)
            {
                Tracing.Trace("CameraController: StopPreviewAsync: 0x{0:X8}\r\n{1}", e.HResult, e);
                throw;
            }

            Tracing.Trace("CameraController: Video preview stopped.");
        }

        /// <summary>
        /// Enables and starts the continuous automatic focus, for those devices that support it.
        /// </summary>
        /// <returns>Asynchronous action.</returns>
        /// <exception cref="InvalidOperationException">Current device does not support the continuous auto-focus.</exception>
        /// <remarks>
        /// Calling into the <see cref="FocusAsync"/>, <see cref="FocusAtPointAsync"/>, and <see cref="ResetFocusAsync"/>
        /// resets this setting, so you need to re-enable it later.
        /// </remarks>
        public async Task<IAsyncAction> EnableContinuousAutoFocusAsync()
        {
            if (!this.ContinuousAutoFocusSupported)
            {
                throw new InvalidOperationException("Continuous auto focus is not supported by the current device.");
            }

            Tracing.Trace("CameraController: Enabling continuous auto-focus.");

            IAsyncAction focusAction;

            try
            {
                this.ConfigureAutoFocus(continuous: true);

                if (this.FocusAtPointSupported)
                {
                    await this.MediaCapture.VideoDeviceController.RegionsOfInterestControl.ClearRegionsAsync();
                }

                focusAction = this.MediaCapture.VideoDeviceController.FocusControl.FocusAsync();
            }
            catch (Exception e)
            {
                Tracing.Trace("CameraController: EnableContinuousAutoFocusAsync: 0x{0:X8}\r\n{1}", e.HResult, e);
                throw;
            }

            return focusAction;
        }

        /// <summary>
        /// Performs the auto-focus operation.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="InvalidOperationException">The current device doesn't support focus operations.</exception>
        /// <seealso cref="BeginFocus"/>
        public async Task FocusAsync()
        {
            try
            {
                this.ConfigureAutoFocus(continuous: false);

                IAsyncAction focusAction = await this.BeginFocus();
                await focusAction;
            }
            catch (Exception e)
            {
                Tracing.Trace("CameraController: FocusAsync: 0x{0:X8}\r\n{1}", e.HResult, e);
                throw;
            }
        }

        /// <summary>
        /// Auto-focuses camera on the specific point in the viewfinder, for those devices that support it.
        /// </summary>
        /// <param name="focusPoint">
        /// Point of interest.<br/>
        /// It should be normalized to <c>1.0</c>.
        /// </param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="focusPoint"/> X value is lesser than <c>0.0</c> or greater than <c>1.0</c>.
        ///     <para>-or-</para>
        /// <paramref name="focusPoint"/> Y is lesser than <c>0.0</c> or greater than <c>1.0</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">The current device doesn't support focus at point operations.</exception>
        /// <seealso cref="BeginFocusAtPoint"/>
        public async Task FocusAtPointAsync(Point focusPoint)
        {
            try
            {
                this.ConfigureAutoFocus(continuous: false);

                IAsyncAction focusAction = await this.BeginFocusAtPoint(focusPoint);
                await focusAction;
            }
            catch (Exception e)
            {
                Tracing.Trace("CameraController: FocusAtPointAsync: 0x{0:X8}\r\n{1}", e.HResult, e);
                throw;
            }
        }

        /// <summary>
        /// Resets the current focus.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="InvalidOperationException">The current device doesn't support focus operations.</exception>
        public async Task ResetFocusAsync()
        {
            try
            {
                await this.BeginResetFocus();
            }
            catch (Exception e)
            {
                Tracing.Trace("CameraController: ResetFocusAsync: 0x{0:X8}\r\n{1}", e.HResult, e);
                throw;
            }
        }

        /// <summary>
        /// Sets the media format for the <paramref name="streamType"/> given.
        /// </summary>
        /// <param name="streamType">Type of the stream to set format for.</param>
        /// <param name="formatSize">The desired format area size.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="formatSize"/> is invalid.</exception>
        /// <exception cref="ArgumentException"><paramref name="streamType"/> is not supported.</exception>
        /// <exception cref="InvalidOperationException">
        /// No suitable media formats found for the <paramref name="streamType"/> given.
        /// </exception>
        public Task SetMediaFormatAsync(MediaStreamType streamType, Size formatSize)
        {
            if (formatSize.Height <= 0.0 || formatSize.Width <= 0.0)
            {
                throw new ArgumentOutOfRangeException("formatSize", formatSize, "Format size is invalid.");
            }

            switch (streamType)
            {
                case MediaStreamType.Photo:
                case MediaStreamType.VideoRecord:
                    // Valid stream types.
                    break;
                case MediaStreamType.VideoPreview:
                    throw new ArgumentException("Unable to set video preview format. Use 'StartPreviewAsync' methods instead.");
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Stream type {0} is not supported.", streamType));
            }

            return this.DoSetMediaFormatAsync(streamType, CameraController.CreateMediaFormatSelector(formatSize));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion // Public methods

        #region Internal methods

        /// <summary>
        /// Starts the auto focus operation.
        /// </summary>
        /// <returns>Asynchronous action.</returns>
        /// <exception cref="InvalidOperationException">The current device doesn't support focus operations.</exception>
        internal async Task<IAsyncAction> BeginFocus()
        {
            if (!this.FocusSupported)
            {
                throw new InvalidOperationException("Focus is not supported by the current device.");
            }

            Tracing.Trace("CameraController: Starting auto focus.");

            if (this.FocusAtPointSupported)
            {
                await this.MediaCapture.VideoDeviceController.RegionsOfInterestControl.ClearRegionsAsync();
            }

            return this.MediaCapture.VideoDeviceController.FocusControl.FocusAsync();
        }

        /// <summary>
        /// Starts a camera auto focus operation on the specific point in the viewfinder, for those devices that support it.
        /// </summary>
        /// <param name="focusPoint">
        /// Point of interest.<br/>
        /// It should be normalized to <c>1.0</c>.
        /// </param>
        /// <returns>Asynchronous action.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="focusPoint"/> X value is lesser than <c>0.0</c> or greater than <c>1.0</c>.
        ///     <para>-or-</para>
        /// <paramref name="focusPoint"/> Y is lesser than <c>0.0</c> or greater than <c>1.0</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">The current device doesn't support focus at point operations.</exception>
        internal async Task<IAsyncAction> BeginFocusAtPoint(Point focusPoint)
        {
            if (!this.FocusAtPointSupported)
            {
                throw new InvalidOperationException("Focus at point is not supported by the current device.");
            }

            if (focusPoint.X < 0.0 || focusPoint.X > 1.0)
            {
                throw new ArgumentOutOfRangeException("focusPoint", focusPoint, "Horizontal location in the viewfinder should be between 0.0 and 1.0.");
            }

            if (focusPoint.Y < 0.0 || focusPoint.Y > 1.0)
            {
                throw new ArgumentOutOfRangeException("focusPoint", focusPoint, "Vertical location in the viewfinder should be between 0.0 and 1.0.");
            }

            double x = focusPoint.X;
            double y = focusPoint.Y;
            double epsilon = 0.01;

            // 'x + width' and 'y + height' should be less than 1.0.
            if (x >= 1.0 - epsilon)
            {
                x = 1.0 - 2 * epsilon;
            }

            if (y >= 1.0 - 0.01)
            {
                y = 1.0 - 2 * epsilon;
            }

            Tracing.Trace("CameraController: Starting focus at point {0:0.00} {1:0.00}.", x, y);

            this.ConfigureAutoFocus(continuous: false);

            await this.MediaCapture.VideoDeviceController.RegionsOfInterestControl.SetRegionsAsync(
                new[]
                {
                    new RegionOfInterest
                    {
                        Type             = RegionOfInterestType.Unknown,
                        Bounds           = new Windows.Foundation.Rect(x, y, epsilon, epsilon),
                        BoundsNormalized = true,
                        AutoFocusEnabled = true,
                        Weight           = 1
                    }
                });

            return this.MediaCapture.VideoDeviceController.FocusControl.FocusAsync();
        }

        /// <summary>
        /// Starts the focus unlocking operation.
        /// </summary>
        /// <returns>Asynchronous action.</returns>
        /// <exception cref="InvalidOperationException">The current device doesn't support focus operations.</exception>
        internal IAsyncAction BeginResetFocus()
        {
            if (!this.FocusSupported)
            {
                throw new InvalidOperationException("Focus is not supported by the current device.");
            }

            Tracing.Trace("CameraController: Resetting focus.");

            return this.MediaCapture.VideoDeviceController.FocusControl.UnlockAsync();
        }

        #endregion // Internal methods

        #region Protected methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ResetCaptureDevice(notifyProperties: false);
            }
        }

        #endregion // Protected methods

        #region Private methods

        /// <summary>
        /// Asynchronously sets the media format for the <paramref name="streamType"/> given.
        /// </summary>
        /// <param name="streamType">Type of the stream to set format for.</param>
        /// <param name="formatSelector">
        /// Predicate that selects the most suitable format from the collection provided.
        /// </param>
        /// <returns>Format set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatSelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="streamType"/> is not supported.</exception>
        /// <exception cref="InvalidOperationException">
        /// No suitable media formats found for the <paramref name="streamType"/> given.
        /// </exception>
        private async Task<VideoEncodingProperties> DoSetMediaFormatAsync(MediaStreamType streamType, Func<IEnumerable<VideoEncodingProperties>, VideoEncodingProperties> formatSelector)
        {
            if (formatSelector == null)
            {
                throw new ArgumentNullException("formatSelector");
            }

            IEnumerable<VideoEncodingProperties> availableFormats;
            switch (streamType)
            {
                case MediaStreamType.Photo:
                    availableFormats = this.AvailablePhotoEncodings;
                    break;
                case MediaStreamType.VideoPreview:
                    availableFormats = this.AvailablePreviewEncodings;
                    break;
                case MediaStreamType.VideoRecord:
                    availableFormats = this.AvailableVideoEncodings;
                    break;
                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Stream type {0} is not supported.", streamType));
            }

            VideoEncodingProperties format = formatSelector(availableFormats);
            if (format == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "No suitable media formats found for stream type {0}.", streamType));
            }

            Tracing.Trace("CameraController: Setting media format ({0}) Width: {1}, Height: {2}, Type: {3}.", streamType, format.Width, format.Height, format.Subtype);
            await this.MediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(streamType, format);

            return format;
        }

        /// <summary>
        /// Creates a new selector used in the <see cref="StartPreviewAsync(Size)"/> and <see cref="SetMediaFormatAsync(MediaStreamType,Size)"/>
        /// that determines the video format to be used.
        /// </summary>
        /// <param name="formatSize">The desired format area size.</param>
        /// <returns>The desired video format to be used.</returns>
        private static Func<IEnumerable<VideoEncodingProperties>, VideoEncodingProperties> CreateMediaFormatSelector(Size formatSize)
        {
            return encodingProperties =>
            {
                if (encodingProperties == null)
                {
                    return null;
                }

                List<VideoEncodingProperties> validProperties = encodingProperties.Where(CameraController.IsValidFormat).ToList();

                // The more the item's area size differs from the requested size, the farther it will be in the list.
                double area = formatSize.Height * formatSize.Width;
                validProperties.Sort((p1, p2) => Math.Abs(p1.Height * p1.Width - area).CompareTo(Math.Abs(p2.Height * p2.Width - area)));

                return validProperties.FirstOrDefault();
            };
        }

        /// <summary>
        /// Predicate used to filter only the suitable encoding properties.
        /// </summary>
        /// <param name="properties">Encoding properties.</param>
        /// <returns>Whether the encoding <paramref name="properties"/> can be used.</returns>
        private static bool IsValidFormat(VideoEncodingProperties properties)
        {
            if (properties == null || properties.Width == 0 || properties.Height == 0 || string.IsNullOrEmpty(properties.Subtype))
            {
                return false;
            }

            return CameraController.SupportedFormats.Contains(properties.Subtype, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Reconfigures the focus control on the current video device.
        /// </summary>
        /// <param name="continuous">Whether the continuous auto focus should be set.</param>
        private void ConfigureAutoFocus(bool continuous)
        {
            Tracing.Trace("CameraController: Configuring {0} auto-focus.", continuous ? "continuous" : "normal");

            FocusSettings focusSettings = continuous
                                          ? new FocusSettings { Mode = FocusMode.Continuous, AutoFocusRange = AutoFocusRange.Normal }
                                          : new FocusSettings { Mode = FocusMode.Single,     AutoFocusRange = AutoFocusRange.FullRange };

            this.MediaCapture.VideoDeviceController.FocusControl.Configure(focusSettings);
        }

        /// <summary>
        /// Resets the current capture device and its properties to their default values.
        /// </summary>
        /// <param name="notifyProperties">Whether to raise the property changed events.</param>
        private void ResetCaptureDevice(bool notifyProperties)
        {
            if (this.MediaCapture != null)
            {
                // We want to stop preview synchronously, because this method is called from constructor and on cleanup.
                this.StopPreviewAsync().GetAwaiter().GetResult();

                this.MediaCapture.Failed                   -= this.MediaCaptureFailed;
                this.MediaCapture.RecordLimitationExceeded -= this.MediaCaptureRecordLimitationExceeded;

                this.MediaCapture.Dispose();
                this.MediaCapture = null;
            }

            this.PreviewVideoPort             = string.Empty;
            this.PreviewFormat                = null;
            this.Rotation                     = VideoRotation.None;
            this.FocusAtPointSupported        = false;
            this.ContinuousAutoFocusSupported = false;
            this.FocusSupported               = false;
            this.FlashSupported               = false;
            this.PreviewBrush                 = null;
            this.PreviewResolution            = Size.Empty;
            this.DeviceInformation            = null;
            this.AvailablePreviewEncodings    = new List<VideoEncodingProperties>().AsReadOnly();
            this.AvailablePhotoEncodings      = new List<VideoEncodingProperties>().AsReadOnly();
            this.AvailableVideoEncodings      = new List<VideoEncodingProperties>().AsReadOnly();

            if (notifyProperties)
            {
                this.NotifyPropertiesChanged();
            }
        }

        /// <summary>
        /// Called when capture device changes focus.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="MediaCaptureFocusChangedEventArgs"/> instance containing the event data.</param>
        private void MediaCaptureFocusChanged(MediaCapture sender, MediaCaptureFocusChangedEventArgs e)
        {
            EventHandler<MediaCaptureFocusChangedEventArgs> handler = this.FocusChanged;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        /// <summary>
        /// The <see cref="MediaCapture.RecordLimitationExceeded"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        private void MediaCaptureRecordLimitationExceeded(MediaCapture sender)
        {
            Tracing.Trace("CameraController: MediaCaptureRecordLimitationExceeded: {0}", sender);
        }

        /// <summary>
        /// The <see cref="MediaCapture.Failed"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="MediaCaptureFailedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">Failure details.</exception>
        private void MediaCaptureFailed(MediaCapture sender, MediaCaptureFailedEventArgs e)
        {
            Tracing.Trace("CameraController: MediaCaptureFailed: {0} (0x{1:X8}).", e.Message, e.Code);
            throw new InvalidOperationException(e.Message);
        }

        #endregion // Private methods
    }
}
