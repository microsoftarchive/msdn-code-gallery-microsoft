//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var configuration = {};

(function () {
    configuration.authenticationHost = "login.contoso.com";
    configuration.userName = "MyUserName";
    configuration.password = "MyPassword";
    configuration.extraParameters = "";
    configuration.markAsManualConnect = false;

    function isAuthenticateThroughBackgroundTask() {
        var settings = Windows.Storage.ApplicationData.current.localSettings;
        var value = settings.values.lookup("background");
        if (value !== null) {
            return value;
        }
        return true; // default value
    }
    configuration.isAuthenticateThroughBackgroundTask = isAuthenticateThroughBackgroundTask;

    function setAuthenticateThroughBackgroundTask(value) {
        var settings = Windows.Storage.ApplicationData.current.localSettings;
        settings.values["background"] = value;
    }
    configuration.setAuthenticateThroughBackgroundTask = setAuthenticateThroughBackgroundTask;

    function getAuthenticationToken() {
        var settings = Windows.Storage.ApplicationData.current.localSettings;
        var value = settings.values.lookup("token");
        if (value !== null) {
            return value;
        }
        return ""; // default value
    }
    configuration.getAuthenticationToken = getAuthenticationToken;

    function setAuthenticationToken(value) {
        var settings = Windows.Storage.ApplicationData.current.localSettings;
        settings.values["token"] = value;
    }
    configuration.setAuthenticationToken = setAuthenticationToken;
})();
