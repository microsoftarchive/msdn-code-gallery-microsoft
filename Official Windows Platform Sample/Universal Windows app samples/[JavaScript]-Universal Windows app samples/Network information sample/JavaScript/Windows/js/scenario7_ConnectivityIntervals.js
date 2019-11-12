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
    var page = WinJS.UI.Pages.define("/html/scenario7_ConnectivityIntervals.html", {
        ready: function (element, options) {
            document.getElementById("scenario7").addEventListener("click", displayConnectivityIntervals, false);
            startDatePicker = new WinJS.UI.DatePicker(StartDatePicker);
            startTimePicker = new WinJS.UI.TimePicker(StartTimePicker);
            endDatePicker = new WinJS.UI.DatePicker(EndDatePicker);
            endTimePicker = new WinJS.UI.TimePicker(EndTimePicker);
        }
    });

    // Declare the Date and Time Picker objects
    var startDatePicker;
    var startTimePicker;
    var startDateTime;
    var endDatePicker;
    var endTimePicker;
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

    function printConnectivityInterval(connectivityInterval) {
        var time = new Date();
        time.setTime(connectivityInterval.startTime.getTime() + connectivityInterval.connectionDuration);
        var result = "Connectivity Interval from " + connectivityInterval.startTime.toString() + " to " + time + "\n";

        return result;
    }

    function getConnectivityIntervalsCompletedHandler(connectivityIntervals) {
        // ConnectivityIntervals can be null if the start time is earlier than the end time
        if (connectivityIntervals === null) {
            WinJS.log && WinJS.log("An error has occurred: Ensure that the start time is earlier than the end time", "sample", "error");
            return;
        }

        var outputString = "";

        // Loop through the ConnectivityIntervals
        for (var i = 0; i < connectivityIntervals.size; i++) {
            outputString += printConnectivityInterval(connectivityIntervals[i]);
        }

        OutputText.textContent = outputString;
        WinJS.log && WinJS.log("Success", "sample", "status");
    }

    function getConnectivityIntervalsErrorHandler(error) {
        WinJS.log && WinJS.log("An error has occurred: " + error, "sample", "error");
    }

    //
    //Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
    //
    function displayConnectivityIntervals() {
        try {
            // Get settings from the UI
            networkUsageStates.roaming = parseTriStates(RoamingSelect.children[RoamingSelect.selectedIndex].textContent);
            networkUsageStates.shared = parseTriStates(SharedSelect.children[SharedSelect.selectedIndex].textContent);

            // Add together the values from the DatePicker and the TimePicker
            // Note: The date portion of the value returned by TimePicker is always July 15, 2011
            startDateTime = startTimePicker.current;
            startDateTime.setFullYear(startDatePicker.current.getFullYear(), startDatePicker.current.getMonth(), startDatePicker.current.getDate());

            endDateTime = endTimePicker.current;
            endDateTime.setFullYear(endDatePicker.current.getFullYear(), endDatePicker.current.getMonth(), endDatePicker.current.getDate());

            if (internetConnectionProfile === null) {
                WinJS.log && WinJS.log("Not connected to Internet\n\r", "sample", "status");
            }
            else {
                internetConnectionProfile.getConnectivityIntervalsAsync(startDateTime, endDateTime, networkUsageStates).then(
                    getConnectivityIntervalsCompletedHandler, getConnectivityIntervalsErrorHandler);
            }
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();