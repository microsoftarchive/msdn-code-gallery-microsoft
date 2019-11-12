//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_sendTextTile.html", {
        ready: function (element, options) {
            document.getElementById("sendTileTextNotification").addEventListener("click", sendTileTextNotification, false);
            document.getElementById("sendTileTextNotificationWithStringManipulation").addEventListener("click", sendTileTextNotificationWithStringManipulation, false);
            document.getElementById("sendTileTextNotificationWithXmlManipulation").addEventListener("click", sendTileTextNotificationWithXmlManipulation, false);
            document.getElementById("clearTileNotification").addEventListener("click", clearTileNotification, false);
        }
    });

    function clearTileNotification() {
        // TileUpdater is also used to clear the tile.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
        document.getElementById("notificationXmlContent").innerText = "";
        WinJS.log && WinJS.log("Tile cleared", "sample", "status");
    }

    function sendTileTextNotification() {
        // Note: This sample contains an additional project, NotificationsExtensions.
        // NotificationsExtensions exposes an object model for creating notifications, but you can also
        // modify the strings directly. See UpdateTileWithTextWithStringManipulation_Click for an example.

        // Users can resize any app tile to the small (Square70x70 on Windows 8.1, Square71x71 on Windows Phone 8.1) and medium (Square150x150) tile sizes.
        // These are both tile sizes an app must minimally support.
        // An app can additionally support the wide (Wide310x150) tile size as well as the large (Square310x310) tile size.
        // Note that in order to support a large (Square310x310) tile size, an app must also support the wide (Wide310x150) tile size (but not vice versa).

        // This sample application supports all four tile sizes: small, medium, wide and large.
        // This means that the user may have resized their tile to any of these four sizes for their custom Start screen layout.
        // Because an app has no way of knowing what size the user resized their app tile to, an app should include template bindings
        // for each supported tile sizes in their notifications. Only Windows Phone 8.1 supports small tile notifications,
        // and there are no text templates available for this size.
        // We assemble one notification with three template bindings by including the content for each smaller
        // tile in the next size up. Square310x310 includes Wide310x150, which includes Square150x150.
        // If we leave off the content for a tile size which the application supports, the user will not see the
        // notification if the tile is set to that size.

        // Create a notification for the Square310x310 tile using one of the available templates for the size.
        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare310x310Text09();
        tileContent.textHeadingWrap.text = "Hello World! My very own tile notification";

        // Create a notification for the Wide310x150 tile using one of the available templates for the size.
        var wide310x150Content = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text03();
        wide310x150Content.textHeadingWrap.text = "Hello World! My very own tile notification";

        // Create a notification for the Square150x150 tile using one of the available templates for the size.
        var square150x150Content = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Text04();
        square150x150Content.textBodyWrap.text = "Hello World! My very own tile notification";

        // Attach the Square150x150 template to the Wide310x150 template.
        wide310x150Content.square150x150Content = square150x150Content;

        // Attach the Wide310x150 template to the Square310x310 template.
        tileContent.wide310x150Content = wide310x150Content;

        // Send the notification to the application’s tile.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());

        document.getElementById("notificationXmlContent").innerText = tileContent.getContent();
        WinJS.log && WinJS.log("Tile notification with text sent", "sample", "status");
    }

    function sendTileTextNotificationWithStringManipulation() {
        // Create a string with the tile template xml.
        // Note that the version is set to "3" and that fallbacks are provided for the Square150x150 and Wide310x150 tile sizes.
        // This is so that the notification can be understood by Windows 8 and Windows 8.1 machines as well.
        var tileXmlString =
            "<tile>"
            + "<visual version='3'>"
            + "<binding template='TileSquare150x150Text04' fallback='TileSquareText04'>"
            + "<text id='1'>Hello World! My very own tile notification</text>"
            + "</binding>"
            + "<binding template='TileWide310x150Text03' fallback='TileWideText03'>"
            + "<text id='1'>Hello World! My very own tile notification</text>"
            + "</binding>"
            + "<binding template='TileSquare310x310Text09'>"
            + "<text id='1'>Hello World! My very own tile notification</text>"
            + "</binding>"
            + "</visual>"
            + "</tile>";

        // Create a DOM.
        var tileDOM = new Windows.Data.Xml.Dom.XmlDocument();

        // Load the xml string into the DOM, catching any invalid xml characters.
        tileDOM.loadXml(tileXmlString);

        // Create a tile notification.
        var tile = new Windows.UI.Notifications.TileNotification(tileDOM);

        // Send the notification to the application’s tile.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tile);

        document.getElementById("notificationXmlContent").innerText = tileDOM.getXml();
        WinJS.log && WinJS.log("Tile notification with text sent", "sample", "status");
    }

    function sendTileTextNotificationWithXmlManipulation() {
        // Get an XML DOM version of a Square150x150 template by using getTemplateContent.
        var square150x150Xml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileSquare150x150Text04);
        // You will need to look at the template documentation to know how many text fields a particular template has.
        // Get the text attributes for this template and fill them in.
        var squareTileTextAttributes = square150x150Xml.getElementsByTagName("text");
        squareTileTextAttributes[0].appendChild(square150x150Xml.createTextNode("Hello World! My very own tile notification"));

        // Get an XML DOM version of a Wide310x150 template by using getTemplateContent.
        var wide310x150Xml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileWide310x150Text03);
        var tileTextAttributes = wide310x150Xml.getElementsByTagName("text");
        tileTextAttributes[0].appendChild(wide310x150Xml.createTextNode("Hello World! My very own tile notification"));

        // Get an XML DOM version of a Square310x310 template by using getTemplateContent.
        var square310x310Xml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileSquare310x310Text09);
        square310x310Xml.getElementsByTagName("text")[0].setAttribute("id", "1");
        square310x310Xml.getElementsByTagName("text")[0].appendChild(square310x310Xml.createTextNode("Hello World! My very own tile notification"));

        // Include the Wide310x150 template into the Square150x150 notification.
        node = square150x150Xml.importNode(wide310x150Xml.getElementsByTagName("binding").item(0), true);
        square150x150Xml.getElementsByTagName("visual").item(0).appendChild(node);

        // Include the Square310x310 template into the Square150x150 notification.
        var node = square150x150Xml.importNode(square310x310Xml.getElementsByTagName("binding").item(0), true);
        square150x150Xml.getElementsByTagName("visual").item(0).appendChild(node);

        // Create a notification from the XML.
        var tileNotification = new Windows.UI.Notifications.TileNotification(square150x150Xml);

        // Send the notification to the application’s tile.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        document.getElementById("notificationXmlContent").innerText = square150x150Xml.getXml();
        WinJS.log && WinJS.log("Tile notification with text sent", "sample", "status");
    }
})();