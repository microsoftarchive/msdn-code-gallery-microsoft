//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/notificationExpiration.html", {
        ready: function (element, options) {
            document.getElementById("useTileNotificationExpiration").addEventListener("click", useTileNotificationExpiration, false);
        }
    });

    function useTileNotificationExpiration() {
        var currentTime = new Date();
        var seconds = document.getElementById("seconds").value;
        var numericExpression = /^[0-9]+$/;
        if (!(seconds.length > 0 && seconds.length <= 3 && seconds.match(numericExpression))) {
            seconds = 10; 
        }

        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideText04();
        tileContent.textBodyWrap.text = "This notification will expire at " + new Date(currentTime.getTime() + seconds * 1000);
        tileContent.branding = NotificationsExtensions.TileContent.TileBranding.none;

        var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareText04();
        squareTileContent.textBodyWrap.text = "This notification will expire at " + new Date(currentTime.getTime() + seconds * 1000);
        squareTileContent.branding = NotificationsExtensions.TileContent.TileBranding.none;
        tileContent.squareContent = squareTileContent;

        var tileNotification = tileContent.createNotification();

        var expiryTime = new Date(currentTime.getTime() + seconds * 1000);

        // set the expiration time on the notification
        tileNotification.expirationTime = expiryTime;
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        WinJS.log && WinJS.log("Tile notification sent. It will expire at " + expiryTime, "sample", "status");
    }
})();
