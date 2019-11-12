//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using MultipleViews;
using SecondaryViewsHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static string DISABLE_MAIN_VIEW_KEY = "DisableShowingMainViewOnActivation";
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Check if a secondary view is supposed to be shown
            ViewLifetimeControl ViewLifetimeControl;
            if (TryFindViewLifetimeControlForViewId(args.CurrentlyShownApplicationViewId, out ViewLifetimeControl))
            {
                await ViewLifetimeControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Window.Current.Activate();
                });
            }
            else
            {
                // We don't have the specified view in the collection, likely because it's the main view
                // that got shown. Set up the main view to display.
                InitializeMainPage(args.PreviousExecutionState, "");

                // This is the usual path at application startup
                Window.Current.Activate();
            }
        }

        private async void InitializeMainPage(ApplicationExecutionState previousExecutionState, String arguments)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (rootFrame == null)
            {
                mainDispatcher = Window.Current.Dispatcher;
                mainViewId = ApplicationView.GetForCurrentView().Id;
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (previousExecutionState == ApplicationExecutionState.Terminated)
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
            if (rootFrame.Content == null || !String.IsNullOrEmpty(arguments))
            {
                // This is encountered on the first launch of the app. Make sure to call
                // DisableShowingMainViewOnActivation before the first call to Window::Activate

                var shouldDisable = Windows.Storage.ApplicationData.Current.LocalSettings.Values[App.DISABLE_MAIN_VIEW_KEY];
                if (shouldDisable != null && (bool)shouldDisable)
                {
                    ApplicationViewSwitcher.DisableShowingMainViewOnActivation();
                }

                // When the navigation stack isn't restored or there are launch arguments
                // indicating an alternate launch (e.g.: via toast or secondary tile), 
                // navigate to the appropriate page, configuring the new page by passing required 
                // information as a navigation parameter
                if (!rootFrame.Navigate(typeof(MainPage), arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = (ProtocolActivatedEventArgs) args;
                // Find out which window the system chose to display
                // Unless you've set DisableShowingMainViewOnActivation,
                // it will always be your main view. See Scenario 2 for details
                ViewLifetimeControl ViewLifetimeControl;
                if (TryFindViewLifetimeControlForViewId(protocolArgs.CurrentlyShownApplicationViewId, out ViewLifetimeControl))
                {
                    await ViewLifetimeControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        var currentPage = (SecondaryViewPage) ((Frame)Window.Current.Content).Content;
                        currentPage.HandleProtocolLaunch(protocolArgs.Uri);
                        Window.Current.Activate();
                    });
                }
                else
                {
                    // We don't have the specified view in the collection, likely because it's the main view
                    // that got shown. Set up the main view to display.
                    InitializeMainPage(args.PreviousExecutionState, "");
                    var rootPage = (MainPage)((Frame)Window.Current.Content).Content;
                    ((ListBox)rootPage.FindName("Scenarios")).SelectedIndex = 1;
                    rootPage.NotifyUser("Main window was launched with protocol: " + protocolArgs.Uri.AbsoluteUri,
                                        NotifyType.StatusMessage);
                    Window.Current.Activate();
                }
            }
        }

        public ObservableCollection<ViewLifetimeControl> SecondaryViews = new ObservableCollection<ViewLifetimeControl>();

        // This method is designed to always be run on the thread that's
        // binding to the list above
        public void UpdateTitle(String newTitle, int viewId)
        {
            ViewLifetimeControl foundData;
            if (TryFindViewLifetimeControlForViewId(viewId, out foundData))
            {
                foundData.Title = newTitle;
            }
            else
            {
                throw new KeyNotFoundException("Couldn't find the view ID in the collection");
            }
        }
        
        // Loop through the collection to find the view ID
        // This should only be run on the main thread.
        bool TryFindViewLifetimeControlForViewId(int viewId, out ViewLifetimeControl foundData)
        {
            foreach (var ViewLifetimeControl in SecondaryViews)
            {
                if (ViewLifetimeControl.Id == viewId)
                {
                    foundData = ViewLifetimeControl;
                    return true;
                }
            }
            foundData = null;
            return false;
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

        private CoreDispatcher mainDispatcher;
        public CoreDispatcher MainDispatcher
        {
            get
            {
                return mainDispatcher;
            }
        }

        private int mainViewId;
        public int MainViewId
        {
            get
            {
                return mainViewId;
            }
        }
    }
}
