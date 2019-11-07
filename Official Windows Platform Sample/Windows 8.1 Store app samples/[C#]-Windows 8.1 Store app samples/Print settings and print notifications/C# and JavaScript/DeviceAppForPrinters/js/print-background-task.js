// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// A JavaScript background task runs a specified JavaScript file.
//
(function () {
    "use strict";

    //
    // Save the printer name and asyncUI xml
    //
    var keyPrinterName = "BA5857FA-DE2C-4A4A-BEF2-49D8B4130A39";
    var keyAsyncUIXML = "55DCA47A-BEE9-43EB-A7C8-92ECA2FA0685";
    var settings = Windows.Storage.ApplicationData.current.localSettings;

    //
    // The background task instance's activation parameters are available via Windows.UI.WebUI.WebUIBackgroundTaskInstance.current
    //
    var backgroundTaskInstance = Windows.UI.WebUI.WebUIBackgroundTaskInstance.current;

    var details = backgroundTaskInstance.triggerDetails;
    settings.values[keyPrinterName] = details.printerName;
    settings.values[keyAsyncUIXML] = details.eventData;
    
    // With the print notification event details we can choose to show a toast, update the tile, or update the badge.
    // It is not recommended to always show a toast, especially for non-actionable events, as it may become annoying for most users.
    // User may even just turn off all toasts from this app, which is not a desired outcome.
    // For events that does not require user's immediate attention, it is recommended to update the tile/badge and not show a toast.
    showToast(details.printerName, details.eventData);
    updateTile(details.printerName, details.eventData);
    updateBadge();

    //
    // A JavaScript background task must call close when it is done.
    //
    close();

    function updateTile(printerName, bidiMessage) {
        var tileUpdater = Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication();
        tileUpdater.clear();

        var tileXml = Windows.UI.Notifications.TileUpdateManager.getTemplateContent(Windows.UI.Notifications.TileTemplateType.tileWide310x150Text09);
        var tileTextAttributes = tileXml.getElementsByTagName("text");
        tileTextAttributes[0].innerText = printerName;
        tileTextAttributes[1].innerText = bidiMessage;

        var tileNotification = new Windows.UI.Notifications.TileNotification(tileXml);
        tileNotification.Tag = "tag01";
        tileUpdater.update(tileNotification);
    }

    function updateBadge()
    {
        var badgeXml = Windows.UI.Notifications.BadgeUpdateManager.getTemplateContent(Windows.UI.Notifications.BadgeTemplateType.badgeGlyph);
        var badgeElement = badgeXml.selectSingleNode("/badge");
        badgeElement.setAttribute("value", "error");

        var badgeNotification = new Windows.UI.Notifications.BadgeNotification(badgeXml);
        Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().update(badgeNotification);
    }

    function showToast(title, body) {
        var notifications = Windows.UI.Notifications;
        var toastNotificationManager = Windows.UI.Notifications.ToastNotificationManager;
        var toastXml = toastNotificationManager.getTemplateContent(notifications.ToastTemplateType.toastText02);

        var temp = "\"Printer Name\"" + ":" + "\"" + title + "\"" + "," + "\"AsyncUI XML\"" + ":" + "\"" + body + "\"";
        if (temp.length > 251) {
            temp = temp.substring(0, 251);
        }
        toastXml.selectSingleNode("/toast").setAttribute("launch", "'{" + temp + "}'");

        var textNodes = toastXml.getElementsByTagName("text");
        textNodes[0].appendChild(toastXml.createTextNode(title));
        textNodes[1].appendChild(toastXml.createTextNode(body));

        var toast = new notifications.ToastNotification(toastXml);
        toastNotificationManager.createToastNotifier().show(toast);
    }
})();
