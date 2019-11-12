//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var authenticationContext;
    var page = WinJS.UI.Pages.define("/html/auth-by-foregroundapp.html", {
        ready: function (element, options) {
            // Configure background task handler to lauch the foreground app
            configuration.setAuthenticateThroughBackgroundTask(false);

            // Register button handlers
            document.getElementById("authenticateButton").addEventListener("click", onButtonAuthenticate, false);
            document.getElementById("skipButton").addEventListener("click", onButtonSkip, false);
            document.getElementById("abortButton").addEventListener("click", onButtonAbort, false);

            // Register background task completion handler
            common.registerBackgroundTaskEventHandler();

            // Register callback for updating UI state
            common.completeHandlerCallback = initializeForegroundAppAuthentication;

            // Probe for pending authentication
            initializeForegroundAppAuthentication();
        }
    });

    // Query authentication token from application storage and upate the UI.
    // The token gets passed from the background task handler.
    function initializeForegroundAppAuthentication() {
        var token = configuration.getAuthenticationToken();
        if (token === "") {
            return; // No token found
        }
        var result = Windows.Networking.NetworkOperators.HotspotAuthenticationContext.tryGetAuthenticationContext(token);
        if (!result.isValid) {
            WinJS.log && WinJS.log("tryGetAuthenticationContext failed", "sample", "error");
            return;
        }
        authenticationContext = result.context;

        document.getElementById("authenticateButton").disabled = false;
        document.getElementById("skipButton").disabled = false;
        document.getElementById("abortButton").disabled = false;
    }

    // Clear the authentication token in the application storage and update the UI.
    function clearAuthenticationToken() {
        configuration.setAuthenticationToken("");
        document.getElementById("authenticateButton").disabled = true;
        document.getElementById("skipButton").disabled = true;
        document.getElementById("abortButton").disabled = true;
    }

    // Handle a click on the Authenticate button
    function onButtonAuthenticate() {
        try {
            authenticationContext.issueCredentials(
                configuration.userName,
                configuration.password,
                configuration.extraParameters,
                configuration.markAsManualConnect);
            WinJS.log && WinJS.log("Issuing credentials succeeded", "sample", "status");
        }
        catch (ex) {
            WinJS.log && WinJS.log(ex.description, "sample", "error");
        }
        clearAuthenticationToken();
    }

    // Handle a click on the Skip button
    function onButtonSkip() {
        try {
            authenticationContext.skipAuthentication();
            WinJS.log && WinJS.log("Authentication skipped", "sample", "status");
        }
        catch (ex) {
            WinJS.log && WinJS.log(ex.description, "sample", "error");
        }
        clearAuthenticationToken();
    }

    // Handle a click on the Abort button
    function onButtonAbort() {
        try {
            authenticationContext.abortAuthentication(
                configuration.markAsManualConnect);
            WinJS.log && WinJS.log("Authentication aborted", "sample", "status");
        }
        catch (ex) {
            WinJS.log && WinJS.log(ex.description, "sample", "error");
        }
        clearAuthenticationToken();
    }

})();
