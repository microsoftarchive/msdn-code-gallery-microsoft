//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// A JavaScript background task runs a specified JavaScript file.
(function () {
    "use strict";

    // Application Id of the foreground app
    var foregroundAppId = "App";

    // The background task instance's activation parameters are available via Windows.UI.WebUI.WebUIBackgroundTaskInstance.current
    var backgroundTaskInstance = Windows.UI.WebUI.WebUIBackgroundTaskInstance.current;

    // Initial state for cancelation
    var cancel = false;

    console.log("Background " + backgroundTaskInstance.task.name + " Starting...");

    // Associate a cancellation handler with the background task.
    function onCanceled(cancelSender, cancelReason) {
        cancel = true;
    }
    backgroundTaskInstance.addEventListener("canceled", onCanceled);

    // Do the background task activity. First, get the authentication context.
    console.log("Getting event details");

    var details = backgroundTaskInstance.triggerDetails;
    var authenticationContext = Windows.Networking.NetworkOperators.HotspotAuthenticationContext.tryGetAuthenticationContext(details.eventToken);
    if (!authenticationContext.isValid) {
        // The event is not targetting this application. There is no further processing to do.
        console.log("Failed to get event context");

        // A JavaScript background task must call close when it is done.
        close();
        return;
    }
    var context = authenticationContext.context;

    var ssid = context.wirelessNetworkId;
    console.log("SSID: " + String.fromCharCode.apply(String, ssid));


    // Get configuration from application storage.   
    importScripts("common.js");
    importScripts("configstore.js");
    importScripts("platform.js");

    var markAsManualConnect = configuration.markAsManualConnect;

    if (platform.HotspotAuthenticationSamplePlatformSpecific.isNativeWISPrSample()) {
        //This is only applicable in Windows for Native WISPr scenarios
        // following HotspotAuthenticationContext properties only work on windows and not on windows phone. 
        // On Windows Phone they return un-useful strings
        // Developers are expected to implement their own WISPr implementation on Phone
        console.log("AuthenticationUrl: " + context.authenticationUrl.rawUri);
        console.log("RedirectMessageUrl: " + context.redirectMessageUrl.rawUri);
        console.log("RedirectMessageXml: " + context.redirectMessageXml.getXml());


        // In this sample, the AuthenticationUrl is always checked in the background task handler
        // to avoid launching the foreground app in case the authentication host is not trusted.
        if (configuration.authenticationHost !== context.authenticationUrl.host) {
            // Hotspot is not using the trusted authentication server.
            // Abort authentication and disconnect.
            console.log("Authentication server is untrusted");
            context.abortAuthentication(markAsManualConnect);

            close();
            return;
        }
    }

    // Check if authentication is handled by foreground app.
    if (!configuration.isAuthenticateThroughBackgroundTask()) {
        console.log("Triggering foreground application");
        // Pass event token to application
        configuration.setAuthenticationToken(details.eventToken);
        // Trigger application


        //
        // Following block of code generates a toast notification. Tapping on the notification will 
        // launch the application. If utilizing native WISPr functionality (on Windows only), you can just call 
        // context.triggerAttentionRequired(foregroundAppId, ""); 
        //
        if (platform.HotspotAuthenticationSamplePlatformSpecific.isNativeWISPrSample()) {
            context.triggerAttentionRequired(foregroundAppId, "");
        } else {

            var notifications = Windows.UI.Notifications;
            var toastTemplateName = "toastText01";

            var notificationManager = notifications.ToastNotificationManager;


            var toastXml = notificationManager.getTemplateContent(notifications.ToastTemplateType[toastTemplateName]);

            toastXml.getElementsByTagName("text").item(0).innerText = "Auth by foreground";

            var toastNode = toastXml.selectSingleNode("/toast");

            toastNode.setAttribute("launch", "AuthByForeground");

            var toast = new Windows.UI.Notifications.ToastNotification(toastXml);

            if (toast.hasOwnProperty("Tag")) {
                toast.Tag = "AuthByForeground";
            }

            if (toast.hasOwnProperty("Group")) {
                toast.Group = "HotspotAuthAPI";
            }

            var notification = Windows.UI.Notifications.ToastNotificationManager.createToastNotifier();
            notification.show(toast);

        }

        close();
        return;
    }

    // Handle authentication in background task.

    // In case this handler performs more complex tasks, it may get canceled at runtime.
    // Check if task was canceled by now.
    if (cancel) {
        // In case the task handler takes too long to generate credentials and gets canceled,
        // the handler should terminate the authentication by aborting it
        console.log("Aborting authentication");
        context.abortAuthentication(markAsManualConnect);

        close();
        return;
    }

    // Before calling an asynchronous API from the background task,
    // get the deferral object from the task instance.
    var deferral = backgroundTaskInstance.getDeferral();


    if (platform.HotspotAuthenticationSamplePlatformSpecific.isNativeWISPrSample()) {
        // The most common way of handling an authentication attempt is by providing WISPr credentials
        // through the IssueCredentialsAsync API.
        // If the task doesn't take any actions for authentication failures, it can use the
        // IssueCredentials API to just provide credenstials.
        // Alternatively, an application could run its own business logic to authentication with the
        // hotspot. In this case it should call the SkipAuthentication API. Note that it should call
        // SkipAuthentication after it has authenticated to allow Windows to refresh the network connectivity
        // state instantly.
        console.log("Issuing credentials");
        context.issueCredentialsAsync(
            configuration.userName,
            configuration.password,
            configuration.extraParameters,
            configuration.markAsManualConnect
            ).done(function (result) {
                var responseCode = result.responseCode;
                if (responseCode === Windows.Networking.NetworkOperators.HotspotAuthenticationResponseCode.loginSucceeded) {
                    console.log("Issuing credentials succeeded", "sample", "status");
                    var logoffUrl = result.logoffUrl;
                    if (logoffUrl !== null) {
                        console.log("The logoff URL is: " + logoffUrl.rawUri, "sample", "status");
                    }
                } else {
                    console.log("Issuing credentials failed", "sample", "error");
                }
                console.log("Background " + backgroundTaskInstance.task.name + " completed");
                deferral.complete();
                close();
            });
    } else {

        //TODO: Please perform any authentication that is required by your particular scenario

        // Finally call SkipAuthentication to indicate that we are not doing native WISPr authentication
        // This call also serves the purpose of indicating a successful authentication.
        context.skipAuthentication();
    }
})();

