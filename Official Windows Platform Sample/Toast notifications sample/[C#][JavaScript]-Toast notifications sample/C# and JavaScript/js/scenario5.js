//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        processed: function (element) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Clicking on a toast from this scenario will cause the activation event to be fired.
            if (WinJS.Navigation.state) {
                try {
                    var parameters = JSON.parse(WinJS.Navigation.state);
                    WinJS.log && WinJS.log("You clicked on a toast with parameters param1=" + parameters.param1 + " and param2=" + parameters.param2, "sample", "status");
                } catch (ex) {
                    WinJS.log && WinJS.log("Error parsing the toast parameters" + parameters.param1 + " and param2=" + parameters.param2, "sample", "error");
                }
            }
        },

        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.
            document.getElementById("displayToastButton").addEventListener("click", displayToast, false);
            document.getElementById("hideToastButton").addEventListener("click", hideToast, false);
        }
    });

    var Notifications = Windows.UI.Notifications;
    var ToastContent = NotificationsExtensions.ToastContent;

    var lastToast;
    function displayToast() {
        var content = ToastContent.ToastContentFactory.createToastText02();

        content.textHeading.text = "Tap toast";
        content.textBodyWrap.text = "Or swipe to dismiss";
        content.launch = '{"type":"toast","param1":"12345","param2":"67890"}';

        lastToast = content.createNotification();

        // You can listen the "activated" event off the toast object
        // or listen to the "activated" event off the Windows.UI.WebUI.WebUIApplication
        // object to tell when the user clicks the toast.
        //
        // The difference is that the WebUIApplication event will
        // be raised by local and cloud toasts, while the event off the
        // toast object will only be raised by local toasts.
        //
        // In this example, we'll use the event off the WebUIApplication object.
        // The registration for the event is in the "initialize" function, which is
        // called on DOMContentLoaded. This ensures that the app can respond to the
        // toast before displaying any content.

        lastToast.addEventListener("dismissed", function (e) {
            switch (e.reason) {
                case Notifications.ToastDismissalReason.applicationHidden:
                    WinJS.log && WinJS.log("The application hid the toast using toastNotifier.hide()", "sample", "status");
                    break;
                case Notifications.ToastDismissalReason.userCanceled:
                    WinJS.log && WinJS.log("The user dismissed this toast", "sample", "status");
                    break;
                case Notifications.ToastDismissalReason.timedOut:
                    WinJS.log && WinJS.log("The toast has timed out", "sample", "status");
                    break;
            }
        }, false);

        lastToast.addEventListener("failed", function (e) {
            WinJS.log && WinJS.log("The toast encountered an error", "sample", "error");
        }, false);

        Notifications.ToastNotificationManager.createToastNotifier().show(lastToast);
    }

    function hideToast(e) {
        if (lastToast) {
            Notifications.ToastNotificationManager.createToastNotifier().hide(lastToast);
            lastToast = null;
        } else {
            WinJS.log && WinJS.log("No toast has been displayed from Scenario 5", "sample", "error");
        }
    }
})();
