//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/enableNotificationQueue.html", {
        ready: function (element, options) {
            document.getElementById("enableNotificationQueue").addEventListener("click", enableNotificationQueue, false);
            document.getElementById("disableNotificationQueue").addEventListener("click", disableNotificationQueue, false);
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

        WinJS.log && WinJS.log("Tile notification queue and cycling enabled", "sample", "status");
    }

    function disableNotificationQueue() {
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueue(false);
        WinJS.log && WinJS.log("Tile notification queue and cycling disabled", "sample", "status");
    }

    function useTileNotificationTag() {
        var currentTime = new Date();
        var tag = document.getElementById("tag").value;
        if (!(tag.length > 0 && tag.length <= 16)) {
            tag = "TestTag01";
        }

        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideText03();
        tileContent.textHeadingWrap.text = document.getElementById("text").value;

        var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareText04(); 
        squareTileContent.textBodyWrap.text = document.getElementById("text").value;
        tileContent.squareContent = squareTileContent;

        var tileNotification = tileContent.createNotification(); 

        // set the tag on the notification
        tileNotification.tag = tag;

        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        WinJS.log && WinJS.log("Tile notification sent. It is tagged with " + tag, "sample", "status");
    }
})();
