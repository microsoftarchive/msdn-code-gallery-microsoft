//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/manageAccounts.html", {
        ready: function (element, options) {
            document.getElementById("ShowButton").addEventListener("click", showButtonClicked, false);
            var facebookToken = roamingSettings.values["FACEBOOK_OAUTH_TOKEN"];
            if (!facebookToken)
            {
                isFacebookUserLoggedIn = false;
            }
            else
            {
                var facebookUser = roamingSettings.values["FACEBOOK_USER_NAME"];
                if (facebookUser)
                {
                    facebookUserName = facebookUser;
                    isFacebookUserLoggedIn = true;
                }
            }

            var twitterToken = roamingSettings.values["TWITTER_OAUTH_TOKEN"];
            var twitterTokenSecret = roamingSettings.values["TWITTER_OAUTH_TOKEN_SECRET"];
            if (!twitterToken || !twitterTokenSecret)
            {
                isTwitterUserLoggedIn = false;
            }
            else
            {
                var twitteruser = roamingSettings.values["TWITTER_USER_NAME"];
                if (twitteruser)
                {
                    twitterUserName = twitteruser;
                    isTwitterUserLoggedIn = true;
                }
            }

        }
    });
    var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;
    var facebookUserName;
    var twitterUserName;
    var isFacebookUserLoggedIn = false;
    var isTwitterUserLoggedIn = false;
    
    var settingsPane = Windows.UI.ApplicationSettings.SettingsPane.getForCurrentView();

    /// <summary>
    /// This event is generated when the user opens the settings pane. During this event, append your
    /// SettingsCommand objects to the available ApplicationCommands vector to make them available to the
    /// SettingsPane UI.
    /// </summary>
    settingsPane.oncommandsrequested = function (args) {
        args.request.applicationCommands.append(
            Windows.UI.ApplicationSettings.SettingsCommand.accountsCommand);
    };

    var accountSettingsPane = Windows.UI.ApplicationSettings.AccountsSettingsPane.getForCurrentView();
    
    /// <summary>
    /// This event is generated when the user clicks on Accounts command in settings pane. During this event, add your
    /// WebAccountProviderCommand, WebAccountCommand, CredentialCommand and  SettingsCommand objects to make them available to the
    /// AccountsSettingsPane UI.
    /// </summary>
    accountSettingsPane.onaccountcommandsrequested = function (args) {
        //Header text
        args.headerText = "This is sample text. You can put a message here to give context to user. This section is optional.";
        
        //Add account providers.
        var webAccountProviderInvokedhandled = function (command) {
            document.getElementById("OutPutTextArea").value += "webAccountProviderInvokedhandled: " + command.webAccountProvider.id + "\r\n";
            if (command.webAccountProvider.id === "Twitter.com") {
                if (!isTwitterUserLoggedIn) {
                    launchTwitterWebAuth();
                } else {
                    document.getElementById("OutPutTextArea").value +=
                        "User is already logged in. If you support multiple accounts from the same provider then do something here to connect new user.\r\n";
                }
            }
            else if (command.webAccountProvider.id === "Facebook.com")
            {
                if (!isFacebookUserLoggedIn) {
                    launchFacebookWebAuth();
                } else {
                    document.getElementById("OutPutTextArea").value +=
                        "User is already logged in. If you support multiple accounts from the same provider then do something here to connect new user.\r\n";
                }
            }
        };

        var facebookProvider = Windows.Security.Credentials.WebAccountProvider("Facebook.com", "Facebook", new Windows.Foundation.Uri("ms-appx:///icons/Facebook.png"));
        var facebookProviderCommand = Windows.UI.ApplicationSettings.WebAccountProviderCommand(facebookProvider, webAccountProviderInvokedhandled);
        args.webAccountProviderCommands.append(facebookProviderCommand);

        var twitterProvider = Windows.Security.Credentials.WebAccountProvider("Twitter.com", "Twitter", new Windows.Foundation.Uri("ms-appx:///icons/Twitter.png"));
        var twitterProviderCommand = Windows.UI.ApplicationSettings.WebAccountProviderCommand(twitterProvider, webAccountProviderInvokedhandled);
        args.webAccountProviderCommands.append(twitterProviderCommand);

        //Add Accounts
        var webAccountInvokedhandled = function (command, accountArgs) {
            document.getElementById("OutPutTextArea").value += "webAccountInvokedhandled: " + command.webAccount.state + " and Selected Action = " + accountArgs.action + "\r\n";
            if (accountArgs.action === Windows.UI.ApplicationSettings.WebAccountAction.remove) {
                //Remove user logon information since user requested to remove account.
                if (command.webAccount.webAccountProvider.id === "Facebook.com") {
                    roamingSettings.values.remove("FACEBOOK_USER_NAME");
                    roamingSettings.values.remove("FACEBOOK_OAUTH_TOKEN");
                    isFacebookUserLoggedIn = false;
                }
                else if (command.webAccount.webAccountProvider.id === "Twitter.com") {
                    roamingSettings.values.remove("TWITTER_USER_NAME");
                    roamingSettings.values.remove("TWITTER_OAUTH_TOKEN");
                    isTwitterUserLoggedIn = false;
                }
            }
        };

        if (isFacebookUserLoggedIn)
        {
            var facebookAccount= Windows.Security.Credentials.WebAccount(facebookProvider, facebookUserName, Windows.Security.Credentials.WebAccountState.connected);
            var facebookAccountCommand = Windows.UI.ApplicationSettings.WebAccountCommand(
            facebookAccount, webAccountInvokedhandled,
            Windows.UI.ApplicationSettings.SupportedWebAccountActions.remove |  Windows.UI.ApplicationSettings.SupportedWebAccountActions.manage);
            args.webAccountCommands.append(facebookAccountCommand);
        }

        if (isTwitterUserLoggedIn)
        {
            var twitterAccount = Windows.Security.Credentials.WebAccount(twitterProvider, twitterUserName, Windows.Security.Credentials.WebAccountState.connected);
            var twitterAccountCommand =  Windows.UI.ApplicationSettings.WebAccountCommand(
            twitterAccount, webAccountInvokedhandled,
            Windows.UI.ApplicationSettings.SupportedWebAccountActions.remove | Windows.UI.ApplicationSettings.SupportedWebAccountActions.manage);
            args.webAccountCommands.append(twitterAccountCommand);
        }


        //Add global links
        var commandInvokedHandler = function (command) {
            document.getElementById("OutPutTextArea").value = "Link Clicked: " + command.label + "\r\n";
            
        };

        var appSpecifiedCmd = Windows.UI.ApplicationSettings.SettingsCommand(
                                                                    1,
                                                                    "Privacy policy",
                                                                    commandInvokedHandler);

        args.commands.append(appSpecifiedCmd);
    };

    /// <summary>
    /// Event handler for Show button. This method demonstrates how to show AccountsSettings pane programatically.
    /// </summary>
    function showButtonClicked(args) {

        Windows.UI.ApplicationSettings.AccountsSettingsPane.show();
    }

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

    function launchFacebookWebAuth() {
        var facebookURL = "https://www.facebook.com/dialog/oauth?client_id=";

        var clientID = document.getElementById("FacebookClientID").value;
        if (clientID === null || clientID === "") {
            WinJS.log("Enter a ClientID", "Web Authentication SDK Sample", "error");
            return;
        }

        var callbackURL = document.getElementById("FacebookCallbackUrl").value;
        if (!isValidUriString(callbackURL)) {
            WinJS.log("Enter a valid Callback URL for Facebook", "Web Authentication SDK Sample", "error");
            return;
        }

        facebookURL += clientID + "&redirect_uri=" + encodeURIComponent(callbackURL) + "&scope=read_stream&display=popup&response_type=token";

        if (authzInProgress) {
            document.getElementById("OutPutTextArea").value += "Authorization already in Progress ... " + "\r\n";
            return;
        }

        var startURI = new Windows.Foundation.Uri(facebookURL);
        var endURI = new Windows.Foundation.Uri(callbackURL);

        document.getElementById("OutPutTextArea").value += "Navigating to: " + facebookURL + "\r\n";

        authzInProgress = true;
        Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
            Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startURI, endURI)
            .done(function (result) {
                document.getElementById("TwitterReturnedToken").value = result.responseData;
                getfacebookUserName(result.responseData);
                document.getElementById("OutPutTextArea").value += "Status returned by WebAuth broker: " + result.responseStatus + "\r\n";
                if (result.responseStatus === Windows.Security.Authentication.Web.WebAuthenticationStatus.errorHttp) {
                    document.getElementById("OutPutTextArea").value += "Error returned: " + result.responseErrorDetail + "\r\n";
                }
                isFacebookUserLoggedIn = true;
                authzInProgress = false;
            }, function (err) {
                WinJS.log("Error returned by WebAuth broker: " + err, "Web Authentication SDK Sample", "error");
                document.getElementById("OutPutTextArea").value += " Error Message: " + err.message + "\r\n";
                authzInProgress = false;
            });
    }

    /// <summary>
    /// This function extracts access_token from the response returned from web authentication broker
    /// and uses that token to get user information using facebook graph api. 
    /// </summary>
    function getfacebookUserName(webAuthResultResponseData) {

        var responseData = webAuthResultResponseData.substring(webAuthResultResponseData.indexOf("access_token"));
        var keyValPairs = responseData.split("&");
        var access_token;
        var expires_in;
        for (var i = 0; i < keyValPairs.length; i++) {
            var splits = keyValPairs[i].split("=");
            switch (splits[0]) {
                case "access_token":
                    access_token = splits[1];
                    break;
                case "expires_in":
                    expires_in = splits[1];
                    break;
            }
        }

        document.getElementById("OutPutTextArea").value += "access_token = " + access_token + "\r\n";
        var response = sendRequest("https://graph.facebook.com/me?access_token=" + access_token);
        var userInfo = JSON.parse(response);
        facebookUserName = userInfo.name;
        document.getElementById("OutPutTextArea").value += userInfo.name + " is connected!! \r\n";

        roamingSettings.values["FACEBOOK_USER_NAME"] = userInfo.name; //store user name locally for further use.
        roamingSettings.values["FACEBOOK_OAUTH_TOKEN"] = access_token; //store access token locally for further use.
    }

    


    function launchTwitterWebAuth() {
        
        // Get all the parameters from the user
        var clientID = document.getElementById("TwitterClientID").value;
        if (clientID === null || clientID === "") {
            WinJS.log("Please enter a ClientID for Twitter App", "Web Authentication SDK Sample", "error");            
            return;
        }

        var clientSecret = document.getElementById("TwitterClientSecret").value;
        if (clientSecret === null || clientSecret === "") {
            WinJS.log("Please enter a Secret for Twitter App", "Web Authentication SDK Sample", "error");            
            return;
        }

        var callbackURL = document.getElementById("TwitterCallbackUrl").value;
        if (!isValidUriString(callbackURL)) {
            WinJS.log("Please enter a Callback URL for Twitter", "Web Authentication SDK Sample", "error");            
            return;
        }

        if (authzInProgress) {
            document.getElementById("OutPutTextArea").value += "Authorization already in Progress ... \r\n";
            return;
        }

        // Acquiring a request token
        var oauth_token = getTwitterRequestToken(callbackURL, clientID, clientSecret);
        
        // Send the user to authorization
        var twitterURL = "https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;

        document.getElementById("OutPutTextArea").value += "\r\nNavigating to: " + twitterURL + "\r\n";
        var startURI = new Windows.Foundation.Uri(twitterURL);
        var endURI = new Windows.Foundation.Uri(callbackURL);

        authzInProgress = true;
        Windows.Security.Authentication.Web.WebAuthenticationBroker.authenticateAsync(
            Windows.Security.Authentication.Web.WebAuthenticationOptions.none, startURI, endURI)
            .done(function (result) {
                getTwitterUserName(result.responseData, clientID, clientSecret);
                document.getElementById("TwitterReturnedToken").value = result.responseData;
                document.getElementById("OutPutTextArea").value += "Status returned by WebAuth broker: " + result.responseStatus + "\r\n";
                if (result.responseStatus === Windows.Security.Authentication.Web.WebAuthenticationStatus.errorHttp) {
                    document.getElementById("OutPutTextArea").value += "Error returned: " + result.responseErrorDetail + "\r\n";
                }
                isTwitterUserLoggedIn = true;
                authzInProgress = false;
            }, function (err) {
                WinJS.log("Error returned by WebAuth broker: " + err, "Web Authentication SDK Sample", "error");
                document.getElementById("OutPutTextArea").value += " Error Message: " + err.message + "\r\n";
                authzInProgress = false;
            });
    }

    /// <summary>
    /// This function extracts oauth_token and oauth_verifier from the response returned from web authentication broker
    /// and uses that token to get Twitter access token. 
    /// </summary>
    function getTwitterUserName(webAuthResultResponseData, clientId, clientSecret) {

        //
        // Acquiring a access_token first.
        //
        var responseData = webAuthResultResponseData.substring(webAuthResultResponseData.indexOf("oauth_token"));
        var request_token;
        var oauth_verifier;
        var keyValPairs = responseData.split("&");

        for (var i = 0; i < keyValPairs.length; i++) {
            var splits = keyValPairs[i].split("=");
            switch (splits[0]) {
                case "oauth_token":
                    request_token = splits[1];
                    break;
                case "oauth_verifier":
                    oauth_verifier = splits[1];
                    break;
            }
        }

        var twitterURL = "https://api.twitter.com/oauth/access_token";
        var timeStamp = getTimeStamp();
        var nonce = getNonce();

        var sigBaseStringParams = "oauth_consumer_key=" + clientId;
        sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
        sigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
        sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
        sigBaseStringParams += "&" + "oauth_token=" + request_token;
        sigBaseStringParams += "&" + "oauth_version=1.0";
        var sigBaseString = "POST&";
        sigBaseString += encodeURIComponent(twitterURL) + "&" + encodeURIComponent(sigBaseStringParams);

        var keyText = clientSecret + "&";
        var signature = getSignature(sigBaseString, keyText);
        var authorizationHeaderParams = "OAuth oauth_consumer_key=\"" + clientId + "\", oauth_nonce=\"" + nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + encodeURIComponent(signature) + "\", oauth_timestamp=\"" + timeStamp + "\", oauth_token=\"" + encodeURIComponent(request_token) + "\", oauth_version=\"1.0\"";

        var request = new XMLHttpRequest();
        request.open("POST", twitterURL, false);
        request.setRequestHeader("Authorization", authorizationHeaderParams);
        request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.send("oauth_verifier=" + oauth_verifier);
        var response = request.responseText;

        var access_token;
        var oauth_token_secret;
        var screen_name;
        keyValPairs = response.split("&");

        for (var j = 0; j < keyValPairs.length; j++) {
            var tokens = keyValPairs[j].split("=");
            switch (tokens[0]) {
                case "oauth_token":
                    access_token = tokens[1];
                    break;
                case "oauth_token_secret":
                    oauth_token_secret = tokens[1];
                    break;
                case "screen_name":
                    screen_name = tokens[1];
                    break;
            }
        }

        document.getElementById("OutPutTextArea").value += "access_token = " + access_token + "\r\n";
        document.getElementById("OutPutTextArea").value += "oauth_token_secret = " + oauth_token_secret + "\r\n";
        document.getElementById("OutPutTextArea").value += "\r\n" + screen_name + " is connected!!" + "\r\n";

        roamingSettings.values["TWITTER_OAUTH_TOKEN"] = access_token; //store access token for further use.
        roamingSettings.values["TWITTER_OAUTH_TOKEN_SECRET"] = oauth_token_secret; //store token secret for further use.
        roamingSettings.values["TWITTER_USER_NAME"] = screen_name; //Store user name locally for further use.
        twitterUserName = screen_name;
    }


    function getTwitterRequestToken(callbackURL, clientID, clientSecret) {

        var twitterURL = "https://api.twitter.com/oauth/request_token";
        var timestamp = getTimeStamp();
        var nonce = getNonce();

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
        var signature = getSignature(sigBaseString, keyText);
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
        document.getElementById("OutPutTextArea").value += "\r\noauth_token = " + oauth_token;
        document.getElementById("OutPutTextArea").value += "\r\noauth_token = " + oauth_token_secret;

        return oauth_token;
    }

    function getNonce()
    {
        var nonce = Math.random();
        nonce = Math.floor(nonce * 1000000000);
        return nonce;
    }

    function getTimeStamp()
    {
        var timestamp = Math.round(new Date().getTime() / 1000.0);
        return timestamp;
    }

    function getSignature(sigBaseString, keyText)
    {
        var keyMaterial = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(keyText, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
        var macAlgorithmProvider = Windows.Security.Cryptography.Core.MacAlgorithmProvider.openAlgorithm("HMAC_SHA1");
        var key = macAlgorithmProvider.createKey(keyMaterial);
        var tbs = Windows.Security.Cryptography.CryptographicBuffer.convertStringToBinary(sigBaseString, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
        var signatureBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.sign(key, tbs);
        var signature = Windows.Security.Cryptography.CryptographicBuffer.encodeToBase64String(signatureBuffer);
        
        return signature;
    }
})();
