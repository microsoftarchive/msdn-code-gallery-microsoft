//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/oAuthFacebook.html", {
        ready: function (element, options) {
            document.getElementById("oAuthFacebookLaunch").addEventListener("click", launchFacebookWebAuth, false);            
        }
    });

    function isValidUriString(uriString) {
        var uri = null;
        try {
            uri = new Windows.Foundation.Uri(uriString);
        }
        catch (err) {
        }
        return uri !== null;
    }

    var authzInProgress = false;

    function launchFacebookWebAuth() {
        var facebookURL = "https://www.facebook.com/dialog/oauth?client_id=";

        var clientID = document.getElementById("FacebookClientID").value;
        if (clientID === null || clientID === "") {
            WinJS.log("Enter a ClientID", "Web Authentication SDK Sample", "error");            
            return;
        }

        var callbackURL = document.getElementById("FacebookCallbackURL").value;
        if (!isValidUriString(callbackURL)) {
            WinJS.log("Enter a valid Callback URL for Facebook", "Web Authentication SDK Sample", "error");
            return;
        }

        facebookURL += clientID + "&redirect_uri=" + encodeURIComponent(callbackURL) + "&scope=read_stream&display=popup&response_type=token";

        if (authzInProgress) {
            document.getElementById("FacebookDebugArea").value += "\r\nAuthorization already in Progress ...";
            return;
        }

        var startURI = new Windows.Foundation.Uri(facebookURL);
        var endURI = new Windows.Foundation.Uri(callbackURL);

        document.getElementById("FacebookDebugArea").value += "Navigating to: " + facebookURL + "\r\n";

        authzInProgress = true;
        Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
            Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startURI, endURI)
            .done(function (result) {
                document.getElementById("FacebookReturnedToken").value = result.responseData;
                document.getElementById("FacebookDebugArea").value += "Status returned by WebAuth broker: " + result.responseStatus + "\r\n";
                if (result.responseStatus === Windows.Security.Authentication.Web.WebAuthenticationStatus.errorHttp) {
                    document.getElementById("FacebookDebugArea").value += "Error returned: " + result.responseErrorDetail + "\r\n";
                }
                authzInProgress = false;
            }, function (err) {
                WinJS.log("Error returned by WebAuth broker: " + err, "Web Authentication SDK Sample", "error");
                document.getElementById("FacebookDebugArea").value += " Error Message: " + err.message + "\r\n";
                authzInProgress = false;
            });
    }
})();
