// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="MainPageViewModel.cs">
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

namespace CameraSampleCS.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera;
    using CameraSampleCS.Models.Camera.Capture;
    using CameraSampleCS.Models.Settings;
    using CameraSampleCS.Resources;
    using CameraSampleCS.Services.Camera;
    using CameraSampleCS.Services.Navigation;
    using CameraSampleCS.Services.Settings;
    using CameraSampleCS.Services.Storage;
    using CameraSampleCS.Views;
    using Microsoft.Devices;
    using Microsoft.Phone.Controls;
    using Windows.Media.Capture;
    using Windows.Media.Devices;
    using Windows.Media.MediaProperties;

    using FlashMode   = CameraSampleCS.Models.Camera.FlashMode;
    using PhotoCamera = CameraSampleCS.Models.Camera.PhotoCamera;

    /// <summary>
    /// View model class for the <see cref="MainPage"/>.
    /// </summary>
    public class MainPageViewModel : BindableBase, IDisposable
    {
        #region Fields

        /// <summary>
        /// The <see cref="State"/> property's name.
        /// </summary>
        public const string StatePropertyName = "State";

        /// <summary>
        /// The <see cref="Items"/> property's name.
        /// </summary>
        public const string ItemsPropertyName = "Items";

        /// <summary>
        /// The <see cref="CameraType"/> property's name.
        /// </summary>
        public const string CameraTypePropertyName = "CameraType";

        /// <summary>
        /// The <see cref="ScreenFormat"/> property's name.
        /// </summary>
        public const string ScreenFormatPropertyName = "ScreenFormat";

        /// <summary>
        /// The <see cref="FlashMode"/> property's name.
        /// </summary>
        public const string FlashModePropertyName = "FlashMode";

        /// <summary>
        /// The <see cref="CaptureMode"/> property's name.
        /// </summary>
        public const string CaptureModePropertyName = "CaptureMode";

        /// <summary>
        /// The <see cref="PreviewBrush"/> property's name.
        /// </summary>
        public const string PreviewBrushPropertyName = "PreviewBrush";

        /// <summary>
        /// The <see cref="ReviewImageBrush"/> property's name.
        /// </summary>
        public const string ReviewImageBrushPropertyName = "ReviewImageBrush";

        /// <summary>
        /// The <see cref="PreviewSize"/> property's name.
        /// </summary>
        public const string PreviewSizePropertyName = "PreviewSize";

        /// <summary>
        /// Navigation service instance.
        /// </summary>
        private readonly INavigationService navigationService;

        /// <summary>
        /// Camera provider instance.
        /// </summary>
        private readonly ICameraProvider cameraProvider;

        /// <summary>
        /// Storage service instance.
        /// </summary>
        private readonly IStorageService storageService;

        /// <summary>
        /// Application settings provider.
        /// </summary>
        private readonly ISettingsProvider settingsProvider;

        /// <summary>
        /// Application-wide settings.
        /// </summary>
        private readonly IApplicationSettings applicationSettings;

        /// <summary>
        /// Current model state.
        /// </summary>
        private MainPageViewModelState state;

        /// <summary>
        /// Collection of items to show in the <see cref="MediaViewer"/>.
        /// </summary>
        private ObservableCollection<object> items;

        /// <summary>
        /// Camera-specific settings.
        /// </summary>
        private ICameraSettings cameraSettings;

        /// <summary>
        /// Current camera device.
        /// </summary>
        private PhotoCamera camera;

        /// <summary>
        /// Camera video preview brush.
        /// </summary>
        private Brush previewBrush;

        /// <summary>
        /// Review image.
        /// </summary>
        private Brush reviewImageBrush;

        /// <summary>
        /// Current preview size.
        /// </summary>
        private Size previewSize;

        /// <summary>
        /// Size of the preview area.
        /// </summary>
        private Size previewAreaSize;

        /// <summary>
        /// Current page orientation.
        /// </summary>
        private PageOrientation orientation = PageOrientation.None;

        /// <summary>
        /// Whether the continuous auto focus is enabled for the current camera device.
        /// </summary>
        /// <remarks>
        /// This variable is used to filter the <see cref="BasicCamera.FocusChanged"/> events.<br/>
        /// We want to show auto-focus brackets in the viewfinder only when continuous auto-focus is active.
        /// </remarks>
        private bool continuousAutoFocusEnabled;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">Navigation service.</param>
        /// <param name="cameraProvider">Camera provider.</param>
        /// <param name="storageService">Phone storage service.</param>
        /// <param name="settingsProvider">Settings provider.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="navigationService"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="cameraProvider"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="storageService"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="settingsProvider"/> is <see langword="null"/>.
        /// </exception>
        public MainPageViewModel(INavigationService navigationService, ICameraProvider cameraProvider, IStorageService storageService, ISettingsProvider settingsProvider)
        {
            if (navigationService == null)
            {
                throw new ArgumentNullException("navigationService");
            }

            if (cameraProvider == null)
            {
                throw new ArgumentNullException("cameraProvider");
            }

            if (storageService == null)
            {
                throw new ArgumentNullException("storageService");
            }

            if (settingsProvider == null)
            {
                throw new ArgumentNullException("settingsProvider");
            }

            this.navigationService   = navigationService;
            this.cameraProvider      = cameraProvider;
            this.storageService      = storageService;
            this.settingsProvider    = settingsProvider;
            this.items               = new ObservableCollection<object>();
            this.state               = MainPageViewModelState.Unloaded;
            this.applicationSettings = settingsProvider.GetApplicationSettings();
            this.cameraSettings      = settingsProvider.GetCameraSettings(this.applicationSettings.CameraType);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        ~MainPageViewModel()
        {
            this.Dispose(false);
        }

        #endregion // Constructor

        #region Events

        /// <summary>
        /// Occurs when camera device is initialized.
        /// </summary>
        public event EventHandler CameraLoaded;

        /// <summary>
        /// Occurs when camera device is unitialized.
        /// </summary>
        public event EventHandler CameraUnloaded;

        /// <summary>
        /// Occurs when camera focus state changes.
        /// </summary>
        public event EventHandler<FocusStateChangedEventArgs> FocusStateChanged;

        /// <summary>
        /// Occurs when photo capture stops.
        /// </summary>
        public event EventHandler CaptureStopped;

        #endregion // Events

        #region Properties

        /// <summary>
        /// Gets or sets the page orientation.
        /// </summary>
        public PageOrientation Orientation
        {
            get
            {
                return this.orientation;
            }

            set
            {
                if (this.orientation == value)
                {
                    return;
                }

                this.orientation = value;

                if (this.camera != null)
                {
                    this.camera.Orientation = value;
                }
            }
        }

        /// <summary>
        /// Gets the current model state.
        /// </summary>
        public MainPageViewModelState State
        {
            get
            {
                return this.state;
            }

            private set
            {
                if (this.state == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.state = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the collection of media items to show.
        /// </summary>
        public ObservableCollection<object> Items
        {
            get
            {
                return this.items;
            }

            private set
            {
                if (this.items == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.items = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the type of the currently managed camera.
        /// </summary>
        public CameraType CameraType
        {
            get
            {
                return this.applicationSettings.CameraType;
            }

            private set
            {
                if (this.applicationSettings.CameraType == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.applicationSettings.CameraType = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current camera screen format.
        /// </summary>
        public ScreenFormat ScreenFormat
        {
            get
            {
                return this.cameraSettings.ScreenFormat;
            }

            private set
            {
                if (this.cameraSettings.ScreenFormat == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.cameraSettings.ScreenFormat = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the camera flash mode.
        /// </summary>
        public FlashMode FlashMode
        {
            get
            {
                return this.cameraSettings.FlashMode;
            }

            private set
            {
                if (this.cameraSettings.FlashMode == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.cameraSettings.FlashMode = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the camera capture mode.
        /// </summary>
        public CaptureMode CaptureMode
        {
            get
            {
                return this.cameraSettings.CaptureMode;
            }

            private set
            {
                if (this.cameraSettings.CaptureMode == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.cameraSettings.CaptureMode = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current video preview brush.
        /// </summary>
        public Brush PreviewBrush
        {
            get
            {
                return this.previewBrush;
            }

            private set
            {
                if (this.previewBrush == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.previewBrush = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the review image.
        /// </summary>
        public Brush ReviewImageBrush
        {
            get
            {
                return this.reviewImageBrush;
            }

            private set
            {
                if (this.reviewImageBrush == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.reviewImageBrush = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the size of the preview.
        /// </summary>
        public Size PreviewSize
        {
            get
            {
                return this.previewSize;
            }

            private set
            {
                if (this.previewSize == value)
                {
                    return;
                }

                this.NotifyPropertyChanging();
                this.previewSize = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether front-facing camera is supported on the phone.
        /// </summary>
        public bool FrontFacingPhotoCameraSupported
        {
            get
            {
                return this.cameraProvider.FrontFacingPhotoCameraSupported;
            }
        }

        /// <summary>
        /// Gets a value indicating whether flash is supported by the current camera device.
        /// </summary>
        public bool FlashSupported { get; private set; }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Loads camera device, if needed.
        /// </summary>
        /// <param name="areaSize">The size of the area available for the video preview.</param>
        /// <returns>Awaitable task.</returns>
        public async Task EnsureCameraLoadedAsync(Size areaSize)
        {
            if (this.camera != null)
            {
                // Make sure the the correct camera is loaded.
                if (this.camera.CameraType == this.CameraType && this.State != MainPageViewModelState.Unloaded)
                {
                    Tracing.Trace("MainPageViewModel: {0} camera is already loaded.", this.CameraType);
                    return;
                }

                await this.UnloadCameraAsync();
            }

            this.State = MainPageViewModelState.Loading;

            this.camera             = await this.cameraProvider.GetCameraAsync(this.CameraType);
            this.cameraSettings     = this.settingsProvider.GetCameraSettings(this.CameraType);
            this.camera.Orientation = this.orientation;
            this.previewAreaSize    = areaSize;

            await this.camera.LoadCameraAsync();
            await this.StartPreviewAsync(this.previewAreaSize);

            // Allow camera to automatically focus without user requests, if supported.
            await this.TryToEnableContinuousAutoFocusAsync();

            this.camera.FocusChanged   += this.CameraOnFocusChanged;
            this.camera.PhotoCaptured  += this.CameraOnPhotoCaptured;
            this.camera.CaptureStopped += this.CameraOnCaptureStopped;

            this.PreviewSize    = this.camera.PreviewSize;
            this.PreviewBrush   = this.camera.PreviewBrush;
            this.FlashSupported = this.camera.FlashSupported;

            await this.UpdatePhotoCaptureAsync();

            this.State = MainPageViewModelState.Loaded;

            EventHandler handler = this.CameraLoaded;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Unloads camera device, if needed.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public async Task UnloadCameraAsync()
        {
            if (this.camera == null || this.State == MainPageViewModelState.Unloaded)
            {
                return;
            }

            this.State        = MainPageViewModelState.Unloaded;
            this.PreviewBrush = null;

            this.continuousAutoFocusEnabled = false;

            this.camera.FocusChanged   -= this.CameraOnFocusChanged;
            this.camera.PhotoCaptured  -= this.CameraOnPhotoCaptured;
            this.camera.CaptureStopped -= this.CameraOnCaptureStopped;

            await this.camera.UnloadCameraAsync();
            this.camera = null;

            EventHandler handler = this.CameraUnloaded;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Focuses the camera on the <paramref name="focusPoint"/> specified.
        /// </summary>
        /// <param name="focusPoint">
        /// Point of intereset to focus on.<br/>
        /// If the <paramref name="focusPoint"/> is not set, camera will focus automatically.
        /// </param>
        /// <returns>Awaitable task.</returns>
        public Task FocusCameraAsync(Point? focusPoint)
        {
            return this.DoFocusCameraAsync(focusPoint);
        }

        /// <summary>
        /// Cancels the current focus operation, if any.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public async Task CancelFocusAsync()
        {
            if (this.State == MainPageViewModelState.FocusInProgress)
            {
                if (this.camera.FocusSupported)
                {
                    this.camera.CancelFocus();
                }

                this.State = MainPageViewModelState.Loaded;
            }

            if (this.State == MainPageViewModelState.Loaded)
            {
                this.NotifyFocusStateChanged(new FocusStateChangedEventArgs(FocusState.Unlocked));
            }

            if (this.camera.FocusSupported)
            {
                await this.camera.ResetFocusAsync();
                this.NotifyFocusStateChanged(new FocusStateChangedEventArgs(FocusState.Unlocked));
            }
        }

        /// <summary>
        /// Starts the photo capture.
        /// </summary>
        /// <param name="focusPoint">
        /// Point of interest to focus on.<br/>
        /// If the <paramref name="focusPoint"/> is not set, camera will focus automatically.
        /// </param>
        /// <returns>Awaitable task.</returns>
        public async Task StartPhotoCaptureAsync(Point? focusPoint)
        {
            if (this.State == MainPageViewModelState.CaptureInProgress)
            {
                return;
            }

            await this.DoFocusCameraAsync(focusPoint);

            this.State = MainPageViewModelState.CaptureInProgress;

            await this.camera.StartPhotoCaptureAsync();
        }

        /// <summary>
        /// Toggles the current <see cref="CameraType"/> and prepares a new camera device.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public async Task ToggleBackFrontFacingCameraAsync()
        {
            if (this.CameraType == CameraType.FrontFacing)
            {
                this.CameraType = CameraType.Primary;
            }
            else
            {
                this.CameraType = CameraType.FrontFacing;
            }

            // Make sure we hide the focus brackets.
            this.NotifyFocusStateChanged(new FocusStateChangedEventArgs(FocusState.Unlocked));

            await this.UnloadCameraAsync();
            await this.EnsureCameraLoadedAsync(this.previewAreaSize);
        }

        /// <summary>
        /// Changes the current <see cref="FlashMode"/> value.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public async Task ToggleFlashModeAsync()
        {
            if (!this.FlashSupported)
            {
                return;
            }

            FlashMode newFlashMode;

            switch (this.FlashMode)
            {
                case FlashMode.Off:
                    newFlashMode = FlashMode.On;
                    break;
                case FlashMode.On:
                    newFlashMode = FlashMode.Auto;
                    break;
                case FlashMode.Auto:
                    newFlashMode = FlashMode.Off;
                    break;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Flash mode {0} is not supported.", this.FlashMode));
            }

            this.FlashMode = newFlashMode;

            await this.UpdatePhotoCaptureAsync();
        }

        /// <summary>
        /// Changes the current <see cref="CaptureMode"/> value.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public Task ToggleCaptureModeAsync()
        {
            CaptureMode newCaptureMode;

            switch (this.CaptureMode)
            {
                case CaptureMode.LowLag:
                    newCaptureMode = CaptureMode.Variable;
                    break;
                case CaptureMode.Variable:
                    newCaptureMode = CaptureMode.LowLag;
                    break;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Capture mode {0} is not supported.", this.CaptureMode));
            }

            this.CaptureMode = newCaptureMode;

            return this.UpdatePhotoCaptureAsync();
        }

        /// <summary>
        /// Navigates to the <see cref="AboutPage"/> page.
        /// </summary>
        public void ShowAboutPage()
        {
            this.navigationService.NavigateTo(Constants.AboutPageUri);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // Public methods

        #region Private methods

        #region Preview

        /// <summary>
        /// Starts the camera video preview.
        /// </summary>
        /// <param name="areaSize">Size of the preview area.</param>
        /// <returns>Awaitable task.</returns>
        private Task StartPreviewAsync(Size areaSize)
        {
            Size previewResolution = this.FindHighestSupportedPreviewResolution(areaSize);
            return this.camera.StartPreviewAsync(previewResolution);
        }

        /// <summary>
        /// Looks for the highest supported camera video preview resolution for the current screen format.
        /// </summary>
        /// <param name="areaSize">Preview area size.</param>
        /// <returns>Resolution found.</returns>
        private Size FindHighestSupportedPreviewResolution(Size areaSize)
        {
            ScreenFormat format = this.ScreenFormat;

            uint maxArea = unchecked((uint)(areaSize.Width * areaSize.Height));

            IEnumerable<Size> supportedResolutions = this.camera.SupportedPreviewResolutions;

            // For the 16:9 format camera provides a native screen-size preview size, which isn't neccessary 16:9.
            // We want to use it, if available.
            if (format == ScreenFormat.SixteenByNine)
            {
                supportedResolutions = supportedResolutions
                    .Where(r =>
                    {
                        ScreenFormat currentFormat = this.GetScreenFormat(new Size(r.Width, r.Height));
                        return (r.Height * r.Width) < maxArea && (currentFormat == format || currentFormat == ScreenFormat.Unknown);
                    });
            }
            else
            {
                supportedResolutions = supportedResolutions.Where(r => (r.Height * r.Width) < maxArea && (this.GetScreenFormat(new Size(r.Width, r.Height)) == format));
            }

            List<Size> supportedResolutionsList = supportedResolutions.ToList();
            return supportedResolutionsList.Aggregate(supportedResolutionsList.First(), (max, current) => (max.Height * max.Width) < (current.Height * current.Width) ? current : max);
        }

        /// <summary>
        /// Looks for the highest supported camera photo resolution for the current screen format.
        /// </summary>
        /// <returns>Resolution found.</returns>
        private Size FindHighestSupportedPhotoResolution()
        {
            ScreenFormat format = this.ScreenFormat;

            List<Size> supportedResolutions = this.camera.SupportedPhotoResolutions
                .Where(r => r.Height * r.Width <= Constants.MaxSupportedResolution && this.GetScreenFormat(new Size(r.Width, r.Height)) == format)
                .ToList();

            return supportedResolutions.Aggregate(supportedResolutions.First(), (max, current) => (max.Height * max.Width) < (current.Height * current.Width) ? current : max);
        }

        /// <summary>
        /// Converts the <paramref name="resolution"/> given to a proper <see cref="ScreenFormat"/> value.
        /// </summary>
        /// <param name="resolution">Screen resolution to convert.</param>
        /// <returns><see cref="ScreenFormat"/> value.</returns>
        private ScreenFormat GetScreenFormat(Size resolution)
        {
            ScreenFormat result = ScreenFormat.Unknown;

            double relation = Math.Max(resolution.Width, resolution.Height) / Math.Min(resolution.Width, resolution.Height);
            if (Math.Abs(relation - (4.0 / 3.0)) < 0.01)
            {
                result = ScreenFormat.FourByThree;
            }
            else if (Math.Abs(relation - (16.0 / 9.0)) < 0.01)
            {
                result = ScreenFormat.SixteenByNine;
            }

            return result;
        }

        #endregion // Preview

        #region Focus

        /// <summary>
        /// Enables the continuous automatic focus for the current device, if it's supported
        /// by the current camera device.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        private async Task TryToEnableContinuousAutoFocusAsync()
        {
            if (!this.camera.ContinuousAutoFocusSupported)
            {
                return;
            }

            await this.camera.EnableContinuousAutoFocusAsync();
            this.continuousAutoFocusEnabled = true;
        }

        /// <summary>
        /// Asynchronously focuses the camera on the <paramref name="focusPoint"/> specified.
        /// </summary>
        /// <param name="focusPoint">Point of intereset to focus on.</param>
        /// <returns>Awaitable task.</returns>
        private async Task DoFocusCameraAsync(Point? focusPoint)
        {
            if (!this.camera.FocusSupported || this.State != MainPageViewModelState.Loaded)
            {
                return;
            }

            this.NotifyFocusStateChanged(new FocusStateChangedEventArgs(FocusState.Focusing, false, focusPoint));
            this.State = MainPageViewModelState.FocusInProgress;

            System.Windows.Point? point = focusPoint.HasValue && this.camera.FocusAtPointSupported
                                          ? (System.Windows.Point?)new System.Windows.Point(focusPoint.Value.X, focusPoint.Value.Y)
                                          : null;

            // Once user taps on the viewfinder (tap point is not NULL), a new focus region will be set, and the continuous
            // auto-focus will be disabled, and it should be re-enabled later. We can do it in the 'CameraOnCaptureStopped'
            // method, because we take picture right after touch focus completes.
            // Setting this value to 'false' will not cause the auto-focus brackets to be shown on focus events.
            this.continuousAutoFocusEnabled = !point.HasValue;

            await this.camera.FocusAsync(point);

            this.NotifyFocusStateChanged(new FocusStateChangedEventArgs(FocusState.Locked, false, focusPoint));
        }

        /// <summary>
        /// Notifies the <see cref="FocusStateChanged"/> event listeners.
        /// </summary>
        /// <param name="args">The <see cref="FocusStateChangedEventArgs"/> instance containing the event data.</param>
        private void NotifyFocusStateChanged(FocusStateChangedEventArgs args)
        {
            EventHandler<FocusStateChangedEventArgs> handler = this.FocusStateChanged;
            if (handler != null)
            {
                handler.Invoke(this, args);
            }
        }

        #endregion // Focus

        #region Capture

        /// <summary>
        /// Updates photo capture in accordance with the current application parameters.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        private async Task UpdatePhotoCaptureAsync()
        {
            bool needToUpdateAgain = false;
            Size resolution        = this.FindHighestSupportedPhotoResolution();

            ImageEncodingProperties encoding = ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8);
            encoding.Width  = unchecked((uint)resolution.Width);
            encoding.Height = unchecked((uint)resolution.Height);

            try
            {
                await this.camera.InitializePhotoCaptureAsync(
                    this.CaptureMode,
                    new CaptureParameters
                    {
                        ImageEncoding = encoding,
                        FlashMode     = this.FlashMode
                    });
            }
            catch
            {
                // VPS may not be supported on the current device.
                if (this.CaptureMode == CaptureMode.Variable)
                {
                    // Fall back to the LowLag photo capture.
                    this.CaptureMode  = CaptureMode.LowLag;
                    needToUpdateAgain = true;

                    // Show the error message to the user.
                    DispatcherHelper.BeginInvokeOnUIThread(() => MessageBox.Show(AppResources.VpsNotSupportedMessage, AppResources.VpsNotSupportedCaption, MessageBoxButton.OK));
                }
                else
                {
                    throw;
                }
            }

            // Call this method again, if the VPS initialization failed, so we'll reinitialize LLPC.
            if (needToUpdateAgain)
            {
                await this.UpdatePhotoCaptureAsync();
            }
        }

        /// <summary>
        /// The <see cref="BasicCamera.FocusChanged"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="MediaCaptureFocusChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="InvalidOperationException">The <paramref name="e"/> contains unexpected focus state.</exception>
        private void CameraOnFocusChanged(object sender, MediaCaptureFocusChangedEventArgs e)
        {
            if (!this.continuousAutoFocusEnabled)
            {
                return;
            }

            FocusState focusState;

            switch (e.FocusState)
            {
                case MediaCaptureFocusState.Failed:
                case MediaCaptureFocusState.Lost:
                case MediaCaptureFocusState.Uninitialized:
                    focusState = FocusState.Unlocked;
                    break;
                case MediaCaptureFocusState.Focused:
                    focusState = FocusState.Locked;
                    break;
                case MediaCaptureFocusState.Searching:
                    focusState = FocusState.Focusing;
                    break;
                default:
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unexpected focus state received: {0}", e.FocusState));
            }

            this.NotifyFocusStateChanged(new FocusStateChangedEventArgs(focusState, true));
        }

        /// <summary>
        /// The <see cref="PhotoCamera.PhotoCaptured"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="capturedPhoto">Captured photo.</param>
        private void CameraOnPhotoCaptured(object sender, ICapturedPhoto capturedPhoto)
        {
            // In our application we capture only single frame, so we can update view model state in
            // the current method.
            // If your application captures multiple frames, you may want to modify the 'CameraOnCaptureStopped'
            // method or add additional logic here.
            DispatcherHelper.BeginInvokeOnUIThread(async () =>
            {
                this.State = MainPageViewModelState.SavingCaptureResult;
                this.NotifyFocusStateChanged(new FocusStateChangedEventArgs(FocusState.Unlocked));

                try
                {
                    using (capturedPhoto)
                    {
                        IThumbnailedImage image = await this.storageService.SaveResultToCameraRollAsync(capturedPhoto);

                        // Get the preview image.
                        BitmapImage source = new BitmapImage();
                        source.SetSource(image.GetThumbnailImage());

                        this.Items.Add(image);
                        this.ReviewImageBrush = new ImageBrush { ImageSource = source };
                    }
                }
                finally
                {
                    this.State = MainPageViewModelState.Loaded;
                }
            });
        }

        /// <summary>
        /// The <see cref="PhotoCamera.CaptureStopped"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void CameraOnCaptureStopped(object sender, EventArgs e)
        {
            // View may display the review image animation at this point.
            EventHandler handler = this.CaptureStopped;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }

            // Re-enable continuous auto-focus, if needed.
            if (!this.continuousAutoFocusEnabled)
            {
                await this.TryToEnableContinuousAutoFocusAsync();
            }
        }

        #endregion // Capture

        #region Helpers

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.UnloadCameraAsync().GetAwaiter().GetResult();
            }
        }

        #endregion // Helpers

        #endregion // Private methods
    }
}
