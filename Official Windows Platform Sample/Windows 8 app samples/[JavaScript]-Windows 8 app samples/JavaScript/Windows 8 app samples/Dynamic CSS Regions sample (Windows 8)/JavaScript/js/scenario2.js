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
            document.getElementById("loadContent2").addEventListener("click", loadContent, false);
        }
    });

    function loadContent() {
        var contentSourceElement = document.getElementById("s2-content-source");
        contentSourceElement.src = "/html/content.html";
        contentSourceElement.addEventListener("load", function () {
            var element = document.getElementById("s2-flexbox");
            element.innerHTML = "";
            var regionElement = document.createElement("div");
            regionElement.className = "s2-container";
            element.appendChild(regionElement);

            while (regionElement.msRegionOverflow === "overflow") {
                regionElement = document.createElement("div");
                regionElement.className = "s2-container";
                element.appendChild(regionElement);
            }

            WinJS.log && WinJS.log("Content Loaded", "sample", "status");
        });
    }
})();
