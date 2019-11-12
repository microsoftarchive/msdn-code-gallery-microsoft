//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/oAuthTwitter.html", {
        ready: function (element, options) {
            document.getElementById("oAuthTwitterLaunch").addEventListener("click", launchTwitterWebAuth, false);
        }
    });

    function sendRequest(url) {
        try {
            var request = new XMLHttpRequest();
            request.open("GET", url, false);
            request.send(null);
            return request.responseText;
        } catch (err) {
            WinJS.log("Error sending request: " + err, "Web Authentication SDK Sample", "error");
        }
    }

    function sendPostRequest(url, authzheader) {
        try {
            var request = new XMLHttpRequest();
            request.open("POST", url, false);
            request.setRequestHeader("Authorization", authzheader);
            request.send(null);
            return request.responseText;
        } catch (err) {
            WinJS.log("Error sending request: " + err, "Web Authentication SDK Sample", "error");            
        }
    }

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

    function launchTwitterWebAuth() {
        var twitterURL = "https://api.twitter.com/oauth/request_token";

        // Get all the parameters from the user
        var clientID = document.getElementById("TwitterClientID").value;
        if (clientID === null || clientID === "") {
            WinJS.log("Please enter a ClientID for Twitter App", "Web Authentication SDK Sample", "error");            
            return;
        }

        var clientSecret = document.getElementById("TwitterSecret").value;
        if (clientSecret === null || clientSecret === "") {
            WinJS.log("Please enter a Secret for Twitter App", "Web Authentication SDK Sample", "error");            
            return;
        }

        var callbackURL = document.getElementById("TwitterCallbackURL").value;
        if (!isValidUriString(callbackURL)) {
            WinJS.log("Please enter a Callback URL for Twitter", "Web Authentication SDK Sample", "error");            
            return;
        }

        if (authzInProgress) {
            document.getElementById("TwitterDebugArea").value += "\r\nAuthorization already in Progress ...";
            return;
        }

        // Acquiring a request token
        var timestamp = Math.round(new Date().getTime() / 1000.0);
        var nonce = Math.random();
        nonce = Math.floor(nonce * 1000000000);

        // Compute base signature string and sign it.
        //    This is a common operation that is required for all requests even after the token is obtained.
        //    Parameters need to be sorted in alphabetical order
        //    Keys and values should be URL Encoded.
        var sigBaseStringParams = "oauth_callback=" + encodeURIComponent(callbackURL);
        sigBaseStringParams += "&" + "oauth_consumer_key=" + clientID;
        sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
        sigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
        sigBaseStringParams += "&" + "oauth_timestamp=" + timestamp;
        sigBaseStringParams += "&" + "oauth_version=1.0";
        var sigBaseString = "POST&";
        sigBaseString += encodeURIComponent(twitterURL) + "&" + encodeURIComponent(sigBaseStringParams);

        var keyText = clientSecret + "&";
        var keyMaterial = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(keyText, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
        var macAlgorithmProvider = Windows.Security.Cryptography.Core.MacAlgorithmProvider.openAlgorithm("HMAC_SHA1");
        var key = macAlgorithmProvider.createKey(keyMaterial);
        var tbs = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(sigBaseString, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
        var signatureBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.sign(key, tbs);
        var signature = Windows.Security.Cryptography.CryptographicBuffer.encodeToBase64String(signatureBuffer);
        var dataToPost = "OAuth oauth_callback=\"" + encodeURIComponent(callbackURL) + "\", oauth_consumer_key=\"" + clientID + "\", oauth_nonce=\"" + nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + timestamp + "\", oauth_version=\"1.0\", oauth_signature=\"" + encodeURIComponent(signature) + "\"";
        var response = sendPostRequest(twitterURL, dataToPost);
        var oauth_token;
        var oauth_token_secret;
        var keyValPairs = response.split("&");

        for (var i = 0; i < keyValPairs.length; i++) {
            var splits = keyValPairs[i].split("=");
            switch (splits[0]) {
                case "oauth_token":
                    oauth_token = splits[1];
                    break;
                case "oauth_token_secret":
                    oauth_token_secret = splits[1];
                    break;
            }
        }

        document.getElementById("TwitterDebugArea").value += "\r\nOAuth Token = " + oauth_token;
        document.getElementById("TwitterDebugArea").value += "\r\nOAuth Token Secret = " + oauth_token_secret;

        // Send the user to authorization
        twitterURL = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;

        document.getElementById("TwitterDebugArea").value += "\r\nNavigating to: " + twitterURL + "\r\n";
        var startURI = new Windows.Foundation.Uri(twitterURL);
        var endURI = new Windows.Foundation.Uri(callbackURL);

        authzInProgress = true;
        Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
            Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startURI, endURI)
            .done(function (result) {
                document.getElementById("TwitterReturnedToken").value = result.responseData;
                document.getElementById("TwitterDebugArea").value += "Status returned by WebAuth broker: " + result.responseStatus + "\r\n";
                if (result.responseStatus === Windows.Security.Authentication.Web.WebAuthenticationStatus.errorHttp) {
                    document.getElementById("TwitterDebugArea").value += "Error returned: " + result.responseErrorDetail + "\r\n";
                }
                authzInProgress = false;
            }, function (err) {
                WinJS.log("Error returned by WebAuth broker: " + err, "Web Authentication SDK Sample", "error");
                document.getElementById("TwitterDebugArea").value += " Error Message: " + err.message + "\r\n";
                authzInProgress = false;
            });
    }
})();
