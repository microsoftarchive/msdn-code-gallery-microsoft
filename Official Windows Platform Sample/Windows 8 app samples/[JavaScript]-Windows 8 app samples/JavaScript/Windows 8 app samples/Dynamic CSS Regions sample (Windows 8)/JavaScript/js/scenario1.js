//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("loadContent1").addEventListener("click", loadContent, false);
        }
    });

    function loadContent() {
        var contentSourceElement = document.getElementById("s1-content-source");
        contentSourceElement.src = "/html/content.html";
        contentSourceElement.addEventListener("load", function () {
            var element = document.getElementById("s1-flexbox");
            element.innerHTML = "";
            var regionElement = document.createElement("div");
            regionElement.className = "s1-container";
            element.appendChild(regionElement);

            while (regionElement.msRegionOverflow === "overflow") {
                regionElement = document.createElement("div");
                regionElement.className = "s1-container";
                element.appendChild(regionElement);
            }

            WinJS.log && WinJS.log("Content Loaded", "sample", "status");
        });
    }
})();
