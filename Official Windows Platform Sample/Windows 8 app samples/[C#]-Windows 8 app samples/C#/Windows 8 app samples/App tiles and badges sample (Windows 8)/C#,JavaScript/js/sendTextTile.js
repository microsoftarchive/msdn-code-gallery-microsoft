//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/sendTextTile.html", {
        ready: function (element, options) {
            document.getElementById("sendTileTextNotification").addEventListener("click", sendTileTextNotification, false);
            document.getElementById("sendTileTextNotificationWithStringManipulation").addEventListener("click", sendTileTextNotificationWithStringManipulation, false);
            document.getElementById("sendTileTextNotificationWithXmlManipulation").addEventListener("click", sendTileTextNotificationWithXmlManipulation, false);
            document.getElementById("clearTileNotification").addEventListener("click", clearTileNotification, false);
        }
    });

    function clearTileNotification() {
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
        WinJS.log && WinJS.log("Tile cleared", "sample", "status");
    }

    function sendTileTextNotification() {
        // Note: This sample contains an additional project, NotificationsExtensions.
        // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
        // of the notification directly. See the additional function sendTileTextNotificationWithXmlManipulation to see how
        // to do it by modifying Xml directly, or sendTileTextNotificationWithStringManipulation to see how to do it
        // by modifying strings directly

        // create the wide template
        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideText03();
        tileContent.textHeadingWrap.text = "Hello World! My very own tile notification";

        // Users can resize tiles to square or wide.
        // Apps can choose to include only square assets (meaning the app's tile can never be wide), or
        // include both wide and square assets (the user can resize the tile to square or wide).
        // Apps cannot include only wide assets.

        // Apps that support being wide should include square tile notifications since users
        // determine the size of the tile.

        // create the square template and attach it to the wide template
        var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareText04();
        squareTileContent.textBodyWrap.text = "Hello World! My very own tile notification";
        tileContent.squareContent = squareTileContent;

        // send the notification
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());

        WinJS.log && WinJS.log(tileContent.getContent(), "sample", "status");
    }

    function sendTileTextNotificationWithStringManipulation() {
        // create a string with the tile template xml
        var tileXmlString = "<tile>"
                              + "<visual>"
                              + "<binding template='TileWideText03'>"
                              + "<text id='1'>Hello World! My very own tile notification</text>"
                              + "</binding>"
                              + "<binding template='TileSquareText04'>"
                              + "<text id='1'>Hello World! My very own tile notification</text>"
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

    function sendTileTextNotificationWithXmlManipulation() {
        // get a XML DOM version of a specific template by using getTemplateContent
        var tileXml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileWideText03);

        // You will need to look at the template documentation to know how many text fields a particular template has
        // get the text attributes for this template and fill them in
        var tileAttributes = tileXml.getElementsByTagName("text");
        tileAttributes[0].appendChild(tileXml.createTextNode("Hello World! My very own tile notification"));

        // fill in a version of the square template returned by GetTemplateContent
        var squareTileXml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileSquareText04);
        var squareTileTextAttributes = squareTileXml.getElementsByTagName("text");
        squareTileTextAttributes[0].appendChild(squareTileXml.createTextNode("Hello World! My very own tile notification"));

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
