//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
        ready: function (element, options) {
            document.getElementById("loopingButton").addEventListener("click", displayLongToast, false);
            document.getElementById("loopingButtonString").addEventListener("click", displayLongToastWithStringManipulation, false);
            document.getElementById("noLoopingButton").addEventListener("click", displayLongToast, false);
            document.getElementById("noLoopingButtonString").addEventListener("click", displayLongToastWithStringManipulation, false);
            document.getElementById("hideToastButton").addEventListener("click", hideToast, false);
        }
    });

    var Notifications = Windows.UI.Notifications;
    var ToastContent = NotificationsExtensions.ToastContent;

    var lastToast;
    function displayLongToast(e) {
        var content = ToastContent.ToastContentFactory.createToastText02();

        content.textHeading.text = "Long Toast";
        content.duration = ToastContent.ToastDuration.long;

        // The "loop" attribute decides if the toast's audio will repeat.
        if (e.currentTarget.id === "loopingButton") {
            content.audio.content = ToastContent.ToastAudioContent.loopingAlarm;
            content.audio.loop = true;
            content.textBodyWrap.text = "Looping audio";
        } else {
            content.audio.content = ToastContent.ToastAudioContent.im;
        }

        lastToast = content.createNotification();
        Notifications.ToastNotificationManager.createToastNotifier().show(lastToast);

        WinJS.log && WinJS.log(content.getContent(), "sample", "status");
    }

    function displayLongToastWithStringManipulation(e) {
        var toastXmlString;
        // The "loop" attribute decides if the toast's audio will repeat.
        if (e.currentTarget.id === "loopingButtonString") {
            toastXmlString = "<toast duration='long'>"
                           + "<visual version='1'>"
                           + "<binding template='ToastText02'>"
                           + "<text id='1'>Long Toast</text>"
                           + "<text id='2'>Looping audio</text>"
                           + "</binding>"
                           + "</visual>"
                           + "<audio loop='true' src='ms-winsoundevent:Notification.Looping.Alarm'/>"
                           + "</toast>";
        } else {
            toastXmlString = "<toast duration='long'>"
                           + "<visual version='1'>"
                           + "<binding template='ToastText02'>"
                           + "<text id='1'>Long Toast</text>"
                           + "</binding>"
                           + "</visual>"
                           + "<audio loop='true' src='ms-winsoundevent:Notification.IM'/>"
                           + "</toast>";
        }

        var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
        toastDOM.loadXml(toastXmlString);

        lastToast = new Notifications.ToastNotification(toastDOM);
        Notifications.ToastNotificationManager.createToastNotifier().show(lastToast);

        WinJS.log && WinJS.log(toastDOM.getXml(), "sample", "status");
    }

    // For long duration toasts, there may be times when hiding the toast before it times out is appropriate.
    function hideToast(e) {
        if (lastToast) {
            Notifications.ToastNotificationManager.createToastNotifier().hide(lastToast);
            lastToast = null;
        } else {
            WinJS.log && WinJS.log("No toast has been displayed from Scenario 6", "sample", "error");
        }
    }

})();
