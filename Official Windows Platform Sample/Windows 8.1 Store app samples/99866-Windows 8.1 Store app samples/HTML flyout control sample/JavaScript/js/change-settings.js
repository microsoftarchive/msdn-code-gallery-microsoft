//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/change-settings.html", {
        ready: function (element, options) {
            document.getElementById("formatTextButton").addEventListener("click", showFormatTextFlyout, false);
            document.getElementById("textColor").addEventListener("change", changeColor, false);
            document.getElementById("textSize").addEventListener("change", changeSize, false);
        }
    });

    // Show the flyout
    function showFormatTextFlyout() {
        var formatTextButton = document.getElementById("formatTextButton");
        document.getElementById("formatTextFlyout").winControl.show(formatTextButton);
    }

    // Change the text color
    function changeColor() {
        document.getElementById("outputText").style.color = document.getElementById("textColor").value;
    }

    // Change the text size
    function changeSize() {
        document.getElementById("outputText").style.fontSize = document.getElementById("textSize").value + "pt";
    }
})();
