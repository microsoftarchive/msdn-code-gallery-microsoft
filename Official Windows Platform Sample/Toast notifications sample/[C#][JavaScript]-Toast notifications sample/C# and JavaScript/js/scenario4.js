//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            var buttons = document.querySelectorAll("button.toastButton");
            for (var i = 0, len = buttons.length; i < len; i++) {
                buttons[i].addEventListener("click", displayAudioToast, false);
            }

            buttons = document.querySelectorAll("button.toastStringButton");
            for (i = 0, len = buttons.length; i < len; i++) {
                buttons[i].addEventListener("click", displayAudioToastWithStringManipulation, false);
            }
        }
    });

    var Notifications = Windows.UI.Notifications;
    var ToastContent = NotificationsExtensions.ToastContent;

    function displayAudioToast(e) {
        // Get some data from the button the user clicked on.
        var targetButton = e.currentTarget;
        var toastSoundSource = targetButton.id;

        // Get the toast manager for the current app.
        var notificationManager = Notifications.ToastNotificationManager;

        var content = ToastContent.ToastContentFactory.createToastText02();

        content.audio.content = ToastContent.ToastAudioContent[toastSoundSource];
        content.textHeading.text = "Sound:";
        content.textBodyWrap.text = toastSoundSource;

        // Displays the XML of the toast.
        WinJS.log && WinJS.log(content.getContent(), "sample", "status");

        // Create a toast, then create a ToastNotifier object
        // to send the toast.
        var toast = content.createNotification();

        // If you have other apps in your package, you can specify the appID of that app
        // to create a ToastNotifier for that app.
        notificationManager.createToastNotifier().show(toast);
    }

    function displayAudioToastWithStringManipulation(e) {
        // Get some data from the button the user clicked on.
        var targetButton = e.currentTarget;
        var toastSoundSource = targetButton.id.substring(6);

        // Get the toast manager for the current app.
        var notificationManager = Notifications.ToastNotificationManager;
        var toastXmlString;
        if (toastSoundSource === "silent") {
            toastXmlString = "<toast>"
                           + "<visual version='1'>"
                           + "<binding template='ToastText02'>"
                           + "<text id='1'>Sound:</text>"
                           + "<text id='2'>" + toastSoundSource + "</text>"
                           + "</binding>"
                           + "</visual>"
                           + "<audio silent='true'/>"
                           + "</toast>";

        } else if (toastSoundSource === "default") {
            toastXmlString = "<toast>"
                       + "<visual version='1'>"
                       + "<binding template='ToastText02'>"
                       + "<text id='1'>Sound:</text>"
                       + "<text id='2'>" + toastSoundSource + "</text>"
                       + "</binding>"
                       + "</visual>"
                       + "</toast>";

        } else {
            toastXmlString = "<toast>"
                       + "<visual version='1'>"
                       + "<binding template='ToastText02'>"
                       + "<text id='1'>Sound:</text>"
                       + "<text id='2'>" + toastSoundSource + "</text>"
                       + "</binding>"
                       + "</visual>"
                       + "<audio src='ms-winsoundevent:Notification." + toastSoundSource + "'/>"
                       + "</toast>";
        }

        // Create a toast, then create a ToastNotifier object
        // to send the toast.
        var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
        try {
            toastDOM.loadXml(toastXmlString);
            var toast = new Notifications.ToastNotification(toastDOM);

            // Display the XML of the toast.
            WinJS.log && WinJS.log(toastDOM.getXml(), "sample", "status");

            // If you have other apps in your package, you can specify the appID of that app
            // to create a ToastNotifier for that app.
            notificationManager.createToastNotifier().show(toast);
        } catch (e) {
            WinJS.log && WinJS.log("Error loading the xml, check for invalid characters in the input", "sample", "error");
        }
    }
})();
