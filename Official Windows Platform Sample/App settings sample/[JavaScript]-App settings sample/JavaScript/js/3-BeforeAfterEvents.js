//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/3-BeforeAfterEvents.html", {
        ready: function (element, options) {
            document.getElementById("scenario3Add").addEventListener("click", scenario3AddSettingsFlyout, false);

            // clear out the current on settings handler to ensure scenarios are atomic
            WinJS.Application.onsettings = null;

            // Display invocation instructions in the SDK sample output region
            WinJS.log && WinJS.log("To show the settings charm, invoke the charm bar by swiping your finger on the right edge of the screen or bringing your mouse to the lower-right corner of the screen, then select Settings. Or you can just press Windows logo + i. To dismiss the settings charm, tap in the application, swipe a screen edge, right click, invoke another charm or application.", "sample", "status");
        }
    });

    function scenario3AddSettingsFlyout() {
        WinJS.Application.onsettings = function (e) {
            e.detail.applicationcommands = { "gameOptions": { title: "Game options", href: "/html/3-SettingsFlyout-Game.html" } };
            WinJS.UI.SettingsFlyout.populateSettings(e);
        };
        // Make sure the following is called after the DOM has initialized. Typically this would be part of app initializationv
        WinJS.Application.start();

        // Display a status message in the SDK sample output region
        WinJS.log && WinJS.log("Game options command and settings flyout added from 3-SettingsFlyout-Game.html", "samples", "status");
    }

})();
