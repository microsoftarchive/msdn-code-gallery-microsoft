//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/enterPage.html", {
        ready: function (element, options) {
            runAnimation.addEventListener("click", runEnterPageAnimation, false);
        }
    });

    function runEnterPageAnimation() {
        // Get user selection from control
        var pageSections = pageSectionsControl.value;
        content.style.overflow = "hidden";

        // Animate the application's entrance in the number of stages chosen by the user
        // Use the recommended offset by leaving the offset argument empty to get the best performance
        var enterPage;
        switch (pageSections) {
            case "1":
                // Animate the whole page together
                enterPage = WinJS.UI.Animation.enterPage(rootGrid, null);
                break;
            case "2":
                // Stagger the header and body
                enterPage = WinJS.UI.Animation.enterPage([[header, featureLabel], [contentHost, footer]], null);
                break;
            case "3":
                // Stagger the header, input, and output areas
                enterPage = WinJS.UI.Animation.enterPage([[header, featureLabel], [inputLabel, input], [outputLabel, output, footer]], null);
                break;
        }
        outputText.innerText = "Page appeared in " + pageSections + " section(s).";

        enterPage.done(
            function () {
                content.style.overflow = "auto";
            });
    }
})();
