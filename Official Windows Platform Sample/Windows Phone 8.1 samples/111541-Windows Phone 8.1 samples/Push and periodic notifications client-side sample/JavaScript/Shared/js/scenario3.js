//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("scenario2StartListening").addEventListener("click", startListening, false);
            document.getElementById("scenario2StopListening").addEventListener("click", stopListening, false);

            WinJS.Navigation.addEventListener("navigated", onNavigation, false);
        }
    });

    function onNavigation(e) {
        var channel = SampleNotifications.currentChannel;
        if (channel) {
            channel.removeEventListener("pushnotificationreceived", pushNotificationReceivedHandler);
        }
        WinJS.Navigation.removeEventListener("navigated", onNavigation);
    }

    var pushNotifications = Windows.Networking.PushNotifications;

    function pushNotificationReceivedHandler(e) {
        var notificationTypeName = "";
        var notificationPayload;
        switch (e.notificationType) {
            // You can get the toast, tile, or badge notification object.
            // In this example, we take the XML from that notification and display it.
            case pushNotifications.PushNotificationType.toast:
                notificationTypeName = "Toast";
                notificationPayload = e.toastNotification.content.getXml();
                break;
            case pushNotifications.PushNotificationType.tile:
                notificationTypeName = "Tile";
                notificationPayload = e.tileNotification.content.getXml();
                break;
            case pushNotifications.PushNotificationType.badge:
                notificationTypeName = "Badge";
                notificationPayload = e.badgeNotification.content.getXml();
                break;
            case pushNotifications.PushNotificationType.raw:
                notificationTypeName = "Raw";
                notificationPayload = e.rawNotification.content;
                break;
        }

        // Setting the cancel property prevents the notification from being delivered. It's especially important to do this for toasts:
        // if your application is already on the screen, there's no need to display a toast from push notifications.
        e.cancel = true;
        WinJS.log && WinJS.log(notificationTypeName + " notification was received and canceled. Notification payload: " + notificationPayload, "sample", "status");
    }

    // You can listen for notification events using addEventListener...
    function startListening() {
        var channel = SampleNotifications.currentChannel;
        if (channel) {
            channel.addEventListener("pushnotificationreceived", pushNotificationReceivedHandler);
            WinJS.log && WinJS.log("Now listening for push notification events.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("No channel is open.", "sample", "error");
        }
    }
     
    // ...or stop using removeEventListener
    function stopListening() {
        var channel = SampleNotifications.currentChannel;
        if (channel) {
            channel.removeEventListener("pushnotificationreceived", pushNotificationReceivedHandler);
            WinJS.log && WinJS.log("No longer listening for push notification events.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("No channel is open.", "sample", "error");
        }
    }
})();
