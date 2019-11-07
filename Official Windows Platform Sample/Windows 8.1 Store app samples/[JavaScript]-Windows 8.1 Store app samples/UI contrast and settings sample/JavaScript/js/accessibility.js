//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/accessibility.html", {
        ready: function (element, options) {
            id("button1").addEventListener("click", getAccessibilitySettings, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function getAccessibilitySettings() {
        var accessibilitySettings = new Windows.UI.ViewManagement.AccessibilitySettings();
        id("highContrast").innerHTML = accessibilitySettings.highContrast;
        id("highContrastScheme").innerHTML = accessibilitySettings.highContrast ? accessibilitySettings.highContrastScheme : "undefined";
    }
})();
