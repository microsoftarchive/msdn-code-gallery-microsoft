//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/uisettings.html", {
        ready: function (element, options) {
            id("button1").addEventListener("click", getUISettings, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function getUISettings() {
        var uiSettings = new Windows.UI.ViewManagement.UISettings();
        var color;

        id("handPreference").innerHTML = uiSettings.handPreference;
        id("cursorSize").innerHTML = uiSettings.cursorSize.width + " x " + uiSettings.cursorSize.height;
        id("scrollbarSize").innerHTML = uiSettings.scrollBarSize.width + " x " + uiSettings.scrollBarSize.height;
        id("scrollbarArrowSize").innerHTML = uiSettings.scrollBarArrowSize.width + " x " + uiSettings.scrollBarArrowSize.height;
        id("scrollbarThumbBoxSize").innerHTML = uiSettings.scrollBarThumbBoxSize.width + " x " + uiSettings.scrollBarThumbBoxSize.height;
        id("messageDuration").innerHTML = uiSettings.messageDuration;
        id("animationsEnabled").innerHTML = uiSettings.animationsEnabled;
        id("caretBrowsingEnabled").innerHTML = uiSettings.caretBrowsingEnabled;
        id("caretBlinkRate").innerHTML = uiSettings.caretBlinkRate;
        id("caretWidth").innerHTML = uiSettings.caretWidth;
        id("doubleClickTime").innerHTML = uiSettings.doubleClickTime;
        id("mouseHoverTime").innerHTML = uiSettings.mouseHoverTime;

        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.activeCaption);
        id("activeCaptionColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.background);
        id("backgroundColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.buttonFace);
        id("buttonFaceColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.buttonText);
        id("buttonTextColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.captionText);
        id("captionTextColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.grayText);
        id("grayTextColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.highlight);
        id("highlightColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.highlightText);
        id("highlightTextColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.hotlight);
        id("hotlightColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.inactiveCaption);
        id("inactiveCaptionColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.inactiveCaptionText);
        id("inactiveCaptionTextColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.window);
        id("windowColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
        color = uiSettings.uiElementColor(Windows.UI.ViewManagement.UIElementType.windowText);
        id("windowTextColor").innerHTML = color.r.toString(10) + ", " + color.g.toString(10) + ", " + color.b.toString(10);
    }
})();
