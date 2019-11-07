//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function ready(element, options) {
        WinJS.UI.processAll(element)
            .done(function () {
                document.getElementById("playMenuItem").addEventListener("click", GameManager.navigateGame, false);
                document.getElementById("rulesMenuItem").addEventListener("click", GameManager.navigateRules, false);
                document.getElementById("scoresMenuItem").addEventListener("click", GameManager.navigateScores, false);
                document.getElementById("creditsMenuItem").addEventListener("click", GameManager.navigateCredits, false);
                document.getElementById("settingsMenuItem").addEventListener("click", GameManager.navigateSettings, false);
            });
    }

    WinJS.UI.Pages.define("/html/homePage.html", {
        ready: ready
    });
})();
