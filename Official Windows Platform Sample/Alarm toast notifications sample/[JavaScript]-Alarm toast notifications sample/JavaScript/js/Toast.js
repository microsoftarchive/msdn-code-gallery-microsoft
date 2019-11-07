//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var delayedToast;
    var snoozeInterval;

    var page = WinJS.UI.Pages.define("/html/Toast.html", {
        ready: function (element, options) {
            var buttons = document.querySelectorAll("button.toastButton");
            for (var i = 0; i < buttons.length; i++) {
                buttons[i].addEventListener("click", displayTextToast, false);
            }
            delayedToast = document.getElementById("delayedToast");
            snoozeInterval = document.getElementById("customSnoozeInterval");
            try {
                Windows.ApplicationModel.Background.AlarmApplicationManager.requestAccessAsync().done(function () { });
            }
            catch (exception) {
                // RequestAccessAsync may throw an exception if the app is not currently in the foreground
            }
        }
    });

    function displayTextToast(e) {
        // Get some data from the button the user clicked on.
        var targetButton = e.currentTarget;
        var toastTemplate = targetButton.id;
        var alarmName = "";

        if (toastTemplate.indexOf("Custom") >= 0) {
            alarmName = "Wake up Time with Custom Snooze!";
        } else {
            alarmName = "Wake up Time with Default Snooze!";
        }

        // Create the toast content by direct string manipulation.
        // See the Toasts SDK Sample for other ways of generating toasts.
        var xmlPayload =
           "<toast duration=\"long\">\n" +
                        "<visual>\n" +
                            "<binding template=\"ToastText02\">\n" +
                                "<text id=\"1\">Alarms Notifications SDK Sample App</text>\n" +
                                "<text id=\"2\">" + alarmName + "</text>\n" +
                            "</binding>\n" +
                        "</visual>\n" +
                        "<commands scenario=\"alarm\">\n" +
                            "<command id=\"snooze\"/>\n" +
                            "<command id=\"dismiss\"/>\n" +
                        "</commands>\n" +
                        "<audio src=\"ms-winsoundevent:Notification.Looping.Alarm2\" loop=\"true\" />\n" +
                    "</toast>\n";
        
        // Display the generated XML for demonstration purposes.
        WinJS.log(xmlPayload, "sample", "status");

        // Create an XML document from the XML.
        var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
        toastDOM.loadXml(xmlPayload);

        // Prepare to raise the toast.
        var toaster = Windows.UI.Notifications.ToastNotificationManager.createToastNotifier();

        // Set the toast due time per user selection
        var dueTime = new Windows.Globalization.Calendar();
        dueTime.setToNow();
        dueTime.addSeconds(delayedToast.value);

        var scheduledToast;
        if (toastTemplate.indexOf("Custom") >= 0) {
            // Schedule the toast with custom snooze.
            var snooze = snoozeInterval.value * 60 * 1000;
            scheduledToast = new Windows.UI.Notifications.ScheduledToastNotification(toastDOM, dueTime.getDateTime(), snooze, 0);
            toaster.addToSchedule(scheduledToast);
        } else {
            // Schedule the toast with default snooze
            scheduledToast = new Windows.UI.Notifications.ScheduledToastNotification(toastDOM, dueTime.getDateTime());
            toaster.addToSchedule(scheduledToast);
        }
    }

})();
