//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var networkInfo = Windows.Networking.Connectivity.NetworkInformation;
    var connectivityNS = Windows.Networking.Connectivity;
    var page = WinJS.UI.Pages.define("/html/scenario8_AttributedNetworkUsage.html", {
        ready: function (element, options) {
            document.getElementById("scenario8").addEventListener("click", displayLocalDataUsage, false);
            var now = new Date();
            var yesterday = new Date(now);
            yesterday.setDate(yesterday.getDate() - 1);
            StartDateTimePicker.value = yesterday.toString();
            EndDateTimePicker.value = now.toString();
        }
    });

    // Declare the DateTime objects
    var startDateTime;
    var endDateTime;

    var internetConnectionProfile = networkInfo.getInternetConnectionProfile();
    var networkUsageStates = {
        roaming: connectivityNS.TriStates.doNotCare,
        shared: connectivityNS.TriStates.doNotCare
    };

    function parseTriStates(input) {
        switch (input) {
            case "Yes":
                return connectivityNS.TriStates.yes;
            case "No":
                return connectivityNS.TriStates.no;
            default:
                return connectivityNS.TriStates.doNotCare;
        }
    }

    function printNetworkUsage(networkUsage, startTime, endTime) {
        var result = "Usage for: " + networkUsage.attributionName +
            "\n\tWith start time " + startTime + " and end time " + endTime +
            "\n\tBytes sent: " + networkUsage.bytesSent +
            "\n\tBytes received: " + networkUsage.bytesReceived + "\n";

        return result;
    }

    function getNetworkUsageCompletedHandler(networkUsages) {
        var outputString = "";

        for (var i = 0; i < networkUsages.size; i++) {
            outputString += printNetworkUsage(networkUsages[i], startDateTime.getTime(), endDateTime.getTime());
        }

        if (networkUsages.size === 0) {
            outputString = "No AttributedNetworkUsages returned during this interval.";
        }

        OutputText.textContent = outputString;
        WinJS.log && WinJS.log("Success", "sample", "status");
    }

    function getNetworkUsageErrorHandler(error) {
        WinJS.log && WinJS.log("An error has occurred: " + error, "sample", "error");
    }

    //
    //Get Internet Connection Profile and display attributed network usage for selected time interval
    //
    function displayLocalDataUsage() {
        try {
            // Get settings from the UI
            networkUsageStates.roaming = parseTriStates(RoamingSelect.children[RoamingSelect.selectedIndex].textContent);
            networkUsageStates.shared = parseTriStates(SharedSelect.children[SharedSelect.selectedIndex].textContent);

            startDateTime = new Date(StartDateTimePicker.value);
            endDateTime = new Date(EndDateTimePicker.value);

            if (internetConnectionProfile === null) {
                WinJS.log && WinJS.log("Not connected to Internet\n\r", "sample", "status");
            }
            else if (!internetConnectionProfile.isWlanConnectionProfile && !internetConnectionProfile.isWwanConnectionProfile) {
                WinJS.log && WinJS.log("getAttributedNetworkUsage is not supported on the emulator.", "sample", "error");
            }
            else {
                internetConnectionProfile.getAttributedNetworkUsageAsync(startDateTime, endDateTime, networkUsageStates).then(
                    getNetworkUsageCompletedHandler, getNetworkUsageErrorHandler);
            }
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();
