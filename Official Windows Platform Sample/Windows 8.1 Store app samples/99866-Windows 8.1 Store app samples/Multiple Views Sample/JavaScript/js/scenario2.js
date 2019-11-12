//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ApplicationData = Windows.Storage.ApplicationData;
    var ViewManagement = Windows.UI.ViewManagement;

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            var disableMainBox = document.getElementById("disableMainBox");

            // Normally, you would hard code calling DisableShowingMainViewOnActivation
            // in the activation handler of default.js. The sample allows 
            // the user to change the preference for demonstration purposes.

            // Check the data stored from last run. Restart if you change this. See
            // default.js for calling DisableShowingMainViewOnActivation
            var shouldDisable = ApplicationData.current.localSettings.values[MultipleViews.disableMainViewKey];
            if (shouldDisable) {
                disableMainBox.checked = true;
            } else {
                disableMainBox.checked = false;
            }

            disableMainBox.addEventListener("change", disableBoxChanged, false);
        }
    });

    function disableBoxChanged(e) {
        // Normally, you would hard code calling DisableShowingMainViewOnActivation
        // in the activation handler of default.js. The sample allows 
        // the user to change the preference for demonstration purposes.
        ApplicationData.current.localSettings.values[MultipleViews.disableMainViewKey] = e.srcElement.checked;
    }
})();
