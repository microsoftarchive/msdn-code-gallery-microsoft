//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("loadContent3").addEventListener("click", loadContent, false);
            document.getElementById("resize").addEventListener("click", resize, false);
        }
    });

    function ensureContentFitsInRegions() {
        var element = document.getElementById("s3-flexbox");
        if (element.children.length > 0) {
            var regionElement = element.lastChild;

            if (regionElement.msRegionOverflow === "overflow") {
                // Create additional region elements until the content no longer overflows the last region.
                while (regionElement.msRegionOverflow === "overflow") {
                    regionElement = document.createElement("div");
                    regionElement.className = "s3-container";
                    element.appendChild(regionElement);
                }
            } else if (regionElement.msRegionOverflow === "empty") {
                // Remove all of the empty region elements.
                while (regionElement.msRegionOverflow === "empty") {
                    element.removeChild(regionElement);
                    regionElement = element.lastChild;
                }
            }
        }
    }

    function resize() {
        var element = document.getElementById("s3-flexbox");
        element.style.width = (element.style.width === "100%" || element.style.width === "") ? "50%" : "100%";
        ensureContentFitsInRegions();
        WinJS.log && WinJS.log("Element was resized", "sample", "status");
    }

    function loadContent() {
        var contentSourceElement = document.getElementById("s3-content-source");
        contentSourceElement.src = "/html/content.html";
        contentSourceElement.addEventListener("load", function () {
            var element = document.getElementById("s3-flexbox");
            element.innerHTML = "";
            var regionElement = document.createElement("div");
            regionElement.className = "s3-container";
            element.appendChild(regionElement);

            while (regionElement.msRegionOverflow === "overflow") {
                regionElement = document.createElement("div");
                regionElement.className = "s3-container";
                element.appendChild(regionElement);
            }

            WinJS.log && WinJS.log("Content Loaded", "sample", "status");
        });
    }
})();
