// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="MainPage.xaml.cs">
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

namespace CameraSampleCS.Views
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Navigation;
    using CameraSampleCS.Controls;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera;
    using CameraSampleCS.Resources;
    using CameraSampleCS.ViewModels;
    using Microsoft.Devices;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    using FlashMode = CameraSampleCS.Models.Camera.FlashMode;

    /// <summary>
    /// Main application page.
    /// </summary>
    public partial class MainPage
    {
        #region Fields

        /// <summary>
        /// Current view model.
        /// </summary>
        private readonly MainPageViewModel viewModel;

        /// <summary>
        /// Represents the set of buttons and menu items to be displayed.
        /// </summary>
        private AppBarMode appBarMode = AppBarMode.Empty;

        /// <summary>
        /// The <c>Flash Auto/On/Off</c> application bar button.
        /// </summary>
        private ApplicationBarIconButton flashButton;

        /// <summary>
        /// The <c>Front</c> application bar button that toggles the camera type.
        /// </summary>
        private ApplicationBarIconButton ffcButton;

        /// <summary>
        /// The <c>LowLag/VPS</c> application bar button that toggles the camera capture type.
        /// </summary>
        private ApplicationBarIconButton captureButton;

        /// <summary>
        /// <c>About</c> application bar menu item.
        /// </summary>
        private ApplicationBarMenuItem aboutMenuItem;

        /// <summary>
        /// Simplifies access to the current viewfinder.
        /// </summary>
        private Viewfinder viewfinder;

        /// <summary>
        /// Size of the preview area.
        /// </summary>
        private Size previewAreaSize = Constants.DefaultPreviewResolution;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.viewModel = (MainPageViewModel)this.DataContext;

            this.InitializeAppBar();
            this.UpdateApplicationBarState();
        }

        #endregion // Constructor

        #region Protected methods

        /// <summary>
        /// Called when the current page is activated.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // In spite we're subscribing to viewfinder events in the OnLoaded method, it will not be
            // called, if application is restored, so try to subscribe here also.
            this.SubscribeToViewfinderEvents();

            this.viewModel.PropertyChanging  += this.ViewModelOnPropertyChanging;
            this.viewModel.PropertyChanged   += this.ViewModelOnPropertyChanged;
            this.viewModel.CameraLoaded      += this.ViewModelOnCameraLoaded;
            this.viewModel.CameraUnloaded    += this.ViewModelOnCameraUnloaded;
            this.viewModel.FocusStateChanged += this.ViewModelOnFocusStateChanged;

            // Try to subscribe to items events.
            if (this.viewModel.Items != null)
            {
                this.viewModel.Items.CollectionChanged += this.ViewModelOnItemsCollectionChanged;
            }

            CameraButtons.ShutterKeyPressed     += this.CameraButtonsOnShutterKeyPressed;
            CameraButtons.ShutterKeyHalfPressed += this.CameraButtonsOnShutterKeyHalfPressed;
            CameraButtons.ShutterKeyReleased    += this.CameraButtonsOnShutterKeyReleased;

            await this.viewModel.EnsureCameraLoadedAsync(this.previewAreaSize);
        }

        /// <summary>
        /// Called when the current page is deactivated.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.viewModel.PropertyChanging  -= this.ViewModelOnPropertyChanging;
            this.viewModel.PropertyChanged   -= this.ViewModelOnPropertyChanged;
            this.viewModel.CameraLoaded      -= this.ViewModelOnCameraLoaded;
            this.viewModel.CameraUnloaded    -= this.ViewModelOnCameraUnloaded;
            this.viewModel.FocusStateChanged -= this.ViewModelOnFocusStateChanged;

            CameraButtons.ShutterKeyPressed     -= this.CameraButtonsOnShutterKeyPressed;
            CameraButtons.ShutterKeyHalfPressed -= this.CameraButtonsOnShutterKeyHalfPressed;
            CameraButtons.ShutterKeyReleased    -= this.CameraButtonsOnShutterKeyReleased;

            // We subscribe to the viewfinder events, when the page is fully loaded.
            this.UnsubscribeFromViewfinderEvents();

            this.UpdateApplicationBarState();

            // We want to unload camera synchronously in case application is being suspended.
            if (e.IsNavigationInitiator)
            {
                await this.viewModel.UnloadCameraAsync();
            }
            else
            {
                Task.Run(async () => await this.viewModel.UnloadCameraAsync()).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Called when the hardware <c>Back</c> button is pressed.
        /// </summary>
        /// <param name="e">
        /// Set <c>e.Cancel</c> to <see langword="true"/> to indicate that
        /// the request was handled by the application.
        /// </param>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            // Move to the viewfinder, if it's not visible.
            if (this.MediaViewer.DisplayedElement != DisplayedElementType.Footer)
            {
                e.Cancel = true;
                this.MediaViewer.JumpToFooter();
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        #endregion // Protected methods

        #region Private methods

        #region MainPage

        /// <summary>
        /// The <see cref="FrameworkElement.Loaded"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.viewfinder = (Viewfinder)this.MediaViewer.FindName("Viewfinder");
            this.SubscribeToViewfinderEvents();

            this.previewAreaSize = new Size(this.viewfinder.ActualWidth, this.viewfinder.ActualHeight);

            // Ensure the page is fully loaded before handling orientation changes for this page.
            this.OrientationChanged += this.OnOrientationChanged;

            // Apply current orientation.
            this.OnOrientationChanged(this, new OrientationChangedEventArgs(this.Orientation));
        }

        /// <summary>
        /// Called when the page orientation changes.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="OrientationChangedEventArgs"/> instance containing the event data.</param>
        private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            this.viewModel.Orientation = e.Orientation;

            if (this.viewfinder != null)
            {
                this.viewfinder.Orientation = this.Orientation;
            }
        }

        #endregion // MainPage

        #region MediaViewer

        /// <summary>
        /// The <see cref="MediaViewer.FooterDisplayed"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MediaViewerOnFooterDisplayed(object sender, EventArgs e)
        {
            this.SetAppBarMode(AppBarMode.Viewfinder);
            this.SetToCameraRollArrowVisibility(true);
        }

        /// <summary>
        /// The <see cref="MediaViewer.ItemDisplayed"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="ItemDisplayedEventArgs"/> instance containing the event data.</param>
        private void MediaViewerOnItemDisplayed(object sender, ItemDisplayedEventArgs e)
        {
            this.SetAppBarMode(AppBarMode.Review);
            this.SetToCameraRollArrowVisibility(true);
        }

        /// <summary>
        /// The <see cref="MediaViewer.ItemZoomed"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MediaViewerOnItemZoomed(object sender, EventArgs e)
        {
            DispatcherHelper.BeginInvokeOnUIThread(() => this.ApplicationBar.IsVisible = false);
        }

        /// <summary>
        /// The <see cref="MediaViewer.ItemUnzoomed"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MediaViewerOnItemUnzoomed(object sender, EventArgs e)
        {
            if (!this.ApplicationBar.IsVisible)
            {
                DispatcherHelper.BeginInvokeOnUIThread(this.RefreshAppBar);
            }
        }

        #endregion // MediaViewer

        #region Viewfinder

        /// <summary>
        /// Subscribes to the <see cref="Viewfinder"/> events, if needed.
        /// </summary>
        private void SubscribeToViewfinderEvents()
        {
            if (this.viewfinder == null)
            {
                return;
            }

            this.viewfinder.SlideReviewComplete += this.ViewfinderOnSlideReviewComplete;
            this.viewfinder.PreviewTap          += this.ViewfinderOnPreviewTap;
            this.viewfinder.LeftArrowButtonTap  += this.ViewfinderOnLeftArrowButtonTap;
        }

        /// <summary>
        /// Removes event handlers from the <see cref="Viewfinder"/>, if needed.
        /// </summary>
        private void UnsubscribeFromViewfinderEvents()
        {
            if (this.viewfinder == null)
            {
                return;
            }

            this.viewfinder.SlideReviewComplete -= this.ViewfinderOnSlideReviewComplete;
            this.viewfinder.PreviewTap          -= this.ViewfinderOnPreviewTap;
            this.viewfinder.LeftArrowButtonTap  -= this.ViewfinderOnLeftArrowButtonTap;
        }

        /// <summary>
        /// Updates the current <c>To Camera Roll</c> arrow button in accordane with the <paramref name="value"/> specified.
        /// </summary>
        /// <param name="value">Whether the button should be visible.</param>
        private void SetToCameraRollArrowVisibility(bool value)
        {
            if (this.viewfinder == null)
            {
                return;
            }

            bool visible = value && this.appBarMode == AppBarMode.Viewfinder && this.MediaViewer.Items != null && this.MediaViewer.Items.Any();
            this.viewfinder.LeftArrowButtonVisibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// The <see cref="viewfinder"/> <see cref="Viewfinder.LeftArrowButtonTap"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ViewfinderOnLeftArrowButtonTap(object sender, EventArgs e)
        {
            if (this.MediaViewer.DragEnabled)
            {
                this.MediaViewer.ScrollLeftOneElement();
            }
        }

        /// <summary>
        /// The <see cref="viewfinder"/> <see cref="Viewfinder.SlideReviewComplete"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ViewfinderOnSlideReviewComplete(object sender, EventArgs e)
        {
            this.SetChromeVisibility(true);
            this.viewfinder.ReviewImage = null;
        }

        /// <summary>
        /// The <see cref="viewfinder"/> <see cref="Viewfinder.PreviewTap"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="PreviewTapLocationEventArgs"/> instance containing the event data.</param>
        private async void ViewfinderOnPreviewTap(object sender, PreviewTapLocationEventArgs e)
        {
            if (this.viewModel.State == MainPageViewModelState.Loaded)
            {
                // If the preview is tapped, we need to focus on the user-defined region.
                await this.viewModel.StartPhotoCaptureAsync(e.PositionPercentage);
            }
        }

        #endregion // Viewfinder

        #region Application bar

        /// <summary>
        /// Creates a new application bar button based on the parameters given.
        /// </summary>
        /// <param name="iconFile">Relative path to the icon file.</param>
        /// <param name="text">Button text.</param>
        /// <param name="handler"><see cref="ApplicationBarIconButton.Click"/> event handler.</param>
        /// <returns><see cref="ApplicationBarIconButton"/> created.</returns>
        private static ApplicationBarIconButton CreateAppBarButton(string iconFile, string text, EventHandler handler)
        {
            ApplicationBarIconButton button = new ApplicationBarIconButton(new Uri(iconFile, UriKind.Relative));
            button.Text = text;
            button.Click += handler;
            return button;
        }

        /// <summary>
        /// Creates a new application bar menu item based on the parameters given.
        /// </summary>
        /// <param name="text">Menu item text.</param>
        /// <param name="handler"><see cref="ApplicationBarMenuItem.Click"/> event handler.</param>
        /// <returns><see cref="ApplicationBarMenuItem"/> created.</returns>
        private static ApplicationBarMenuItem CreateAppBarMenuItem(string text, EventHandler handler)
        {
            ApplicationBarMenuItem menuItem = new ApplicationBarMenuItem(text);
            menuItem.Click += handler;
            return menuItem;
        }

        /// <summary>
        /// Creates application bar items.
        /// </summary>
        private void InitializeAppBar()
        {
            this.ApplicationBar.Opacity = Constants.DefaultApplicationBarOpacity;

            this.flashButton   = MainPage.CreateAppBarButton("/Assets/Icons/appbar.flash.off.rest.png", AppResources.ToggleFlashAppBarButtonLabel,   this.FlashButtonOnClick);
            this.ffcButton     = MainPage.CreateAppBarButton("/Assets/Icons/appbar.ffc.dark.rest.png",  AppResources.ToggleCameraAppBarButtonLabel,  this.FFCButtonOnClick);
            this.captureButton = MainPage.CreateAppBarButton("/Assets/Icons/appbar.mode.lowlag.png",    AppResources.ToggleCaptureAppBarButtonLabel, this.CaptureButtonOnClick);

            this.aboutMenuItem = MainPage.CreateAppBarMenuItem(AppResources.AboutMenuItemLabel, this.AboutItemOnClick);
        }

        /// <summary>
        /// Updates the current application bar buttons in accordance with the <paramref name="mode"/> specified, if needed.
        /// </summary>
        /// <param name="mode">New application bar mode.</param>
        private void SetAppBarMode(AppBarMode mode)
        {
            if (this.appBarMode == mode)
            {
                return;
            }

            this.appBarMode = mode;
            this.RefreshAppBar();
        }

        /// <summary>
        /// Updates the set of items shown in the application bar.
        /// </summary>
        private void RefreshAppBar()
        {
            this.ApplicationBar.Buttons.Clear();
            this.ApplicationBar.MenuItems.Clear();

            this.ApplicationBar.MenuItems.Add(this.aboutMenuItem);

            bool appBarVisible   = true;
            bool appBarMinimized = false;

            switch (this.appBarMode)
            {
                case AppBarMode.Viewfinder:
                    this.ApplicationBar.Buttons.Add(this.flashButton);
                    this.ApplicationBar.Buttons.Add(this.ffcButton);
                    this.ApplicationBar.Buttons.Add(this.captureButton);
                    break;
                case AppBarMode.Review:
                    appBarMinimized = true;
                    break;
                default:
                    appBarVisible = false;
                    break;
            }

            this.ApplicationBar.IsVisible = appBarVisible;
            this.ApplicationBar.Mode      = appBarMinimized ? ApplicationBarMode.Minimized : ApplicationBarMode.Default;
        }

        /// <summary>
        /// Modifies buttons on the application bar in accordance with the current view model state.
        /// </summary>
        private void UpdateApplicationBarState()
        {
            this.flashButton.IsEnabled = this.viewModel.State == MainPageViewModelState.Loaded && this.viewModel.FlashSupported;

            switch (this.viewModel.FlashMode)
            {
                case FlashMode.Off:
                    this.flashButton.IconUri = new Uri("/Assets/Icons/appbar.flash.off.rest.png", UriKind.Relative);
                    break;
                case FlashMode.On:
                    this.flashButton.IconUri = new Uri("/Assets/Icons/appbar.flash.on.rest.png", UriKind.Relative);
                    break;
                case FlashMode.Auto:
                    this.flashButton.IconUri = new Uri("/Assets/Icons/appbar.flash.auto.rest.png", UriKind.Relative);
                    break;
            }

            this.ffcButton.IsEnabled = this.viewModel.State == MainPageViewModelState.Loaded && this.viewModel.FrontFacingPhotoCameraSupported;
            this.ffcButton.IconUri   = this.viewModel.CameraType == CameraType.Primary
                                       ? new Uri("/Assets/Icons/appbar.ffc.dark.rest.png", UriKind.Relative)
                                       : new Uri("/Assets/Icons/appbar.ffc.dark.down.png", UriKind.Relative);

            this.captureButton.IsEnabled = this.viewModel.State == MainPageViewModelState.Loaded;

            switch (this.viewModel.CaptureMode)
            {
                case CaptureMode.LowLag:
                    this.captureButton.IconUri = new Uri("/Assets/Icons/appbar.mode.lowlag.png", UriKind.Relative);
                    break;
                case CaptureMode.Variable:
                    this.captureButton.IconUri = new Uri("/Assets/Icons/appbar.mode.variable.png", UriKind.Relative);
                    break;
            }
        }

        /// <summary>
        /// <see cref="ApplicationBar.StateChanged"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="ApplicationBarStateChangedEventArgs"/> instance containing the event data.</param>
        private void ApplicationBarOnStateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            ApplicationBar appBar = sender as ApplicationBar;
            if (appBar != null)
            {
                appBar.Opacity = e.IsMenuVisible ? 0.99 : Constants.DefaultApplicationBarOpacity;
            }
        }

        #endregion // Application bar

        #region Buttons

        /// <summary>
        /// The <see cref="flashButton"/> <see cref="ApplicationBarIconButton.Click"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void FlashButtonOnClick(object sender, EventArgs e)
        {
            await this.viewModel.ToggleFlashModeAsync();
        }

        /// <summary>
        /// The <see cref="ffcButton"/> <see cref="ApplicationBarIconButton.Click"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void FFCButtonOnClick(object sender, EventArgs e)
        {
            await this.viewModel.ToggleBackFrontFacingCameraAsync();
        }

        /// <summary>
        /// The <see cref="captureButton"/> <see cref="ApplicationBarIconButton.Click"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void CaptureButtonOnClick(object sender, EventArgs e)
        {
            await this.viewModel.ToggleCaptureModeAsync();
        }

        /// <summary>
        /// The <see cref="aboutMenuItem"/> <see cref="ApplicationBarMenuItem.Click"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AboutItemOnClick(object sender, EventArgs e)
        {
            this.viewModel.ShowAboutPage();
        }

        #endregion // Buttons

        #region Camera buttons

        /// <summary>
        /// The <see cref="CameraButtons.ShutterKeyHalfPressed" /> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void CameraButtonsOnShutterKeyHalfPressed(object sender, EventArgs e)
        {
            if (this.MediaViewer.DisplayedElement != DisplayedElementType.Footer || this.viewModel.State != MainPageViewModelState.Loaded)
            {
                return;
            }

            // Use auto-focus.
            await this.viewModel.FocusCameraAsync(focusPoint: null);
        }

        /// <summary>
        /// The <see cref="CameraButtons.ShutterKeyReleased"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void CameraButtonsOnShutterKeyReleased(object sender, EventArgs e)
        {
            if (this.MediaViewer.DisplayedElement != DisplayedElementType.Footer || this.viewModel.State != MainPageViewModelState.FocusInProgress)
            {
                return;
            }

            await this.viewModel.CancelFocusAsync();
        }

        /// <summary>
        /// The <see cref="CameraButtons.ShutterKeyPressed"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void CameraButtonsOnShutterKeyPressed(object sender, EventArgs e)
        {
            if (this.MediaViewer.DisplayedElement == DisplayedElementType.Footer)
            {
                if (this.viewModel.State == MainPageViewModelState.Loaded || this.viewModel.State == MainPageViewModelState.FocusInProgress)
                {
                    await this.viewModel.StartPhotoCaptureAsync(focusPoint: null);
                }
            }
            else
            {
                this.MediaViewer.JumpToFooter();
            }
        }

        #endregion // Camera buttons

        #region LensViewModel

        /// <summary>
        /// The <see cref="MainPageViewModel.PropertyChanging"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="PropertyChangingEventArgs"/> instance containing the event data.</param>
        private void ViewModelOnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            switch (e.PropertyName)
            {
                case MainPageViewModel.ItemsPropertyName:
                    this.viewModel.Items.CollectionChanged -= this.ViewModelOnItemsCollectionChanged;
                    break;
            }
        }

        /// <summary>
        /// The <see cref="MainPageViewModel.PropertyChanged"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="PropertyChangingEventArgs"/> instance containing the event data.</param>
        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DispatcherHelper.InvokeOnUIThread(() =>
            {
                switch (e.PropertyName)
                {
                    case MainPageViewModel.StatePropertyName:
                        this.SetChromeVisibility(this.viewModel.State == MainPageViewModelState.Loaded);
                        this.UpdateApplicationBarState();
                        break;
                    case MainPageViewModel.ItemsPropertyName:
                        if (this.viewModel.Items != null)
                        {
                            this.viewModel.Items.CollectionChanged += this.ViewModelOnItemsCollectionChanged;
                        }

                        break;
                    case MainPageViewModel.CameraTypePropertyName:
                        if (this.viewfinder != null)
                        {
                            this.viewfinder.CameraType = this.viewModel.CameraType;
                        }

                        this.UpdateApplicationBarState();
                        break;
                    case MainPageViewModel.FlashModePropertyName:
                        if (this.viewfinder != null)
                        {
                            string message = string.Empty;
                            switch (this.viewModel.FlashMode)
                            {
                                case FlashMode.Off:
                                    message = AppResources.FlashOffTextLabel;
                                    break;
                                case FlashMode.On:
                                    message = AppResources.FlashOnTextLabel;
                                    break;
                                case FlashMode.Auto:
                                    message = AppResources.FlashAutoTextLabel;
                                    break;
                            }

                            this.viewfinder.ShowStatusMessage(message);
                        }

                        this.UpdateApplicationBarState();

                        break;
                    case MainPageViewModel.CaptureModePropertyName:
                        if (this.viewfinder != null)
                        {
                            string message = string.Empty;
                            switch (this.viewModel.CaptureMode)
                            {
                                case CaptureMode.LowLag:
                                    message = AppResources.LowLagTextLabel;
                                    break;
                                case CaptureMode.Variable:
                                    message = AppResources.VariableTextLabel;
                                    break;
                            }

                            this.viewfinder.ShowStatusMessage(message);
                        }

                        this.UpdateApplicationBarState();

                        break;
                    case MainPageViewModel.PreviewBrushPropertyName:
                        if (this.viewfinder != null)
                        {
                            this.viewfinder.PreviewBrush = this.viewModel.PreviewBrush;
                        }

                        break;
                    case MainPageViewModel.PreviewSizePropertyName:
                        if (this.viewfinder != null)
                        {
                            double max = Math.Max(this.viewModel.PreviewSize.Width, this.viewModel.PreviewSize.Height);
                            double min = Math.Min(this.viewModel.PreviewSize.Width, this.viewModel.PreviewSize.Height);

                            if (OrientationHelper.IsPortrait(this.Orientation))
                            {
                                this.viewfinder.PreviewHeight = max;
                                this.viewfinder.PreviewWidth  = min;
                            }
                            else
                            {
                                this.viewfinder.PreviewHeight = min;
                                this.viewfinder.PreviewWidth  = max;
                            }
                        }

                        break;
                    case MainPageViewModel.ReviewImageBrushPropertyName:
                        if (this.viewfinder != null)
                        {
                            // If your application captures video and needs to slide the last
                            // video frame, please, refer to the Viewfinder.GetPreviewSnapshot method.
                            this.viewfinder.ReviewImageBrush = this.viewModel.ReviewImageBrush;
                            this.viewfinder.SlideReviewImage();
                        }

                        break;
                }
            });
        }

        /// <summary>
        /// Handles the <see cref="MainPageViewModel.Items"/> collection changes.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void ViewModelOnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SetToCameraRollArrowVisibility(this.viewModel.Items != null && this.viewModel.Items.Any());
        }

        /// <summary>
        /// The <see cref="MainPageViewModel.FocusStateChanged"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="FocusStateChangedEventArgs"/> instance containing the event data.</param>
        private void ViewModelOnFocusStateChanged(object sender, FocusStateChangedEventArgs e)
        {
            DispatcherHelper.BeginInvokeOnUIThread(() =>
            {
                switch (e.FocusState)
                {
                    case FocusState.Focusing:
                        if (this.viewfinder != null)
                        {
                            if (e.FocusPoint.HasValue)
                            {
                                this.viewfinder.FlashPointFocusBrackets(e.FocusPoint.Value);
                            }
                            else
                            {
                                this.viewfinder.FlashAutoFocusBrackets();
                            }
                        }

                        break;
                    case FocusState.Locked:
                        if (this.viewfinder != null)
                        {
                            if (e.FocusPoint.HasValue)
                            {
                                this.viewfinder.LockTouchFocusBrackets();
                            }
                            else
                            {
                                this.viewfinder.LockAutoFocusBrackets();
                            }
                        }

                        break;
                    case FocusState.Unlocked:
                        if (this.viewfinder != null)
                        {
                            this.viewfinder.DismissFocusBrackets();
                        }

                        break;
                }
            });
        }

        /// <summary>
        /// The <see cref="MainPageViewModel.CameraLoaded"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ViewModelOnCameraLoaded(object sender, EventArgs eventArgs)
        {
            this.UpdateApplicationBarState();
        }

        /// <summary>
        /// The <see cref="MainPageViewModel.CameraUnloaded"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ViewModelOnCameraUnloaded(object sender, EventArgs eventArgs)
        {
            this.UpdateApplicationBarState();
        }

        #endregion // LensViewModel

        #region Helpers

        /// <summary>
        /// Updates visibility of the control components.
        /// </summary>
        /// <param name="value">Whether the chrome components should be visible.</param>
        private void SetChromeVisibility(bool value)
        {
            this.SetToCameraRollArrowVisibility(value);

            this.ApplicationBar.IsVisible = value;
            this.MediaViewer.DragEnabled  = value;
        }

        #endregion // Helpers

        #endregion // Private methods

        #region Enums

        /// <summary>
        /// Possible application bar button modes.
        /// </summary>
        private enum AppBarMode
        {
            /// <summary>
            /// No items should be shown in the application bar.
            /// </summary>
            Empty = 0,

            /// <summary>
            /// Viewfinder is displayed.
            /// </summary>
            Viewfinder,

            /// <summary>
            /// Previously captured frame is visible.
            /// </summary>
            Review
        };

        #endregion // Enums
    }
}
