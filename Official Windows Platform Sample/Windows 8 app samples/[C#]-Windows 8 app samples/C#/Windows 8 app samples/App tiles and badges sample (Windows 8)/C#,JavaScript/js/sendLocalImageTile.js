//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/sendLocalImageTile.html", {
        ready: function (element, options) {
            document.getElementById("sendTileLocalImageNotification").addEventListener("click", sendTileLocalImageNotification, false);
            document.getElementById("sendTileLocalImageNotificationWithStringManipulation").addEventListener("click", sendTileLocalImageNotificationWithStringManipulation, false);
            document.getElementById("sendTileLocalImageNotificationWithXmlManipulation").addEventListener("click", sendTileLocalImageNotificationWithXmlManipulation, false);
            document.getElementById("clearTileNotification").addEventListener("click", clearTileNotification, false);
        }
    });

    function clearTileNotification() {
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
        WinJS.log && WinJS.log("Tile cleared", "sample", "status");
    }

    function sendTileLocalImageNotification() {
        // Note: This sample contains an additional project, NotificationsExtensions.
        // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
        // of the notification directly. See the additional function sendTileLocalImageNotificationWithXml to see how
        // to do it by modifying Xml directly, or sendLocalImageNotificationWithStringManipulation to see how to do it
        // by modifying strings directly

        // create the wide template
        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideImageAndText01();
        tileContent.textCaptionWrap.text = "This tile notification uses ms-appx images";
        tileContent.image.src = "ms-appx:///images/redWide.png";

        // Users can resize tiles to square or wide.
        // Apps can choose to include only square assets (meaning the app's tile can never be wide), or
        // include both wide and square assets (the user can resize the tile to square or wide).
        // Apps cannot include only wide assets.

        // Apps that support being wide should include square tile notifications since users
        // determine the size of the tile.

        // create the square template and attach it to the wide template
        var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareImage();
        squareTileContent.image.src = "ms-appx:///images/graySquare.png";
        tileContent.squareContent = squareTileContent;

        // send the notification
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());

        WinJS.log && WinJS.log(tileContent.getContent(), "sample", "status");
    }

    function sendTileLocalImageNotificationWithStringManipulation() {
        // create a string with the tile template xml
        var tileXmlString = "<tile>"
                              + "<visual>"
                              + "<binding template='TileWideImageAndText01'>"
                              + "<text id='1'>This tile notification uses ms-appx images</text>"
                              + "<image id='1' src='ms-appx:///images/redWide.png'/>"
                              + "</binding>"
                              + "<binding template='TileSquareImage'>"
                              + "<image id='1' src='ms-appx:///images/graySquare.png'/>"
                              + "</binding>"
                              + "</visual>"
                              + "</tile>";

        // create a DOM
        var tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
        // load the xml string into the DOM, catching any invalid xml characters 
        tileDOM.loadXml(tileXmlString);

        // create a tile notification
        var tile = new Windows.UI.Notifications.TileNotification(tileDOM);

        // send the notification to the app's application tile
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tile);

        WinJS.log && WinJS.log(tileDOM.getXml(), "sample", "status");
    }

    function sendTileLocalImageNotificationWithXmlManipulation() {
        // get a XML DOM version of a specific template by using getTemplateContent
        var tileXml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileWideImageAndText01);

        // get the text attributes for this template and fill them in
        var tileTextAttributes = tileXml.getElementsByTagName("text");
        tileTextAttributes[0].appendChild(tileXml.createTextNode("This tile notification uses ms-appx images"));

        // get the image attributes for this template and fill them in
        var tileImageAttributes = tileXml.getElementsByTagName("image");
        tileImageAttributes[0].setAttribute("src", "ms-appx:///images/redWide.png");

        // fill in a version of the square template returned by GetTemplateContent
        var squareTileXml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileSquareImage);
        var squareTileImageAttributes = squareTileXml.getElementsByTagName("image");
        squareTileImageAttributes[0].setAttribute("src", "ms-appx:///images/graySquare.png");

        // include the square template into the notification
        var node = tileXml.importNode(squareTileXml.getElementsByTagName("binding").item(0), true);
        tileXml.getElementsByTagName("visual").item(0).appendChild(node);

        // create the notification from the XML
        var tileNotification = new Windows.UI.Notifications.TileNotification(tileXml);

        // send the notification to the app's application tile
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        WinJS.log && WinJS.log(tileXml.getXml(), "sample", "status");
    }
})();
