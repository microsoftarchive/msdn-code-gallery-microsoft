//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario6_HighPriority.html", {
        ready: function (element, options) {
            document.getElementById("incrementHighPriority").addEventListener("click", incrementHighPriority, false);
            Windows.Storage.ApplicationData.current.addEventListener("datachanged", roamingDataChangedHandler);
            highPriorityDisplayOutput(false);
        },

        unload: function () {
            Windows.Storage.ApplicationData.current.removeEventListener("datachanged", roamingDataChangedHandler);
        }
    });

    var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;

    // Guidance for using the HighPriority setting.
    //
    // Writing to the HighPriority setting enables a developer to store a small amount of
    // data that will be roamed out to the cloud with higher priority than other roaming
    // data, when possible.
    //
    // Applications should carefully consider which data should be stored in the 
    // HighPriority setting.  "Context" data such as the user's location within
    // media, or their current game-baord and high-score, can make the most sense to
    // roam with high priority.  By using the HighPriority setting, this information has
    // a higher likelihood of being available to the user when they begin to use another
    // machine.
    //
    // Applications should update their HighPriority setting when the user makes
    // a significant change to the data it represents.  Examples could include changing
    // music tracks, turning the page in a book, or finishing a level in a game.

    function incrementHighPriority() {
        var counter = roamingSettings.values["HighPriority"] || 0;

        roamingSettings.values["HighPriority"] = counter + 1;

        highPriorityDisplayOutput(false);
    }

    function roamingDataChangedHandler() {
        highPriorityDisplayOutput(true);
    }

    function highPriorityDisplayOutput(remoteUpdate)
    {
        var counter = roamingSettings.values["HighPriority"] || 0;

        document.getElementById("highPriorityOutput").innerHTML = "Counter: " + counter + (remoteUpdate ? " (updated remotely)" : "");
    }

})();
