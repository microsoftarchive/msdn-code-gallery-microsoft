//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var pushNotifications = Windows.Networking.PushNotifications;

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("scenario2AttachListener").addEventListener("click", scenario2AddListener, false);
            document.getElementById("scenario2RemoveListener").addEventListener("click", scenario2RemoveListener, false);
        }
    });

    WinJS.Navigation.addEventListener("beforenavigate", function () {
        if (WinJS.Navigation.location === "/html/scenario2.html" && SdkSample.channel) {
            SdkSample.channel.removeEventListener("pushnotificationreceived", onPushNotification, false);
        }
    }, false);

    function onPushNotification(e) {
        if (e.notificationType === pushNotifications.PushNotificationType.raw) {
            WinJS.log && WinJS.log("Raw notification received with content: " + e.rawNotification.content, "sample", "status");
            // Prevents the notification from being delivered to the background task
            e.cancel = true;
        }
    }

    function scenario2AddListener() {
        if (SdkSample.channel) {
            SdkSample.channel.addEventListener("pushnotificationreceived", onPushNotification, false);
            WinJS.log && WinJS.log("Now listening for raw notifications", "sample", "status");
        } else {
            WinJS.log && WinJS.log("No channel is open", "sample", "error");
        }
    }

    function scenario2RemoveListener() {
        if (SdkSample.channel) {
            SdkSample.channel.removeEventListener("pushnotificationreceived", onPushNotification, false);
            WinJS.log && WinJS.log("No longer listening for raw notifications", "sample", "status");
        } else {
            WinJS.log && WinJS.log("No channel is open", "sample", "error");
        }
    }
})();
