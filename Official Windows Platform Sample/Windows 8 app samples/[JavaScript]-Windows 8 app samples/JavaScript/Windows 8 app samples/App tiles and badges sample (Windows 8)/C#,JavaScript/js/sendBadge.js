//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/sendBadge.html", {
        ready: function (element, options) {
            document.getElementById("sendBadgeNotification").addEventListener("click", sendBadgeNotification, false);
            document.getElementById("sendBadgeNotificationWithStringManipulation").addEventListener("click", sendBadgeNotificationWithStringManipulation, false);
            document.getElementById("clearBadgeNotification").addEventListener("click", clearBadgeNotification, false);
            document.getElementById("badgeTypeSelector").addEventListener("change", badgeTypeChanged, false);

        }
    });

    function clearBadgeNotification() {
        Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().clear();
        WinJS.log && WinJS.log("Badge cleared", "sample", "status");
    }

    function sendBadgeNotification() {
        var badgeType = document.getElementById("badgeTypeSelector").options[document.getElementById("badgeTypeSelector").selectedIndex].value;
        var badgeContent;
        var badgeAttributes;

        // Note: This sample contains an additional project, NotificationsExtensions.
        // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
        // of the notification directly. See the additional function sendBadgeNotificationWithStringManipulation to see how to do it
        // by modifying strings directly

        // sending a badge notification with a number
        if (badgeType === "Number") {
            var numberInput = document.getElementById("badgeNumberInput").value;
            badgeContent = new NotificationsExtensions.BadgeContent.BadgeNumericNotificationContent(numberInput);
            Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().update(badgeContent.createNotification());
        } else { //sending a badge notification with a glyph, not a number
            var glyph = document.getElementById("badgeGlyphTypes").selectedIndex;
            // note: usually this would be created with NotificationsExtensions.BadgeContent.GlyphValue.alert or any of the values of GlyphValue
            badgeContent = new NotificationsExtensions.BadgeContent.BadgeGlyphNotificationContent(glyph);

            Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().update(badgeContent.createNotification());
        }

        // send the notification to the app's application tile
        Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().update(badgeContent.createNotification());
        WinJS.log && WinJS.log(badgeContent.getContent(), "sample", "status");

    }

    function sendBadgeNotificationWithStringManipulation() {
        var badgeType = document.getElementById("badgeTypeSelector").options[document.getElementById("badgeTypeSelector").selectedIndex].value;
        var badgeXmlString;

        // sending a badge notification with a number
        if (badgeType === "Number") {
            var numberInput = document.getElementById("badgeNumberInput").value;

            // create a string with the badge template xml
            badgeXmlString = "<badge value='" + numberInput + "'/>";
        } else { //sending a badge notification with a glyph, not a number
            var glyph = document.getElementById("badgeGlyphTypes").options[document.getElementById("badgeGlyphTypes").selectedIndex].value;

            // create a string with the badge template xml
            badgeXmlString = "<badge value='" + glyph + "'/>";
        }

        // create a DOM
        var badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
        try {

            // load the xml string into the DOM, catching any invalid xml characters 
            badgeDOM.loadXml(badgeXmlString);

            // create a badge notification
            var badge = new Windows.UI.Notifications.BadgeNotification(badgeDOM);

            // send the notification to the app's application tile
            Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().update(badge);

            WinJS.log && WinJS.log(badgeDOM.getXml(), "sample", "status");
        } catch (e) {
            WinJS.log && WinJS.log("Error loading the xml, check for invalid characters in the input", "sample", "error");
        }
    }

    function badgeTypeChanged() {
        var badgeType = document.getElementById("badgeTypeSelector").options[document.getElementById("badgeTypeSelector").selectedIndex].value;

        var divToShow;
        var divToHide;
        if (badgeType === "Number") {
            divToShow = document.getElementById("badgeNumber");
            divToHide = document.getElementById("badgeGlyph");

        } else {
            divToShow = document.getElementById("badgeGlyph");
            divToHide = document.getElementById("badgeNumber");
        }
        divToShow.style.display = "block";
        divToHide.style.display = "none";
    }
})();
