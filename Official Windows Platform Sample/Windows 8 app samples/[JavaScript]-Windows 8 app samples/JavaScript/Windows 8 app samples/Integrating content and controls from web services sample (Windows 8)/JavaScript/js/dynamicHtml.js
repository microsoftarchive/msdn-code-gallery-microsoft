//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/dynamicHtml.html", {
        ready: function (element, options) {
            try {
                document.getElementById("dynamicHtmlOutput1").innerHTML = "<div onclick='this.style.color = \"red\"'>Content for option 1</div>";
            }
            catch (e) {
                document.getElementById("dynamicHtmlOutput1").innerHTML = "JS error is thrown = " + e;
            }

            document.getElementById("dynamicHtmlOutput2").innerHTML = toStaticHTML("<div onclick='this.style.color = \"red\"'>Content for option 2</div>");

            // Assume you obtained this content from a XHR call, and expect it to be only text (no html)
            var externalContent = "<div onclick='this.style.color = \"red\"'>Content for option 3</div>";
            document.getElementById("dynamicHtmlOutput3").innerText = externalContent;
        }
    });
})();
