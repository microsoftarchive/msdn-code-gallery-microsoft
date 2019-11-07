//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;

namespace SDKTemplate
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// Initializes the window content of the app and navigates to the appropriate page. 
        /// This is called regardless of how the app is being activated. 
        /// </summary>
        /// <param name="args">Details about the activation request and process.</param>
        private async Task InitializeApp(IActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            LaunchActivatedEventArgs launchArgs = args as LaunchActivatedEventArgs;
            String launchArguments = (launchArgs != null) ? launchArgs.Arguments : null;
            if (rootFrame.Content == null || !String.IsNullOrEmpty(launchArguments))
            {
                // When the navigation stack isn't restored or there are launch arguments
                // indicating an alternate launch (e.g.: via toast or secondary tile), 
                // navigate to the appropriate page, configuring the new page by passing required 
                // information as a navigation parameter
                if (!rootFrame.Navigate(typeof(MainPage), launchArguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Navigate to appropriate page based on activation arguments if present.
            var mainPage = rootFrame.Content as MainPage;
            mainPage.ContactEvent = null;
            mainPage.ProtocolEvent = null;

            var contactArgs = args as IContactActivatedEventArgs;
            if (contactArgs != null)
            {
                mainPage.ContactEvent = contactArgs;
                mainPage.NavigateToContactEventPage();
            }
            else
            {
                var protocolArgs = args as ProtocolActivatedEventArgs;
                if (protocolArgs != null)
                {
                    mainPage.ProtocolEvent = protocolArgs;
                    mainPage.NavigateToProtocolEventPage();
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await InitializeApp(args);
        }

        /// <summary>
        /// Invoked when the application is being activated to handle a contract such as the tel:
        /// protocol, or actions on contact data, such as calling a phone number, and so forth.
        /// </summary>
        /// <param name="args">Details about the activation request and process.</param>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if ((args.Kind == ActivationKind.Contact) || (args.Kind == ActivationKind.Protocol))
            {
                await InitializeApp(args);
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
