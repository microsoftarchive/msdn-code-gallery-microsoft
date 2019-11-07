//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3_sendWebImageTile.html", {
        ready: function (element, options) {
            document.getElementById("sendTileWebImageNotification").addEventListener("click", sendTileWebNotification, false);
            document.getElementById("sendTileWebImageNotificationWithStringManipulation").addEventListener("click", sendTileWebImageNotificationWithStringManipulation, false);
            document.getElementById("sendTileWebImageNotificationWithXmlManipulation").addEventListener("click", sendTileWebImageNotificationWithXmlManipulation, false);
            document.getElementById("clearTileNotification").addEventListener("click", clearTileNotification, false);
        }
    });

    function clearTileNotification() {
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
        document.getElementById("notificationXmlContent").innerText = "";
        WinJS.log && WinJS.log("Tile cleared", "sample", "status");
    }

    function sendTileWebNotification() {
        // Note: This sample contains an additional project, NotificationsExtensions.
        // NotificationsExtensions exposes an object model for creating notifications, but you can also
        // modify the strings directly. See UpdateTileWithWebImageWithStringManipulation_Click for an example.

        // !Important!
        // The Internet (Client) capability must be checked in the manifest in the Capabilities tab
        // to display web images in tiles (either the http:// or https:// protocols)

        // Users can resize any app tile to the small (Square70x70 on Windows 8.1, Square71x71 on Windows Phone 8.1) and medium (Square150x150) tile sizes.
        // These are both tile sizes an app must minimally support.
        // An app can additionally support the wide (Wide310x150) tile size as well as the large (Square310x310) tile size.
        // Note that in order to support a large (Square310x310) tile size, an app must also support the wide (Wide310x150) tile size (but not vice versa).

        // This sample application supports all four tile sizes: small, medium, wide and large.
        // This means that the user may have resized their tile to any of these four sizes for their custom Start screen layout.
        // Because an app has no way of knowing what size the user resized their app tile to, an app should include template bindings
        // for each supported tile sizes in their notifications. Only Windows Phone 8.1 supports small tile notifications,
        // and there are no text templates available for this size.
        // We assemble one notification with four template bindings by including the content for each smaller
        // tile in the next size up. Square310x310 includes Wide310x150, which includes Square150x150, which includes Square71x71.
        // If we leave off the content for a tile size which the application supports, the user will not see the
        // notification if the tile is set to that size.

        // Create a notification for the Square310x310 tile using one of the available templates for the size.
        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare310x310Image();
        tileContent.addImageQuery = true;
        tileContent.image.src = document.getElementById("imageSrcInput").value;
        tileContent.image.alt = "Web Image";

        // Create a notification for the Wide310x150 tile using one of the available templates for the size.
        var wide310x150Content = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150ImageAndText01();
        wide310x150Content.textCaptionWrap.text = "This tile notification uses web images";
        wide310x150Content.image.src = document.getElementById("imageSrcInput").value;
        wide310x150Content.image.alt = "Web image";

        // Create a notification for the Square150x150 tile using one of the available templates for the size.
        var square150x150Content = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Image();
        square150x150Content.image.src = document.getElementById("imageSrcInput").value;
        square150x150Content.image.alt = "Web image";

        // Create a notification for the Square71x71 tile using one of the available templates for the size.
        var square71x71Content = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare71x71Image();
        square71x71Content.image.src = document.getElementById("imageSrcInput");
        square71x71Content.image.alt = "Web image";

        // Attach the Square71x71 template to the Square150x150 template.
        square150x150Content.square71x71Content = square71x71Content;
        
        // Attach the Square150x150 template to the Wide310x150 template.
        wide310x150Content.square150x150Content = square150x150Content;

        // Attach the Wide310x150 template to the Square310x310 template.
        tileContent.wide310x150Content = wide310x150Content;

        // Send the notification to the application’s tile.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());

        document.getElementById("notificationXmlContent").innerText = tileContent.getContent();
        WinJS.log && WinJS.log("Tile notification with web images sent", "sample", "status");
    }

    function sendTileWebImageNotificationWithStringManipulation() {
        // Create a string with the tile template xml.
        // Note that the version is set to "3" and that fallbacks are provided for the Square150x150 and Wide310x150 tile sizes.
        // This is so that the notification can be understood by Windows 8 and Windows 8.1 machines as well.
        var tileXmlString =
            "<tile>"
            + "<visual version='3' addImageQuery='true'>"
            + "<binding template='TileSquare71x71Image'>"
            + "<image id='1' src='" + document.getElementById("imageSrcInput").value + "' alt='Web image'/>"
            + "</binding>"
            + "<binding template='TileSquare150x150Image' fallback='TileSquareImage'>"
            + "<image id='1' src='" + document.getElementById("imageSrcInput").value + "' alt='Web image'/>"
            + "</binding>"
            + "<binding template='TileWide310x150ImageAndText01' fallback='TileWideImageAndText01'>"
            + "<text id='1'>This tile notification uses web images</text>"
            + "<image id='1' src='" + document.getElementById("imageSrcInput").value + "' alt='Web image'/>"
            + "</binding>"
            + "<binding template='TileSquare310x310Image'>"
            + "<image id='1' src='" + document.getElementById("imageSrcInput").value + "' alt='Web image'/>"
            + "</binding>"
            + "</visual>"
            + "</tile>";

        // Create a DOM.
        var tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
        try {
            // Load the xml string into the DOM, catching any invalid xml characters.
            tileDOM.loadXml(tileXmlString);

            // Create a tile notification.
            var tile = new Windows.UI.Notifications.TileNotification(tileDOM);

            // Send the notification to the application’s tile.
            Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tile);

            document.getElementById("notificationXmlContent").innerText = tileDOM.getXml();
            WinJS.log && WinJS.log("Tile notification with web images sent", "sample", "status");

        } catch (e) {
            document.getElementById("notificationXmlContent").innerText = "";
            WinJS.log && WinJS.log("Error loading the xml, check for invalid characters in the input", "sample", "error");
        }
    }

    function sendTileWebImageNotificationWithXmlManipulation() {
        // Get an XML DOM version of a Square71x71 template by using getTemplateContent.
        var square71x71Xml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileSquare71x71Image);
        // You will need to look at the template documentation to know how many image/text fields a particular template has.
        // Get the image attribute for this template and fill it in.
        var square71x71TileImageAttributes = square71x71Xml.getElementsByTagName("image");
        square71x71TileImageAttributes[0].setAttribute("src", document.getElementById("imageSrcInput").value);
        square71x71TileImageAttributes[0].setAttribute("alt", "Web image");

        // Get an XML DOM version of a Square150x150 template by using getTemplateContent.
        var square150x150Xml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileSquare150x150Image);
        var square150x150TileImageAttributes = square150x150Xml.getElementsByTagName("image");
        square150x150TileImageAttributes[0].setAttribute("src", document.getElementById("imageSrcInput").value);
        square150x150TileImageAttributes[0].setAttribute("alt", "Web image");

        // Get an XML DOM version of a Wide310x150 template by using getTemplateContent.
        var wide310x150Xml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileWide310x150ImageAndText01);
        var wide310x150TileTextAttributes = wide310x150Xml.getElementsByTagName("text");
        wide310x150TileTextAttributes[0].appendChild(wide310x150Xml.createTextNode("This tile notification uses web images"));
        var wide310x150TileImageAttributes = wide310x150Xml.getElementsByTagName("image");
        wide310x150TileImageAttributes[0].setAttribute("src", document.getElementById("imageSrcInput").value);
        wide310x150TileImageAttributes[0].setAttribute("alt", "Web image");

        // Get an XML DOM version of a Square310x310 template by using getTemplateContent.
        var square310x310Xml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileSquare310x310Image);
        square310x310Xml.getElementsByTagName("image")[0].setAttribute("src", document.getElementById("imageSrcInput").value);
        square310x310Xml.getElementsByTagName("image")[0].setAttribute("alt", "Web image");

        // Include the Square150x150 template into the Square71x71 notification.
        var node = square71x71Xml.importNode(square150x150Xml.getElementsByTagName("binding").item(0), true);
        square71x71Xml.getElementsByTagName("visual").item(0).appendChild(node);

        // Include the Wide310x150 template into the Square71x71 notification.
        node = square71x71Xml.importNode(wide310x150Xml.getElementsByTagName("binding").item(0), true);
        square71x71Xml.getElementsByTagName("visual").item(0).appendChild(node);

        // Include the Square310x310 template into the Square71x71 notification.
        node = square71x71Xml.importNode(square310x310Xml.getElementsByTagName("binding").item(0), true);
        square71x71Xml.getElementsByTagName("visual").item(0).appendChild(node);

        // Create a notification from the XML.
        var tileNotification = new Windows.UI.Notifications.TileNotification(square71x71Xml);

        // Send the notification to the application’s tile.
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        document.getElementById("notificationXmlContent").innerText = square71x71Xml.getXml();
        WinJS.log && WinJS.log("Tile notification with web images sent", "sample", "status");
    }
})();