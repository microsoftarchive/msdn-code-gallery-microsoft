//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/oAuthFilters.html", {
        ready: function (element, options) {
            document.getElementById("oAuthFacebookLaunch").addEventListener("click", launchFacebookWebAuth, false);
            document.getElementById("clearButton").addEventListener("click", clearTokenInfo, false);
        }
    });

    var switchf = null;

    function clearTokenInfo() {
        if (switchf !== null) {
            switchf.clearAll();
        }
    }

    function launchFacebookWebAuth() {
        var clientID = document.getElementById("FacebookClientID").value;
        if (clientID === null || clientID === "") {
            WinJS.log("Enter a ClientID", "Web Authentication SDK Sample", "error");            
            return;
        }

        var bpf = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
        switchf = new AuthFilters.SwitchableAuthFilter(bpf);
        var f = makeFacebook(clientID, bpf);
        switchf.addOAuth2Filter(f);
        
        var httpClient = new Windows.Web.Http.HttpClient(switchf);
        var uri = new Windows.Foundation.Uri("https://graph.facebook.com/me");
        httpClient.getStringAsync(uri).then(
            function (str) {
                document.getElementById("FacebookDebugArea").value += str;
            },
            function (str) {
                document.getElementById("FacebookDebugArea").value += "ERROR: " + str;
            }
        );
        
    }

    function makeFacebook(clientId, innerFilter) {
        var f = new AuthFilters.OAuth2Filter(innerFilter);
        var config = {
            clientId: clientId,

            // Common to all developers for this site
            technicalName: "facebook.com",
            apiUriPrefix: "https://graph.facebook.com/",
            sampleUri: "https://graph.facebook.com/me",

            redirectUri: "https://www.facebook.com/connect/login_success.html",
            clientSecret: "",
            scope: "read_stream",
            display: "popup",
            state: "",
            additionalParameterName: "",
            additionalParameterValue: "",
            responseType: "", // blank==default "token:.  null doesn't marshall.
            accessTokenLocation: "", // blank=default "query",
            accessTokenQueryParameterName: "", // blank=default "access_token",
            authorizationUri: "https://www.facebook.com/dialog/oauth",
            authorizationCodeToTokenUri: ""
        };
        f.authConfiguration = config;

        return f;
    }


})();
