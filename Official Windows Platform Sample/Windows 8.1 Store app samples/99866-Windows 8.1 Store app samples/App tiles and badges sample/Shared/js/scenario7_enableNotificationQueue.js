//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario7_enableNotificationQueue.html", {
        ready: function (element, options) {
            document.getElementById("enableNotificationQueue").addEventListener("click", enableNotificationQueue, false);
            document.getElementById("disableNotificationQueue").addEventListener("click", disableNotificationQueue, false);
            document.getElementById("enableSquare150x150NotificationQueue").addEventListener("click", enableSquare150x150NotificationQueue, false);
            document.getElementById("disableSquare150x150NotificationQueue").addEventListener("click", disableSquare150x150NotificationQueue, false);
            document.getElementById("enableWide310x150NotificationQueue").addEventListener("click", enableWide310x150NotificationQueue, false);
            document.getElementById("disableWide310x150NotificationQueue").addEventListener("click", disableWide310x150NotificationQueue, false);
            document.getElementById("enableSquare310x310NotificationQueue").addEventListener("click", enableSquare310x310NotificationQueue, false);
            document.getElementById("disableSquare310x310NotificationQueue").addEventListener("click", disableSquare310x310NotificationQueue, false);
            document.getElementById("useTileNotificationTag").addEventListener("click", useTileNotificationTag, false);
            document.getElementById("clearTileNotification").addEventListener("click", clearTileNotification, false);
        }
    });

    function clearTileNotification() {
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
        WinJS.log && WinJS.log("Tile cleared", "sample", "status");
    }

    function enableNotificationQueue() {
        // Enable the notification queue - this only needs to be called once in the lifetime of your app
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueue(true);

        WinJS.log && WinJS.log("Tile notification cycling enabled for all tile sizes.", "sample", "status");
    }

    function disableNotificationQueue() {
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueue(false);
        WinJS.log && WinJS.log("Tile notification cycling disabled for all tile sizes.", "sample", "status");
    }

    function enableSquare150x150NotificationQueue()
    {
        // Enable the notification queue for Square150x150 Tiles.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueueForSquare150x150(true);

        WinJS.log && WinJS.log("Notification cycling enabled for Square150x150 Tiles.", "sample", "status");
    }

    function disableSquare150x150NotificationQueue()
    {
        // Disable the notification queue for Square150x150 Tiles.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueueForSquare150x150(false);

        WinJS.log && WinJS.log("Notification cycling disabled for Square150x150 Tiles.", "sample", "status");
    }

    function enableWide310x150NotificationQueue()
    {
        // Enable the notification queue for Wide310x150 Tiles.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueueForWide310x150(true);

        WinJS.log && WinJS.log("Notification cycling enabled for Wide310x150 Tiles.", "sample", "status");
    }

    function disableWide310x150NotificationQueue()
    {
        // Enable the notification queue for Wide310x150 Tiles.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueueForWide310x150(false);

        WinJS.log && WinJS.log("Notification cycling disabled for Wide310x150 Tiles.", "sample", "status");
    }

    function enableSquare310x310NotificationQueue()
    {
        // Enable the notification queue for Square310x310 Tiles.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueueForSquare310x310(true);

        WinJS.log && WinJS.log("Notification cycling enabled for Square310x310 Tiles.", "sample", "status");
    }

    function disableSquare310x310NotificationQueue()
    {
        // Disable the notification queue for Square310x310 Tiles.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueueForSquare310x310(false);

        WinJS.log && WinJS.log("Notification cycling disabled for Square310x310 Tiles.", "sample", "status");
    }

    function useTileNotificationTag() {
        var currentTime = new Date();
        var tag = document.getElementById("tag").value;
        if (!(tag.length > 0 && tag.length <= 16)) {
            tag = "TestTag01";
        }

        var tileText = document.getElementById("text").value;

        // Create a notification for the Square310x310 tile using one of the available templates for the size.
        var square310x310TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare310x310Text09();
        square310x310TileContent.textHeadingWrap.text = tileText;

        // Create a notification for the Wide310x150 tile using one of the available templates for the size.
        var wide310x150TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text03();
        wide310x150TileContent.textHeadingWrap.text = tileText;

        // Create a notification for the Square150x150 tile using one of the available templates for the size.
        var square150x150TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Text04();
        square150x150TileContent.textBodyWrap.text = tileText;

        // Attach the Square150x150 template to the Wide310x150 template.
        wide310x150TileContent.square150x150Content = square150x150TileContent;

        // Attach the Wide310x150 template to the Square310x310 template.
        square310x310TileContent.wide310x150Content = wide310x150TileContent;

        var tileNotification = square310x310TileContent.createNotification();

        // Set the tag on the notification.
        tileNotification.tag = tag;

        // Send the notification to the application’s tile.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        WinJS.log && WinJS.log("Tile notification sent. It is tagged with " + tag, "sample", "status");
    }
})();
