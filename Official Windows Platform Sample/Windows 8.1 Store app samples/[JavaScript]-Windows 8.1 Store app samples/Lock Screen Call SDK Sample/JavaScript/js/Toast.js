//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var delayedToast;
    var callerType;

    var page = WinJS.UI.Pages.define("/html/Toast.html", {
        ready: function (element, options) {
            var buttons = document.querySelectorAll("button.toastButton");
            for (var i = 0, len = buttons.length; i < len; i++) {
                buttons[i].addEventListener("click", displayTextToast, false);
            }
            delayedToast = document.getElementById("delayedToast");
            callerType = document.getElementById("callerType");
            Windows.ApplicationModel.Background.BackgroundExecutionManager.requestAccessAsync().done();
        }
    });

    function displayTextToast(e) {
        // Get some data from the button the user clicked on.
        var targetButton = e.currentTarget;
        var toastTemplate = targetButton.id;

        // Decide what buttons are enabled in this scenario.
        var videoButtonXml = "";
        var voiceButtonXml = "";
        var infoString;
        
        // The arguments attribute contains application-defined information.
        // You should use it to identify which button was pressed and which call is being answered.
        // Our sample uses the format
        //   "<call mode> <caller identity> <simulated call duration>"

        function buildArguments(mode) {
            return [mode, callerType.value, callTimeout.value].join(" ");
        }

        if (toastTemplate.indexOf("Video") >= 0) {
            videoButtonXml = '<command id="video" arguments="' + buildArguments("Video") + '"/>';
            infoString = "Incoming video call";
        } else {
            infoString = "Incoming voice call";
        }
        if  (toastTemplate.indexOf("Voice") >= 0)
        {
            voiceButtonXml = '<command id="voice" arguments="' + buildArguments("Voice") + '"/>';
        }

        // Create the toast content by direct string manipulation.
        // See the Toasts SDK Sample for other ways of generating toasts.
        var xmlPayload =
            '<toast duration="long">'
          +  '<audio loop="true" src="ms-winsoundevent:Notification.Looping.Call3"/>'
          +   '<visual>'
          +    '<binding template="ToastText02">'
          +     '<text id="1">' + callerType.value + '</text>'
          +     '<text id="2">' + infoString + '</text>'
          +    '</binding>'
          +   '</visual>'
          +  '<commands scenario="incomingCall">'
          +   videoButtonXml
          +   voiceButtonXml
          +   '<command id="decline"/>'
          +  '</commands>'
          + '</toast>';
        
        // Display the generated XML for demonstration purposes.
        WinJS.log && WinJS.log(xmlPayload, "sample", "status");

        // Create an XML document from the XML.
        var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
        toastDOM.loadXml(xmlPayload);

        // Prepare to raise the toast.
        var notificationManager = Windows.UI.Notifications.ToastNotificationManager;
        var toastNotifier = notificationManager.createToastNotifier();

        if (delayedToast.value > 0) {
            // Schedule the toast in the future.
            var dueTime = new Date(Date.now() + delayedToast.value * 1000);
            var scheduledToast = new Windows.UI.Notifications.ScheduledToastNotification(toastDOM, dueTime);
            toastNotifier.addToSchedule(scheduledToast);
        } else {
            // Raise the toast immediately.
            var toast = new Windows.UI.Notifications.ToastNotification(toastDOM);
            toastNotifier.show(toast);
        }
    }
})();
