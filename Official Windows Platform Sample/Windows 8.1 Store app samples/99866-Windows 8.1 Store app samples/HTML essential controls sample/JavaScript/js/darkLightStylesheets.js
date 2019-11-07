//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/darkLightStylesheets.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });

})();

var pageColorStyle = document.createElement('style');
pageColorStyle.type = 'text/css';
document.getElementsByTagName('head')[0].appendChild(pageColorStyle);

function changePageColor() {
    var red = document.getElementById("redSlider").valueAsNumber;
    var green = document.getElementById("greenSlider").valueAsNumber;
    var blue = document.getElementById("blueSlider").valueAsNumber;

    pageColorStyle.innerHTML = "body.stylePageColor{ background-color: rgb(" + red + ", " + green + ", " + blue + "); }";

    if (!WinJS.Utilities.hasClass(document.body, "stylePageColor")) {
        WinJS.Utilities.addClass(document.body, "stylePageColor");
    }

    updateControlStyleMessage();
}

function getPageColor() {
    var pageColor = window.getComputedStyle(document.body).getPropertyValue("background-color");
    var colors = /rgb\((\d+), ?(\d+), ?(\d+)\)/.exec(pageColor);
    return colors;
}

// tell the developer what's the suggested control style sheet
function updateControlStyleMessage() {
    var msg = document.getElementById("controlStyleMessage");

    var colors = getPageColor(),
        red = parseInt(colors[1]),
        green = parseInt(colors[2]),
        blue = parseInt(colors[3]);

    var luminance = 0.299 * red + 0.587 * green + 0.114 * blue;    // human perceived brightness
    if (luminance >= 128) {
        msg.innerText = "For current page color you should use light style.";
        WinJS.Utilities.removeClass(msg, "dark");
        WinJS.Utilities.addClass(msg, "light");
    } else {
        msg.innerText = "For current page color you should use dark style.";
        WinJS.Utilities.removeClass(msg, "light");
        WinJS.Utilities.addClass(msg, "dark");
    }
}

function resetPageColor() {
    WinJS.Utilities.removeClass(document.body, "stylePageColor");
    updateControlStyleMessage();
    updatePageColorSlider();
}

function updatePageColorSlider() {
    var colors = getPageColor();
    document.getElementById("redSlider").value = colors[1];
    document.getElementById("greenSlider").value = colors[2];
    document.getElementById("blueSlider").value = colors[3];
}
