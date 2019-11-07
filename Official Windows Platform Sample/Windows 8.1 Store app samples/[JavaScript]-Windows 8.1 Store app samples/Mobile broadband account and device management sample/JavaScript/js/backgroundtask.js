// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// A JavaScript background task runs a specified JavaScript file.
//
(function () {
    "use strict";

    // The background task instance's activation parameters are
    // available via Windows.UI.WebUI.WebUIBackgroundTaskInstance.current.
    var cancel = false;
    var backgroundTaskInstance = Windows.UI.WebUI.WebUIBackgroundTaskInstance.current;
    var settings = Windows.Storage.ApplicationData.current.localSettings;
    var key = null;
    var notifications = Windows.UI.Notifications;

    var networkOperatorNotificationDetails = backgroundTaskInstance.triggerDetails;

    console.log("Background " + backgroundTaskInstance.task.name + " Starting...");

    // Associate a cancellation handler with the background task.
    function onCanceled(cancelSender, cancelReason) {
        cancel = true;
        console.log("Background " + backgroundTaskInstance.task.name + " canceled");
    }
    backgroundTaskInstance.addEventListener("canceled", onCanceled);

    // In this example, we will show a toast notification for the
    // operator message.
    function showOperatorNotificatonToast(message) {

        var notificationManager = notifications.ToastNotificationManager;

        var toastXml = notificationManager.getTemplateContent(notifications.ToastTemplateType.toastText02);
        var textNodes = toastXml.getElementsByTagName("text");
        textNodes.item(0).appendChild(toastXml.createTextNode("Mobile Broadband Message:"));
        textNodes.item(1).appendChild(toastXml.createTextNode(message));

        // Create a toast from the XML, then create a ToastNotifier object
        // to send the toast.
        var toast = new notifications.ToastNotification(toastXml);

        notificationManager.createToastNotifier().show(toast);
    }

    // This sample only handles notification types that typically have message content.
    // The message is displayed in a toast.
    if ((networkOperatorNotificationDetails.notificationType === Windows.Networking.NetworkOperators.NetworkOperatorEventMessageType.gsm) ||
        (networkOperatorNotificationDetails.notificationType === Windows.Networking.NetworkOperators.NetworkOperatorEventMessageType.cdma) ||
        (networkOperatorNotificationDetails.notificationType === Windows.Networking.NetworkOperators.NetworkOperatorEventMessageType.ussd) )
    {
        showOperatorNotificatonToast(networkOperatorNotificationDetails.message);
    }
    else if (networkOperatorNotificationDetails.notificationType === Windows.Networking.NetworkOperators.NetworkOperatorEventMessageType.tetheringNumberOfClientsChanged)
    {
        showOperatorNotificatonToast("tetheringNumberOfClientsChanged");
    }
    else if (networkOperatorNotificationDetails.notificationType === Windows.Networking.NetworkOperators.NetworkOperatorEventMessageType.tetheringOperationalStateChanged) {
        showOperatorNotificatonToast("tetheringOperationalStateChanged");
    }

    // Use the succeeded property to indicate that this background task completed successfully.
    backgroundTaskInstance.succeeded = true;
    backgroundTaskInstance.progress = 100;

    // Write to localSettings to indicate that this background task completed.
    key = backgroundTaskInstance.task.taskId.toString();
    settings.values[key] = "Completed";

    // Putting notification type
    settings.values[key+"_type"] = networkOperatorNotificationDetails.notificationType;

    console.log("Background " + backgroundTaskInstance.task.name + " finished");

    close();
})();
