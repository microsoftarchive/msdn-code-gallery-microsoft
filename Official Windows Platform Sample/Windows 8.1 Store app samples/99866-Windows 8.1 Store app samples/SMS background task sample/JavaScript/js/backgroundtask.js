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

    //
    // The background task instance's activation parameters are available via Windows.UI.WebUI.WebUIBackgroundTaskInstance.current
    //
    var cancel = false;
    var backgroundTaskInstance = Windows.UI.WebUI.WebUIBackgroundTaskInstance.current;
    var settings = Windows.Storage.ApplicationData.current.localSettings;
    var key = null;
    var smsDevice = null;
    var notifications = Windows.UI.Notifications;

    var smsNotificationDetails = backgroundTaskInstance.triggerDetails;

    console.log("Background " + backgroundTaskInstance.task.name + " Starting...");

    //
    // Associate a cancellation handler with the background task.
    //
    function onCanceled(cancelSender, cancelReason) {
        cancel = true;
    }
    backgroundTaskInstance.addEventListener("canceled", onCanceled);

    //
    // This function is called if any error occurs
    //
    function errorCallback(error) {

        //
        // If we hit any error we stop the background task.
        //

        console.log("Error in background task " + error.name + " : " + error.description);

        backgroundTaskInstance.succeeded = false;

        key = backgroundTaskInstance.task.taskId.toString();
        settings.values[key] = "Canceled";

        close();
    }

    //
    // Function to show toast pop up
    //
    function showSmsToast(strFrom, strBody) {

        //
        // Get the toast manager.
        //
        var notificationManager = notifications.ToastNotificationManager;

        var toastXml = notificationManager.getTemplateContent(notifications.ToastTemplateType.toastText02);

        //
        // You can use the methods from the XML document to specify
        // all the required parameters for the toast.
        //
        var textNodes = toastXml.getElementsByTagName("text");
        textNodes.item(0).appendChild(toastXml.createTextNode(strFrom));
        textNodes.item(1).appendChild(toastXml.createTextNode(strBody));

        //
        // Create a toast from the XML, then create a ToastNotifier object
        // to send the toast.
        //
        var toast = new notifications.ToastNotification(toastXml);

        notificationManager.createToastNotifier().show(toast);
    }

    //
    // Function to read sms
    //
    function readSMS() {
        var textMessage = new Windows.Devices.Sms.SmsTextMessage.fromBinaryMessage(smsNotificationDetails.binaryMessage);

        showSmsToast(textMessage.from, textMessage.body);
        //
        // Use the succeeded property to indicate that this background task completed successfully.
        //
        backgroundTaskInstance.succeeded = true;
        backgroundTaskInstance.progress = 100;

        console.log("Background " + backgroundTaskInstance.task.name + " Completed");

        //
        // Write to localSettings to indicate that this background task completed.
        //
        key = backgroundTaskInstance.task.taskId.toString();
        settings.values[key] = "Completed";

        //
        // A JavaScript background task must call close when it is done.
        //
        close();
    }
 
    readSMS();

})();

