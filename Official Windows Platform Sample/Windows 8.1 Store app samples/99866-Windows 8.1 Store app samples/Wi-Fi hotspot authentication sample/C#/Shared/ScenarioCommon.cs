//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using HotspotAuthenticationTask;
using System;
using SDKTemplate;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;

namespace HotspotAuthenticationApp
{
    // A delegate type for hooking up foreground authentication notifications.
    public delegate void ForegroundAuthenticationDelegate(object sender, EventArgs e);

    // Shared code for all scenario pages
    class ScenarioCommon
    {
        // Singleton reference to share a single instance with all pages
        static ScenarioCommon scenarioCommonSingleton;

        public static ScenarioCommon Instance
        {
            get
            {
                if (scenarioCommonSingleton == null)
                {
                    scenarioCommonSingleton = new ScenarioCommon();
                }
                return scenarioCommonSingleton;
            }
        }

        // The entry point name of the background task handler:

        public const string BackgroundTaskEntryPoint = "HotspotAuthenticationTask.AuthenticationTask";

        // The (arbitrarily chosen) name assigned to the background task:
        public const string BackgroundTaskName = "AuthenticationBackgroundTask";

        // A delegate for subscribing for foreground authentication events
        public ForegroundAuthenticationDelegate ForegroundAuthenticationCallback;

        // A pointer back to the main page.  This is needed to call methods in MainPage such as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // A reference to the main window dispatcher object to the UI.
        CoreDispatcher coreDispatcher = Windows.UI.Xaml.Window.Current.CoreWindow.Dispatcher;

        // A flag to remember if a background task handler has been registered
        private bool HasRegisteredBackgroundTaskHandler = false;

        /// <summary>
        /// Register completion handler for registered background task on application startup.
        /// </summary>
        /// <returns>True if a registerd task was found</returns>
        public bool RegisteredCompletionHandlerForBackgroundTask()
        {
            if (!HasRegisteredBackgroundTaskHandler)
            {
                try
                {
                    // Associate background task completed event handler with background task.
                    foreach (var cur in BackgroundTaskRegistration.AllTasks)
                    {
                        if (cur.Value.Name == BackgroundTaskName)
                        {
                            cur.Value.Completed += new BackgroundTaskCompletedEventHandler(OnBackgroundTaskCompleted);
                            HasRegisteredBackgroundTaskHandler = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                }
            }
            return HasRegisteredBackgroundTaskHandler;
        }

        /// <summary>
        /// Background task completion handler. When authenticating through the foreground app, this triggers the authentication flow if the app is currently running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnBackgroundTaskCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            // Update the UI with progress reported by the background task.
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                      new DispatchedHandler(() =>
                                      {
                                          try
                                          {
                                              if ((sender != null) && (e != null))
                                              {
                                                  // If the background task threw an exception, display the exception in the error text box.
                                                  e.CheckResult();

                                                  // Update the UI with the completion status of the background task
                                                  // The Run method of the background task sets this status.
                                                  if (sender.Name == BackgroundTaskName)
                                                  {
                                                      rootPage.NotifyUser("Background task completed", NotifyType.StatusMessage);

                                                      // Signal callback for foreground authentication
                                                      if (!ConfigStore.AuthenticateThroughBackgroundTask && ForegroundAuthenticationCallback != null)
                                                      {
                                                          ForegroundAuthenticationCallback(this, null);
                                                      }
                                                  }
                                              }
                                          }
                                          catch (Exception ex)
                                          {
                                              rootPage.NotifyUser(ex.ToString(), NotifyType.ErrorMessage);
                                          }
                                      }));
        }
    }
}
