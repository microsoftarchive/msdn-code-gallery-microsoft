// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="App.xaml.cs">
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

namespace CameraSampleCS
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Navigation;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Resources;
    using CameraSampleCS.ViewModels;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;

    /// <summary>
    /// Main application class.
    /// </summary>
    public partial class App
    {
        #region Fields

        /// <summary>
        /// To avoid double-initialization.
        /// </summary>
        private bool phoneApplicationInitialized;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            this.UnhandledException += this.ApplicationUnhandledException;

            // Standard XAML initialization.
            this.InitializeComponent();

            // Phone-specific initialization.
            this.InitializePhoneApplication();

            // Language display initialization.
            this.InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                // Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                // Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution: Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the Root frame.<br/>
        /// Provides easy access to the root frame of the phone application.
        /// </summary>
        /// <returns>The root frame of the phone application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        #endregion // Properties

        #region Private methods

        /// <summary>
        /// Do not add any additional code to this method.
        /// </summary>
        private void InitializePhoneApplication()
        {
            if (this.phoneApplicationInitialized)
            {
                return;
            }

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            this.RootFrame = new PhoneApplicationFrame();
            this.RootFrame.Navigated += this.CompleteInitializePhoneApplication;

            // Handle navigation failures.
            this.RootFrame.NavigationFailed += this.RootFrameNavigationFailed;

            // Handle reset requests for clearing the backstack.
            this.RootFrame.Navigated += this.CheckForResetNavigation;

            // Ensure we don't initialize again.
            this.phoneApplicationInitialized = true;
        }

        /// <summary>
        /// Initialize the app's font and flow direction as defined in its localized resource strings.
        /// </summary>
        /// <remarks>
        /// To ensure that the font of your application is aligned with its supported languages and that the
        /// <see cref="FlowDirection"/> for each of those languages follows its traditional direction, <c>ResourceLanguage</c>
        /// and <c>ResourceFlowDirection</c> should be initialized in each <c>resx</c> file to match these values with that
        /// file's culture.<br/>
        /// For example:<br/>
        /// <br/>
        /// <c>AppResources.es-ES.resx</c><br/>
        ///    <c>ResourceLanguage</c>'s value should be "<c>es-ES</c>"<br/>
        ///    <c>ResourceFlowDirection</c>'s value should be "<c>LeftToRight</c>".
        /// <br/>
        /// <c>AppResources.ar-SA.resx</c><br/>
        ///     <c>ResourceLanguage</c>'s value should be "<c>ar-SA</c>"<br/>
        ///     <c>ResourceFlowDirection</c>'s value should be "<c>RightToLeft</c>".<br/>
        /// <br/>
        /// For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        /// </remarks>
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from the resource file.
                this.RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                this.RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either ResourceLangauge
                // not being correctly set to a supported language code or ResourceFlowDirection
                // is set to a value other than LeftToRight or RightToLeft.
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        /// <summary>
        /// Code to execute when the application is launching (eg, from <c>Start</c>).<br/>
        /// This code will not execute when the application is reactivated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="LaunchingEventArgs"/> instance containing the event data.</param>
        private void ApplicationLaunching(object sender, LaunchingEventArgs e)
        {
            // Do nothing.
        }

        /// <summary>
        /// Code to execute when the application is closing (eg, user hit <c>Back</c>).<br/>
        /// This code will not execute when the application is deactivated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="ClosingEventArgs"/> instance containing the event data.</param>
        private void ApplicationClosing(object sender, ClosingEventArgs e)
        {
            ViewModelLocator.Cleanup();
        }

        /// <summary>
        /// Code to execute when the application is activated (brought to foreground).<br/>
        /// This code will not execute when the application is first launched.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="ActivatedEventArgs"/> instance containing the event data.</param>
        private void ApplicationActivated(object sender, ActivatedEventArgs e)
        {
            // Do nothing.
        }

        /// <summary>
        /// Code to execute when the application is deactivated (sent to background).<br/>
        /// This code will not execute when the application is closing.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="DeactivatedEventArgs"/> instance containing the event data.</param>
        private void ApplicationDeactivated(object sender, DeactivatedEventArgs e)
        {
            // Do nothing.
        }

        /// <summary>
        /// Code to execute on navigation failures.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="NavigationFailedEventArgs"/> instance containing the event data.</param>
        private void RootFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Tracing.Trace(string.Format(CultureInfo.InvariantCulture, "Navigation failed:\r\n{0}", e.Exception));

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        /// <summary>
        /// Code to execute on unhandled exceptions.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="ApplicationUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Tracing.Trace(string.Format(CultureInfo.InvariantCulture, "Unhandled exception:\r\n{0}", e.ExceptionObject));

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        /// <summary>
        /// Do not add any additional code to this method.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render.
            if (this.RootVisual != this.RootFrame)
            {
                this.RootVisual = this.RootFrame;
            }

            // Remove this handler since it is no longer needed.
            this.RootFrame.Navigated -= this.CompleteInitializePhoneApplication;
        }

        /// <summary>
        /// Handles reset requests for clearing the backstack.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset.
            if (e.NavigationMode == NavigationMode.Reset)
            {
                this.RootFrame.Navigated += this.ClearBackStackAfterReset;
            }
        }

        /// <summary>
        /// Clears the navigation stack after the reset event is received.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again.
            this.RootFrame.Navigated -= this.ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations.
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
            {
                return;
            }

            // For UI consistency, clear the entire page stack.
            while (this.RootFrame.RemoveBackEntry() != null)
            {
                // Do nothing.
            }
        }

        #endregion // Private methods
    }
}
