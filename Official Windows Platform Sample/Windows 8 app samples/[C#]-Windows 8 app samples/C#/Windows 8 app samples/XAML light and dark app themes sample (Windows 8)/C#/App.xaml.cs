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
using System;
using Windows.Storage;

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
            this.DetermineAppTheme();

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
        /// Custom code to show how to read "setting" for theme and set it. This function is called in the app constructor.
        /// </summary>
        private void DetermineAppTheme()
        {
            object oUseLightTheme = true;

            // Read the value of theme preference, if set. This value gets set when user changes theme in Scenario 2.
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("IsLightTheme", out oUseLightTheme))
            {
                if ((bool)oUseLightTheme == true)
                    this.RequestedTheme = ApplicationTheme.Light;
                else
                    this.RequestedTheme = ApplicationTheme.Dark;
            } else
                this.RequestedTheme = ApplicationTheme.Light;

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

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Do an asynchronous restore
                await SuspensionManager.RestoreAsync();

            }
            if (Window.Current.Content == null)
            {
                var rootFrame = new Frame();
                rootFrame.Navigate(typeof(MainPage));
                Window.Current.Content = rootFrame;
            }
            Window.Current.Activate();
        }
    }
}
