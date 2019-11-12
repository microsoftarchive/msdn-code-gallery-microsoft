//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/header-menu.html", {
        ready: function (element, options) {
            document.querySelector(".titlearea").addEventListener("click", showHeaderMenu, false);
            document.getElementById("collectionMenuItem").addEventListener("click", function() { goToSection("Collection"); }, false);
            document.getElementById("marketplaceMenuItem").addEventListener("click", function () { goToSection("Marketplace"); }, false);
            document.getElementById("newsMenuItem").addEventListener("click", function () { goToSection("News"); }, false);
            document.getElementById("homeMenuItem").addEventListener("click", function () { goHome(); }, false);

            WinJS.log && WinJS.log("Click or tap the title to show the header menu.", "sample", "status");
        }
    });

    // Place the menu under the title and aligned to the left of it
    function showHeaderMenu() {
        var title = document.querySelector("header .titlearea");
        var menu = document.getElementById("headerMenu").winControl;
        menu.anchor = title;
        menu.placement = "bottom";
        menu.alignment = "left";

        menu.show();
    }

    // When navigating using the header menu for sections, change the subtitle to reflect the current pivot
    function goToSection(section) {
        WinJS.log && WinJS.log("You are viewing the " + section + " section.", "sample", "status");
    }

    // Hide the subtitle if no pivot is being used
    function goHome() {
        WinJS.log && WinJS.log("You are home.", "sample", "status");
    }

})();
