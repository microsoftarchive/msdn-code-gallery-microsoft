//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario6_TimeZoneSupport.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario illustrates TimeZone support in DateTimeFormatter class

        // Displayed TimeZones (other than local timezone)
        var timeZones = ["UTC", "America/New_York", "Asia/Kolkata"];
        
        // Create formatter object using longdate and longtime template
        var formatter = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longdate longtime");
        
        // Obtain the date that will be formatted.
        var dateToFormat = new Date();
        
        // Show current time in timezones desired to be displayed including local timezone
        var results = "Current date and time -\n";
        results += "In Local timezone:   " + formatter.format(dateToFormat) + "\n";
        for (var i in timeZones) {
            results += "In " + timeZones[i] + " timezone:   " + formatter.format(dateToFormat, timeZones[i]) + "\n";
        }
        results += "\n";

        // Show a time on 14th day of second month of next year in local, and other desired Time Zones
        // This will show if there were day light savings in time (month #s start from zero for date in js)
        results += "Same time on 14th day of second month of next year -\n";
        dateToFormat = new Date(dateToFormat.getFullYear() + 1, 1, 14, dateToFormat.getHours(), dateToFormat.getMinutes(), dateToFormat.getSeconds());
        results += "In Local timezone:   " + formatter.format(dateToFormat) + "\n";
        for (i in timeZones) {
            results += "In " + timeZones[i] + " timezone:   " + formatter.format(dateToFormat, timeZones[i]) + "\n";
        }
        results += "\n";

        // Show a time on 14th day of 10th month of next year in local, and other desired Time Zones
        // This will show if there were day light savings in time
        results += "Same time on 14th day of tenth month of next year -\n";
        dateToFormat.setMonth(9);
        results += "In Local timezone:   " + formatter.format(dateToFormat) + "\n";
        for (i in timeZones) {
            results += "In " + timeZones[i] + " timezone:   " + formatter.format(dateToFormat, timeZones[i]) + "\n";
        }
        results += "\n";

        // Display the results.
        document.getElementById("output").innerText = results;
    }
})();
