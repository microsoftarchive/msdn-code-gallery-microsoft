//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

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
        public LaunchActivatedEventArgs LaunchArgs;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += new SuspendingEventHandler(OnSuspending);
        }

        async void OnSuspending(object sender, SuspendingEventArgs args)
        {
            SuspendingDeferral deferral = args.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        async protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            this.LaunchArgs = args;

            Frame rootFrame = new Frame();
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Do an asynchronous restore
                await SuspensionManager.RestoreAsync();
            }
            if (Window.Current.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage));
                Window.Current.Content = rootFrame;
            }
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            // Check to see if the app was activated via a protocol
            if (args.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = (ProtocolActivatedEventArgs)args;

                // This app was activated via the Account picture apps section in PC Settings / Personalize / Account picture.
                // Here you would do app-specific logic so that the user receives account picture selection UX.
                if (protocolArgs.Uri.Scheme == "ms-accountpictureprovider")
                {
                    // The Content might be null if App has not yet been activated, if so first activate the main page.
                    if (Window.Current.Content == null)
                    {
                        ConstructMainPage();
                    }
                    // The scenario is set to 4 (Set Account Picture) explicitly if Content has already been loaded
                    MainPage.Current.NavigateToSetAccountPictureAndListen();
                }
            }
        }

        private void ConstructMainPage()
        {
            Frame rootFrame = new Frame();

            if (Window.Current.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage));
                Window.Current.Content = rootFrame;
            }
            Window.Current.Activate();
        }
    }
}
