//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/range-bingMaps.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });

})();

function loadMap() {
    var key = document.getElementById("developerKeyTextbox").value;
    var errorElement = document.getElementById("mapError");

    if ("" === key) {
        errorElement.innerText = "Error: no developer key.";
    } else {
        errorElement.innerText = "";

        var domain = "*";
        if (window.Windows) {
            domain = "ms-appx-web://" + document.location.host;
        }

        document.getElementById("mapContainer").contentWindow.postMessage(key, domain);
    }
}
