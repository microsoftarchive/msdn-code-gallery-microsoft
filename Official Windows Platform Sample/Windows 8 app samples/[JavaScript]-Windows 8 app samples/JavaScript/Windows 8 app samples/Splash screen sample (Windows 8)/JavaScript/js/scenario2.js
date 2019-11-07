//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            // Display the splash screen image coordinates that were used to position the extended splash screen image.
            document.getElementById("y").innerText = SdkSample.coordinates.y;
            document.getElementById("x").innerText = SdkSample.coordinates.x;
            document.getElementById("width").innerText = SdkSample.coordinates.width;
            document.getElementById("height").innerText = SdkSample.coordinates.height;
        }
    });
})();
