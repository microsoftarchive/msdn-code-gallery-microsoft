//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5_DataChangedEvent.html", {
        ready: function (element, options) {
            document.getElementById("roamingSimulateRoaming").addEventListener("click", roamingSimulateRoaming, false);
            Windows.Storage.ApplicationData.current.addEventListener("datachanged", roamingDataChangedHandler);
            roamingDisplayOutput();
        },

        unload: function() {
            Windows.Storage.ApplicationData.current.removeEventListener("datachanged", roamingDataChangedHandler);
        }
    });

    var roamingSettings = Windows.Storage.ApplicationData.current.roamingSettings;
    var settingName = "userName";

    function roamingDataChangedHandler() {
        var value = roamingSettings.values[settingName];
        if (value) {
            document.getElementById("roamingOutput").innerText = "Name: \"" + value + "\"";
        } else {
            document.getElementById("roamingOutput").innerText = "Name: <empty>";
        }
    }

    function roamingSimulateRoaming() {
        roamingSettings.values[settingName] = document.getElementById("roamingUserName").value;

        // Simulate roaming by intentionally signaling a data changed event.
        Windows.Storage.ApplicationData.current.signalDataChanged();
    }

    function roamingDisplayOutput() {
        roamingDataChangedHandler();
    }
})();
