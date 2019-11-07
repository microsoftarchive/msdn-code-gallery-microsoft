// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Diagnostics;
using WalletQuickstart.Common;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WalletQuickstart
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        Frame RootFrame; 

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            CreateRootFrame();

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
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

            if (RootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                RootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        } 


        /// <summary>
        /// Both the OnLaunched and OnActivated event handlers need to make sure the root frame has been created, so the common 
        /// code to do that is factored into this method and called from both.
        /// </summary>
        private void CreateRootFrame()
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame != null)
                return;

            // Create a Frame to act as the navigation context and navigate to the first page
            RootFrame = new Frame();

            //Associate the frame with a SuspensionManager key                                
            SuspensionManager.RegisterFrame(RootFrame, "AppFrame");

            // Set the default language
            RootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

            RootFrame.NavigationFailed += OnNavigationFailed;

            // Place the frame in the current Window
            Window.Current.Content = RootFrame;
        } 

        /// <summary>
        /// The OnActivated event occurs when the app has been activated through an action other than the normal opening of the app
        /// by the user from the start page. In this Wallet sample, activation occurs when the user taps on 
        /// an action on a card associated with this app in Wallet. 
        /// In this event handler, the activation arguments are then passed to the MainPage.
        /// To see how this activation is handled in this sample, see DetectActivationKind in MainPage.xaml.cs
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnActivated(IActivatedEventArgs e)
        {
            // Check whether app was activated due to a Wallet action
            if (e.Kind == ActivationKind.WalletAction)
            {
                Debug.WriteLine("Activated by a Wallet action");

                // Cast the incoming arguments to a WalletActionActivatedEventArgs object
                WalletActionActivatedEventArgs walletActivationArgs = e as WalletActionActivatedEventArgs;

                // Check the properties of the WalletActionActivatedEventArgs to determine what
                // action and item caused the app to be activated.
                Debug.WriteLine("ActionId = {0}", walletActivationArgs.ActionId);
                Debug.WriteLine("ActionKind = {0}", walletActivationArgs.ActionKind);
                Debug.WriteLine("ItemId = {0}", walletActivationArgs.ItemId);
            }

            CreateRootFrame();

            // Restore the saved session state only when appropriate
            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
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

            if (RootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                RootFrame.Navigate(typeof(MainPage), e);
            }

            Window.Current.Activate();
        } 

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
