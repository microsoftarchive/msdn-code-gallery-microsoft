//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("scheduleNotificationButton").addEventListener("click", scheduleNotification, false);
            document.getElementById("scheduleNotificationButtonString").addEventListener("click", scheduleNotification, false);
        }
    });

    var Notifications = Windows.UI.Notifications;
    var ToastContent = NotificationsExtensions.ToastContent;
    var TileContent = NotificationsExtensions.TileContent;

    function scheduleNotification(e) {
        var targetButton = e.currentTarget;
        var useStrings = false;
        if (targetButton.id === "scheduleNotificationButtonString") {
            useStrings = true;
        }

        var dueTimeInSeconds = parseInt(document.getElementById("futureTimeBox").value);
        if (dueTimeInSeconds && dueTimeInSeconds > 0) {
            var updateString = document.getElementById("notificationString").value;

            // Use a Javascript Date object to specify the time the toast should be delivered.
            var currentTime = new Date();
            var dueTime = new Date(currentTime.getTime() + dueTimeInSeconds * 1000);
            var idNumber = Math.floor(Math.random() * 100000000);

            if (document.getElementById("toastRadio").checked) {
                if (useStrings) {
                    scheduleToastWithStringManipulation(updateString, dueTime, idNumber);
                } else {
                    scheduleToast(updateString, dueTime, idNumber);
                }
            } else {

                if (useStrings) {
                    scheduleTileWithStringManipulation(updateString, dueTime, idNumber);
                } else {
                    scheduleTile(updateString, dueTime, idNumber);
                }
            }
        } else {
            WinJS.log && WinJS.log("You must input a valid time in seconds.", "sample", "error");
        }
    }

    function scheduleToast(updateString, dueTime, idNumber) {
        // Scheduled toasts use the same toast templates as all other kinds of toasts.
        var toastContent = ToastContent.ToastContentFactory.createToastText02();
        toastContent.textHeading.text = updateString;
        toastContent.textBodyWrap.text = "Received: " + dueTime.toLocaleTimeString();

        var toast;
        if (document.getElementById("repeatBox").checked) {
            // In this sample, repeating toasts will default to repeating five times, each time 60 seconds apart
            toast = new Notifications.ScheduledToastNotification(toastContent.getXml(), dueTime, 60 * 1000, 5);

            // You can specify an ID so that you can manage toasts later.
            // Make sure the ID is 15 characters or less.
            toast.id = "Repeat" + idNumber;
        } else {
            toast = new Notifications.ScheduledToastNotification(toastContent.getXml(), dueTime);
            toast.id = "Toast" + idNumber;
        }

        Notifications.ToastNotificationManager.createToastNotifier().addToSchedule(toast);
        WinJS.log && WinJS.log("Scheduled a toast with ID: " + toast.id, "sample", "status");
    }

    function scheduleTile(updateString, dueTime, idNumber) {
        var tileContent = TileContent.TileContentFactory.createTileWide310x150Text09();
        tileContent.textHeading.text = updateString;
        tileContent.textBodyWrap.text = "Received: " + dueTime.toLocaleTimeString();

        // Include square tile in notification
        var squareContent = TileContent.TileContentFactory.createTileSquare150x150Text04();
        squareContent.textBodyWrap.text = updateString;

        tileContent.square150x150Content = squareContent;

        // Create the notification object
        var futureTile = new Notifications.ScheduledTileNotification(tileContent.getXml(), dueTime);
        futureTile.id = "Tile" + idNumber;

        // Add to schedule
        // You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
        // See "Tiles" sample for more details
        Notifications.TileUpdateManager.createTileUpdaterForApplication().addToSchedule(futureTile);

        WinJS.log && WinJS.log("Scheduled a tile with ID: " + futureTile.id, "sample", "status");
    }

    // Creating a tile with a string and loading it into a DOM
    function scheduleTileWithStringManipulation(updateString, dueTime, idNumber) {
        var tileXmlString = "<tile>"
                         + "<visual version='2'>"
                         + "<binding template='TileWide310x150Text09' fallback='TileWideText09'>"
                         + "<text id='1'>" + updateString + "</text>"
                         + "<text id='2'>" + "Received: " + dueTime.toLocaleTimeString() + "</text>"
                         + "</binding>"
                         + "<binding template='TileSquare150x150Text04' fallback='TileSquareText04'>"
                         + "<text id='1'>" + updateString + "</text>"
                         + "</binding>"
                         + "</visual>"
                         + "</tile>";

        var tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
        try {
            tileDOM.loadXml(tileXmlString);

            // Create the notification object
            var futureTile = new Notifications.ScheduledTileNotification(tileDOM, dueTime);
            futureTile.id = "Tile" + idNumber;

            // Add to schedule
            // You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
            // See "Tiles" sample for more details
            Notifications.TileUpdateManager.createTileUpdaterForApplication().addToSchedule(futureTile);

            WinJS.log && WinJS.log("Scheduled a tile with ID: " + futureTile.id, "sample", "status");
        } catch (e) {
            WinJS.log && WinJS.log("Error loading the xml, check for invalid characters in the input", "sample", "error");
        }
    }

    // Creating a toast with a string and loading it into a DOM
    function scheduleToastWithStringManipulation(updateString, dueTime, idNumber) {
        // Scheduled toasts use the same toast templates as all other kinds of toasts.
        var toastXmlString = "<toast>"
            + "<visual version='2'>"
            + "<binding template='ToastText02'>"
            + "<text id='2'>" + updateString + "</text>"
            + "<text id='1'>" + "Received: " + dueTime.toLocaleTimeString() + "</text>"
            + "</binding>"
            + "</visual>"
            + "</toast>";

        toastXmlString = toastXmlString.replace("updateString", updateString);

        var toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
        try {
            toastDOM.loadXml(toastXmlString);
            var toast;
            if (document.getElementById("repeatBox").checked) {
                // In this sample, repeating toasts will default to repeating five times, each time 60 seconds apart
                toast = new Notifications.ScheduledToastNotification(toastDOM, dueTime, 60 * 1000, 5);

                // You can specify an ID so that you can manage toasts later.
                // Make sure the ID is 15 characters or less.
                toast.id = "Repeat" + idNumber;
            } else {
                toast = new Notifications.ScheduledToastNotification(toastDOM, dueTime);
                toast.id = "Toast" + idNumber;
            }

            Notifications.ToastNotificationManager.createToastNotifier().addToSchedule(toast);
            WinJS.log && WinJS.log("Scheduled a toast with ID: " + toast.id, "sample", "status");
        } catch (e) {
            WinJS.log && WinJS.log("Error loading the xml, check for invalid characters in the input", "sample", "error");
        }
    }
})();