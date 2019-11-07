//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Networking.NetworkOperators;

// The namespace for the background tasks.
namespace HotspotAuthenticationTask
{
    // A background task always implements the IBackgroundTask interface.
    public sealed class AuthenticationTask : IBackgroundTask
    {
        private const string _foregroundAppId = "HotspotAuthenticationApp.App";

        volatile bool _cancelRequested = false;

        // The Run method is the entry point of a background task.
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Background " + taskInstance.Task.Name + " starting...");

            // Associate a cancelation handler with the background task for handlers
            // that may take a considerable time to complete.
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            // Do the background task activity. First, get the authentication context.
            Debug.WriteLine("Getting event details");
            var details = taskInstance.TriggerDetails as HotspotAuthenticationEventDetails;

            HotspotAuthenticationContext context;
            if (!HotspotAuthenticationContext.TryGetAuthenticationContext(details.EventToken, out context))
            {
                // The event is not targetting this application. There is no further processing to do.
                Debug.WriteLine("Failed to get event context");
                return;
            }

            // If the event targets this application, the event handler must ensure that it always
            // handles the event even in case of an internal error.
            // A try-catch block can be used to handle unexpected errors.

            // Default value in case the configuration cannot be loaded.
            bool markAsManualConnect = false;
            bool handleUnexpectedError = false;
            try
            {
                byte[] ssid = context.WirelessNetworkId;
                Debug.WriteLine("SSID: " + System.Text.UTF8Encoding.UTF8.GetString(ssid, 0, ssid.Length));
                Debug.WriteLine("AuthenticationUrl: " + context.AuthenticationUrl.OriginalString);
                Debug.WriteLine("RedirectMessageUrl: " + context.RedirectMessageUrl.OriginalString);
                Debug.WriteLine("RedirectMessageXml: " + context.RedirectMessageXml.GetXml());

                // Get configuration from application storage.
                markAsManualConnect = ConfigStore.MarkAsManualConnect;

                // In this sample, the AuthenticationUrl is always checked in the background task handler
                // to avoid launching the foreground app in case the authentication host is not trusted.
                if (ConfigStore.AuthenticationHost != context.AuthenticationUrl.Host)
                {
                    // Hotspot is not using the trusted authentication server.
                    // Abort authentication and disconnect.
                    Debug.WriteLine("Authentication server is untrusted");
                    context.AbortAuthentication(markAsManualConnect);
                    return;
                }

                // Check if authentication is handled by foreground app.
                if (!ConfigStore.AuthenticateThroughBackgroundTask)
                {
                    Debug.WriteLine("Triggering foreground application");
                    // Pass event token to application
                    ConfigStore.AuthenticationToken = details.EventToken;
                    // Trigger application
                    context.TriggerAttentionRequired(_foregroundAppId, "");
                    return;
                }

                // Handle authentication in background task.

                // In case this handler performs more complex tasks, it may get canceled at runtime.
                // Check if task was canceled by now.
                if (_cancelRequested)
                {
                    // In case the task handler takes too long to generate credentials and gets canceled,
                    // the handler should terminate the authentication by aborting it
                    Debug.WriteLine("Aborting authentication");
                    context.AbortAuthentication(markAsManualConnect);
                }
                else
                {
                    // The most common way of handling an authentication attempts is by providing WISPr credentials
                    // through the IssueCredentials API.
                    // Alternatively, an application could run its own business logic to authentication with the
                    // hotspot. In this case it should call the SkipAuthentication API. Note that it should call
                    // SkipAuthentication after it has authenticated to allow Windows to refresh the network connectivity
                    // state instantly.
                    Debug.WriteLine("Issuing credentials");
                    context.IssueCredentials(ConfigStore.UserName, ConfigStore.Password, ConfigStore.ExtraParameters, markAsManualConnect);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unhandled expection: " + e.ToString());
                handleUnexpectedError = true;
            }

            // The background task handler should always handle the authentication context.
            if (handleUnexpectedError)
            {
                try
                {
                    context.AbortAuthentication(markAsManualConnect);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Unhandled expection: " + e.ToString());
                }
            }

            Debug.WriteLine("Background " + taskInstance.Task.Name + " completed");
        }

        // Handles background task cancellation.
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Indicate that the background task is canceled.
            _cancelRequested = true;

            Debug.WriteLine("Background " + sender.Task.Name + " cancel requested...");
        }
    }
}
