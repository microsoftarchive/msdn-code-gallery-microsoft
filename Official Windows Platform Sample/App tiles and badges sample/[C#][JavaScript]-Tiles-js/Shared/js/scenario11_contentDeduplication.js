//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario11_contentDeduplication.html", {
        ready: function (element, options) {
            document.getElementById("enableNotificationQueue").addEventListener("click", enableNotificationQueue, false);
            document.getElementById("sendNotifications").addEventListener("click", sendNotifications, false);
            document.getElementById("clearTileNotification").addEventListener("click", clearTileNotification, false);
        }
    });

    function clearTileNotification() {
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
        WinJS.log && WinJS.log("Tile cleared.", "sample", "status");
    }

    function enableNotificationQueue() {
        // Enable the notification queue - this only needs to be called once in the lifetime of your app
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueue(true);

        WinJS.log && WinJS.log("Tile notification cycling enabled for all tile sizes.", "sample", "status");
    }

    function sendNotifications() {
        // Create a notification for the first set of stories with bindings for all 3 tile sizes
        var square310x310TileContent1 = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare310x310Text09();
        square310x310TileContent1.textHeadingWrap.text = "Main Story";

        var wide310x150TileContent1 = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text03();
        wide310x150TileContent1.textHeadingWrap.text = "Main Story";

        var square150x150TileContent1 = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Text04();
        square150x150TileContent1.textBodyWrap.text = "Main Story";

        wide310x150TileContent1.square150x150Content = square150x150TileContent1;
        square310x310TileContent1.wide310x150Content = wide310x150TileContent1;

        // Set the contentId on the Square310x310 tile
        square310x310TileContent1.contentId = "Main_1";

        // Tag the notification and send it to the tile
        var tileNotification = square310x310TileContent1.createNotification();
        tileNotification.tag = "1";
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        // Create the first notification for the second set of stories with binding for all 3 tiles sizes
        var square310x310TileContent2 = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare310x310TextList03();
        square310x310TileContent2.textHeading1.text = "Additional Story 1";
        square310x310TileContent2.textHeading2.text = "Additional Story 2";
        square310x310TileContent2.textHeading3.text = "Additional Story 3";

        var wide310x150TileContent2 = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text03();
        wide310x150TileContent2.textHeadingWrap.text = "Additional Story 1";

        var square150x150TileContent2 = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Text04();
        square150x150TileContent2.textBodyWrap.text = "Additional Story 1";

        wide310x150TileContent2.square150x150Content = square150x150TileContent2;
        square310x310TileContent2.wide310x150Content = wide310x150TileContent2;

        // Set the contentId on the Square310x310 tile
        square310x310TileContent2.ContentId = "Additional_1";

        // Tag the notification and send it to the tile
        tileNotification = square310x310TileContent2.createNotification();
        tileNotification.tag = "2";
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        // Create the second notification for the second set of stories with binding for all 3 tiles sizes
        // Notice that we only replace the Wide310x150 and Square150x150 binding elements,
        // and keep the Square310x310 content the same - this will cause the Square310x310 to be ignored for this notification,
        // since the contentId for this size is the same as in the first notification of the second set of stories.
        var wide310x150TileContent3 = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text03();
        wide310x150TileContent3.textHeadingWrap.text = "Additional Story 2";

        var square150x150TileContent3 = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Text04();
        square150x150TileContent3.textBodyWrap.text = "Additional Story 2";

        wide310x150TileContent3.square150x150Content = square150x150TileContent3;
        square310x310TileContent2.wide310x150Content = wide310x150TileContent3;

        // Tag the notification and send it to the tile
        tileNotification = square310x310TileContent2.createNotification();
        tileNotification.tag = "3";
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        // Create the third notification for the second set of stories with binding for all 3 tiles sizes
        // Notice that we only replace the Wide310x150 and Square150x150 binding elements,
        // and keep the Square310x310 content the same again - this will cause the Square310x310 to be ignored for this notification,
        // since the contentId for this size is the same as in the first notification of the second set of stories.
        var wide310x150TileContent4 = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text03();
        wide310x150TileContent4.textHeadingWrap.text = "Additional Story 3";

        var square150x150TileContent4 = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Text04();
        square150x150TileContent4.textBodyWrap.text = "Additional Story 3";

        wide310x150TileContent4.square150x150Content = square150x150TileContent4;
        square310x310TileContent2.wide310x150Content = wide310x150TileContent4;

        // Tag the notification and send it to the tile
        tileNotification = square310x310TileContent2.createNotification();
        tileNotification.tag = "4";
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        WinJS.log && WinJS.log("Four notifications sent.", "sample", "status");
    }
})();
