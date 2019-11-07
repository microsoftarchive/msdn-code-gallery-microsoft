//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SDKTemplate
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
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

        async private Task EnsureMainPageActivatedAsync(IActivatedEventArgs args)
        {
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

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        async protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            await EnsureMainPageActivatedAsync(args);
        }

        /// <summary>
        /// This is the handler for Search activation.
        /// </summary>
        /// <param name="args">This is the list of arguments for search activation, including QueryText and Language</param>
        async protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            await EnsureMainPageActivatedAsync(args);
            if (args.QueryText == "")
            {
                // navigate to landing page.
            }
            else
            {
                // display search results.
                MainPage.Current.ProcessQueryText(args.QueryText);
            }
        }

        private void OnQuerySubmitted(object sender, SearchPaneQuerySubmittedEventArgs args)
        {
            if (MainPage.Current != null)
            {
                MainPage.Current.ProcessQueryText(args.QueryText);
            }
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            // Register QuerySubmitted handler for the window at window creation time and only registered once
            // so that the app can receive user queries at any time.
            SearchPane.GetForCurrentView().QuerySubmitted += new TypedEventHandler<SearchPane, SearchPaneQuerySubmittedEventArgs>(OnQuerySubmitted);
        }
    }
}
