//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/globalization.html", {
        ready: function (element, options) {
            document.getElementById("viewCurrentResources").addEventListener("click", viewCurrentResources, false);
            document.getElementById("sendImageTileNotification").addEventListener("click", sendImageTileNotification, false);
            document.getElementById("sendTextTileNotification").addEventListener("click", sendTextTileNotification, false);
            document.getElementById("sendTileNotificationWithQueryStrings").addEventListener("click", sendTileNotificationWithQueryStrings, false);            
        }
    });

    function viewCurrentResources() {
        var context = Windows.ApplicationModel.Resources.Core.ResourceContext();
        var qualifierValues = context.qualifierValues;
        var scale = qualifierValues["scale"];
        var contrast = qualifierValues["contrast"];
        var ASLS = context.languages[0]; // the application specific language is always first in the list
        WinJS.log && WinJS.log("You system is currently set to the following values: Application Language: " + ASLS + ", Scale: " + scale + ", Contrast: " + contrast + ". If using web images and AddImageQuery, the following query string would be appened to the URL: ?ms-lang=" + ASLS + "&ms-scale=" + scale + "&ms-contrast=" + contrast, "sample", "status");
    }

    function sendTileNotificationWithQueryStrings() {
        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideImageAndText01();
        tileContent.textCaptionWrap.text = "This tile notification uses web images";
        tileContent.image.src = document.getElementById("imageSrcInput").value;
        tileContent.addImageQuery = true;        

        var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareImage();
        squareTileContent.image.src = document.getElementById("imageSrcInput").value;
        tileContent.squareContent = squareTileContent;

        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());

        WinJS.log && WinJS.log(tileContent.getContent(), "sample", "status");
    }

    function sendImageTileNotification() {
        var context = Windows.ApplicationModel.Resources.Core.ResourceContext();
        var qualifierValues = context.qualifierValues;
        var scale = qualifierValues["scale"];
        
        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideSmallImageAndText03();
        tileContent.textBodyWrap.text = "graySquare.png in the xml is actually graySquare.scale-" + scale + ".png"; 
        tileContent.image.src = "ms-appx:///images/graySquare.png";

        var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareImage(); 
        squareTileContent.image.src = "ms-appx:///images/graySquare.png";
        squareTileContent.squareContent = squareTileContent;
        tileContent.squareContent = squareTileContent;

        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());

        WinJS.log && WinJS.log(tileContent.getContent(), "sample", "status");
    }

    function sendTextTileNotification() {
        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideText03();

        // check out /en-US resources.resjson to understand where this string will come from
        tileContent.textHeadingWrap.text = "ms-resource:greeting";

        var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareText04();
        squareTileContent.textBodyWrap.text = "ms-resource:greeting";
        tileContent.squareContent = squareTileContent;

        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());

        WinJS.log && WinJS.log(tileContent.getContent(), "sample", "status");
    }
})();
