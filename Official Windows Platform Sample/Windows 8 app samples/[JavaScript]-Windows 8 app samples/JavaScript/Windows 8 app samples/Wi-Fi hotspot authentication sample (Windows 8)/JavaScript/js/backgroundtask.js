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
    var result = Windows.Networking.NetworkOperators.HotspotAuthenticationContext.tryGetAuthenticationContext(details.eventToken);
    if (!result.isValid) {
        // The event is not targetting this application. There is no further processing to do.
        console.log("Failed to get event context");

        // A JavaScript background task must call close when it is done.
        close();
        return;
    }
    var context = result.context;

    // If the event targets this application, the event handler must ensure that it always
    // handles the event even in case of an internal error.
    // A try-catch block can be used to handle unexpected errors.

    // Default value in case the configuration cannot be loaded.
    var markAsManualConnect = false;
    var handleUnexpectedError = false;
    try {
        var ssid = context.wirelessNetworkId;
        console.log("SSID: " + String.fromCharCode.apply(String, ssid));
        console.log("AuthenticationUrl: " + context.authenticationUrl.rawUri);
        console.log("RedirectMessageUrl: " + context.redirectMessageUrl.rawUri);
        console.log("RedirectMessageXml: " + context.redirectMessageXml.getXml());

        // Get configuration from application storage.
        importScripts("configstore.js");
        markAsManualConnect = configuration.markAsManualConnect;

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

        // Check if authentication is handled by foreground app.
        if (!configuration.isAuthenticateThroughBackgroundTask()) {
            console.log("Triggering foreground application");
            // Pass event token to application
            configuration.setAuthenticationToken(details.eventToken);
            // Trigger application
            context.triggerAttentionRequired(foregroundAppId, "");

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
        }
        else {
            // The most common way of handling an authentication attempts is by providing WISPr credentials
            // through the IssueCredentials API.
            // Alternatively, an application could run its own business logic to authentication with the
            // hotspot. In this case it should call the SkipAuthentication API. Note that it should call
            // SkipAuthentication after it has authenticated to allow Windows to refresh the network connectivity
            // state instantly.
            console.log("Issuing credentials");
            context.issueCredentials(
                configuration.userName,
                configuration.password,
                configuration.extraParameters,
                markAsManualConnect);
        }
    }
    catch (ex) {
        console.log("Unhandled expection: " + ex.description);
        handleUnexpectedError = true;
    }

    // The background task handler should always handle the authentication context.
    if (handleUnexpectedError) {
        try {
            context.abortAuthentication(markAsManualConnect);
        }
        catch (ex) {
            console.log("Unhandled expection: " + ex.description);
        }
    }

    close();

    console.log("Background " + backgroundTaskInstance.task.name + " completed");
})();

