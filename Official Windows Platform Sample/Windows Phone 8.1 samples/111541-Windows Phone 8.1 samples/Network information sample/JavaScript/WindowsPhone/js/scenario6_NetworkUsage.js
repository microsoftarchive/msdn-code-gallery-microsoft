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
    var page = WinJS.UI.Pages.define("/html/scenario6_NetworkUsage.html", {
        ready: function (element, options) {
            document.getElementById("scenario6").addEventListener("click", displayLocalDataUsage, false);
            var now = new Date();
            var anHourAgo = new Date(now);
            anHourAgo.setHours(anHourAgo.getHours() - 1);
            StartDateTimePicker.value = anHourAgo.toString();
            EndDateTimePicker.value = now.toString();
        }
    });

    // Declare the DateTime objects
    var startDateTime;
    var endDateTime;

    var internetConnectionProfile = networkInfo.getInternetConnectionProfile();
    var granularity = connectivityNS.DataUsageGranularity.perHour;
    var networkUsageStates = {
        roaming: connectivityNS.TriStates.doNotCare,
        shared: connectivityNS.TriStates.doNotCare
    };

    function parseDataUsageGranularity(input) {
        switch (input) {
            case "Per Minute":
                return connectivityNS.DataUsageGranularity.perMinute;
            case "Per Hour":
                return connectivityNS.DataUsageGranularity.perHour;
            case "Per Day":
                return connectivityNS.DataUsageGranularity.perDay;
            default:
                return connectivityNS.DataUsageGranularity.total;
        }
    }

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
        var result = "Usage with start time " + startTime + " and end time " + endTime +
            "\n\tBytes sent: " + networkUsage.bytesSent +
            "\n\tBytes received: " + networkUsage.bytesReceived +
            "\n\tConnected Duration: " + networkUsage.connectionDuration + "\n";

        return result;
    }

    // Returns the amount of time in milliseconds between each period of network usage for a given granularity
    function granularityToTimeSpan(inputGranularity) {
        switch (inputGranularity) {
            case connectivityNS.DataUsageGranularity.perMinute:
                return 60 * 1000;
            case connectivityNS.DataUsageGranularity.perHour:
                return 60 * 60 * 1000;
            case connectivityNS.DataUsageGranularity.perDay:
                return 24 * 60 * 60 * 1000;
            default:
                return 0;
        }
    }

    function getNetworkUsageCompletedHandler(networkUsages) {
        var outputString = "";
        var timeSpan = granularityToTimeSpan(granularity);
        var startTime = new Date();
        startTime.setTime(startDateTime.getTime());

        for (var i = 0; i < networkUsages.size; i++) {
            var endTime = new Date();
            if (granularity === connectivityNS.DataUsageGranularity.total) {
                endTime.setTime(endDateTime.getTime());
            } else {
                endTime.setTime(startTime.getTime() + timeSpan);
            }

            outputString += printNetworkUsage(networkUsages[i], startTime, endTime);
            startTime.setTime(endTime.getTime());
        }

        OutputText.textContent = outputString;
        WinJS.log && WinJS.log("Success", "sample", "status");
    }

    function getNetworkUsageErrorHandler(error) {
        // This can happen if you try to get the network usage for a long period of time with
        // too fine a granularity.
        WinJS.log && WinJS.log("An error has occurred: " + error, "sample", "error");
    }

    //
    //Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
    //
    function displayLocalDataUsage() {
        try {
            // Get settings from the UI
            granularity = parseDataUsageGranularity(GranularitySelect.children[GranularitySelect.selectedIndex].textContent);
            networkUsageStates.roaming = parseTriStates(RoamingSelect.children[RoamingSelect.selectedIndex].textContent);
            networkUsageStates.shared = parseTriStates(SharedSelect.children[SharedSelect.selectedIndex].textContent);

            startDateTime = new Date(StartDateTimePicker.value);
            endDateTime = new Date(EndDateTimePicker.value);

            if (internetConnectionProfile === null) {
                WinJS.log && WinJS.log("Not connected to Internet\n\r", "sample", "status");
            }
            else {
                internetConnectionProfile.getNetworkUsageAsync(startDateTime, endDateTime, granularity, networkUsageStates).then(
                    getNetworkUsageCompletedHandler, getNetworkUsageErrorHandler);
            }
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();
