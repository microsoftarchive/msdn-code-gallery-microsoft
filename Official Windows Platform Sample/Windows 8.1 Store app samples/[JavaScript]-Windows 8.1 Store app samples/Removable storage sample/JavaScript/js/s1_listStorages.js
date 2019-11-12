//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/s1_listStorages.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", scenario1_listStorages, false);
        }
    });

    //// There are two ways to find removable storages:
    //// The first way uses the Removable Devices KnownFolder to get a snapshot of the currently
    //// connected devices as StorageFolders.  This is demonstrated in scenario 1.
    //// The second way uses Windows.Devices.Enumeration and is demonstrated in scenario 2().
    //// Windows.Devices.Enumeration supports more advanced scenarios such as subscibing for device
    //// arrival, removal and updates. Refer to the DeviceEnumeration sample for details on
    //// Windows.Devices.Enumeration.
    function scenario1_listStorages() {
        var scenarioOutput = document.getElementById("scenarioOutput");
        scenarioOutput.innerHTML = "";

        // Find all storage devices using the known folder
        Windows.Storage.KnownFolders.removableDevices.getFoldersAsync()
            .done(
                function (removableStorages) {
                    // Display each storage device
                    var numRemovableStorages = removableStorages.length;
                    if (numRemovableStorages > 0) {
                        removableStorages.forEach(function (removableStorage, i) {
                            scenarioOutput.innerHTML += removableStorage.displayName + "<br/>";
                        });
                    } else {
                        WinJS.log && WinJS.log("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", "sample", "status");
                    }
                },
                function (e) {
                    WinJS.log && WinJS.log("Failed to find all storage devices, error: " + e.message, "sample", "error");
                });
    }

})();
