//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/oAuthAnyService.html", {
        ready: function (element, options) {
            document.getElementById("oAuthAnyServiceLaunch").addEventListener("click", launchAnyServiceWebAuth, false);            
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

    function launchAnyServiceWebAuth() {

        var serviceRequestURI = document.getElementById("ServiceRequestURI").value;
        if (!isValidUriString(serviceRequestURI)) {
            WinJS.log("Enter a Start URI", "Web Authentication SDK Sample", "error");            
            return;
        }

        var callbackURL = document.getElementById("ServiceCallbackURI").value;
        if (!isValidUriString(callbackURL)) {
            WinJS.log("Enter an End URI", "Web Authentication SDK Sample", "error");
            return;
        }

        if (authzInProgress) {
            document.getElementById("AnyServiceDebugArea").value += "\r\nAuthorization already in Progress ...";
            return;
        }

        var startURI = new Windows.Foundation.Uri(serviceRequestURI);
        var endURI = new Windows.Foundation.Uri(callbackURL);

        document.getElementById("AnyServiceDebugArea").value += "Navigating to: " + serviceRequestURI + "\r\n";

        authzInProgress = true;
        Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
            Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startURI,endURI)
            .done(function (result) {
                document.getElementById("AnyServiceReturnedToken").value = result.responseData;
                document.getElementById("AnyServiceDebugArea").value += "Status returned by WebAuth broker: " + result.responseStatus + "\r\n";
                if (result.responseStatus === Windows.Security.Authentication.Web.WebAuthenticationStatus.errorHttp) {
                    document.getElementById("AnyServiceDebugArea").value += "Error returned: " + result.responseErrorDetail + "\r\n";
                }
                authzInProgress = false;
            }, function (err) {
                WinJS.log("Error returned by WebAuth broker: " + err, "Web Authentication SDK Sample", "error");        
                document.getElementById("AnyServiceDebugArea").value += " Error Message: " + err.message + "\r\n";
                authzInProgress = false;
            });
    }
})();
