//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var Notifications = Windows.UI.Notifications;
    var Background = Windows.ApplicationModel.Background;

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("scenario1RequestLockScreenAccess").addEventListener("click", scenario1RequestLockScreenAccess, false);
            document.getElementById("scenario1RemoveLockScreenAccess").addEventListener("click", scenario1RemoveLockScreenAccess, false);
            document.getElementById("scenario1QueryLockScreenAccess").addEventListener("click", scenario1QueryLockScreenAccess, false);
        }
    });

    function scenario1RequestLockScreenAccess() {
        // An app can call the add or query API as many times as it wants; however, it will only present the dialog box to the user one time.
        Background.BackgroundExecutionManager.requestAccessAsync().done(function (result) {
            switch (result) {
                case Background.BackgroundAccessStatus.denied:
                    WinJS.log && WinJS.log("This app is not on the lock screen.", "sample", "status");
                    break;

                case Background.BackgroundAccessStatus.allowedWithAlwaysOnRealTimeConnectivity:
                    WinJS.log && WinJS.log("This app is on the lock screen and has access to Always-On Real Time Connectivity.", "sample", "status");
                    break;

                case Background.BackgroundAccessStatus.allowedMayUseActiveRealTimeConnectivity:
                    WinJS.log && WinJS.log("This app is on the lock screen and has access to Active Real Time Connectivity.", "sample", "status");
                    break;

                case Background.BackgroundAccessStatus.unspecified:
                    WinJS.log && WinJS.log("The user has not yet taken any action. This is the default setting and the app is not on the lock screen.", "sample", "status");
                    break;
            }
        }, function (e) {
            WinJS.log && WinJS.log(e.message, "sample", "error");
        });
    }

    function scenario1RemoveLockScreenAccess() {
        Background.BackgroundExecutionManager.removeAccess();
        WinJS.log && WinJS.log("This app has been removed from the lock screen.", "sample", "status");
    }

    function scenario1QueryLockScreenAccess() {
        var result = Background.BackgroundExecutionManager.getAccessStatus();
        switch (result) {
            case Background.BackgroundAccessStatus.denied:
                WinJS.log && WinJS.log("This app is not on the lock screen.", "sample", "status");
                break;

            case Background.BackgroundAccessStatus.allowedWithAlwaysOnRealTimeConnectivity:
                WinJS.log && WinJS.log("This app is on the lock screen and has access to Always-On Real Time Connectivity.", "sample", "status");
                break;

            case Background.BackgroundAccessStatus.allowedMayUseActiveRealTimeConnectivity:
                WinJS.log && WinJS.log("This app is on the lock screen and has access to Active Real Time Connectivity.", "sample", "status");
                break;

            case Background.BackgroundAccessStatus.unspecified:
                WinJS.log && WinJS.log("The user has not yet taken any action. This is the default setting and the app is not on the lock screen.", "sample", "status");
                break;
        }
    }

})();
