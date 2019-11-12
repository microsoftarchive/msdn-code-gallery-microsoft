//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/4-ProgrammaticInvocation.html", {
        ready: function (element, options) {
            document.getElementById("scenario4Add").addEventListener("click", scenario4AddSettingsFlyout, false);
            document.getElementById("scenario4Show").addEventListener("click", scenario4ShowSettingsFlyout, false);

            // clear out the current on settings handler to ensure scenarios are atomic
            WinJS.Application.onsettings = null;

            // Display invocation instructions in the SDK sample output region
            WinJS.log && WinJS.log("To show the settings charm, invoke the charm bar by swiping your finger on the right edge of the screen or bringing your mouse to the lower-right corner of the screen, then select Settings. Or you can just press Windows logo + i. To dismiss the settings charm, tap in the application, swipe a screen edge, right click, invoke another charm or application.", "sample", "status");
        }
    });

    function scenario4AddSettingsFlyout() {
        WinJS.Application.onsettings = function (e) {
            e.detail.applicationcommands = { "defaults": { title: "Defaults", href: "/html/4-SettingsFlyout-Settings.html" } };
            WinJS.UI.SettingsFlyout.populateSettings(e);
        };
        // Make sure the following is called after the DOM has initialized. Typically this would be part of app initialization
        WinJS.Application.start();

        // Display a status message in the SDK sample output region
        WinJS.log && WinJS.log("Defaults settings flyout added from 4-SettingsFlyout-Settings.html", "samples", "status");
    }

    function scenario4ShowSettingsFlyout() {
        WinJS.UI.SettingsFlyout.showSettings("defaults", "/html/4-SettingsFlyout-Settings.html");

        // Display a status message in the SDK sample output region
        WinJS.log && WinJS.log("Defaults settings flyout showing", "samples", "status");
    }
})();
