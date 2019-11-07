//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Microsoft.WindowsAzure.MobileServices;
using StoreRoom.Entities;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StoreRoom
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://piyushjostore.azure-mobile.net/",
            "HiTFsQyPgcdFpbvGayEXqrElWelnXh37");

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        public static PushNotificationChannel CurrentChannel { get; set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await this.AcquirePushChannel();
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
            if (rootFrame.Content == null || !String.IsNullOrEmpty(args.Arguments))
            {
                // When the navigation stack isn't restored or there are launch arguments
                // indicating an alternate launch (e.g.: via toast or secondary tile), 
                // navigate to the appropriate page, configuring the new page by passing required 
                // information as a navigation parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
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

        private async Task AcquirePushChannel()
        {
            CurrentChannel
                = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            var channel = new Channel()
            {
                Uri = CurrentChannel.Uri,
                ExpirationTime = CurrentChannel.ExpirationTime.ToUniversalTime()
            };

            try
            {
                var channelTable = App.MobileService.GetTable<Channel>();
                if (ApplicationData.Current.LocalSettings.Values["ChannelId"] == null)
                {
                    await channelTable.InsertAsync(channel);
                    ApplicationData.Current.LocalSettings.Values["ChannelId"] = channel.Id;
                }
                else
                {
                    channel.Id = (string)ApplicationData.Current.LocalSettings.Values["ChannelId"];
                    await channelTable.UpdateAsync(channel);
                }
            }
            catch (HttpRequestException)
            {
                ShowError();
            }
        }

        private async void ShowError()
        {
            MessageDialog dialog = new MessageDialog("Correctly configure the Mobile Service url and key in the sample.", "Configuration error");
            await dialog.ShowAsync();
        }
    }
}
