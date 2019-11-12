//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5_CalendarTimeZoneSupport.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario illustrates TimeZone support in Windows.Globalization.Calendar class

        // Displayed TimeZones (other than local timezone)
        var timeZones = ["UTC", "America/New_York", "Asia/Kolkata"];

        // Create a calendar based on the current user.
        var calendar = new Windows.Globalization.Calendar();
        var localTimeZone = calendar.getTimeZone();

        // Show current time in timezones desired to be displayed including local timezone
        var results = "Current date and time -" + "\n" ;
        results += getFormattedCalendarDateTime(calendar);
        for (var i in timeZones) {
            calendar.changeTimeZone(timeZones[i]);
            results += getFormattedCalendarDateTime(calendar);
        }
        results += "\n";
        calendar.changeTimeZone(localTimeZone);

        // Show a time on 14th day of second month of next year in local, GMT, New York and Indian Time Zones
        // This will show if there were day light savings in time
        results += "Same time on 14th day of second month of next year -" + "\n";
        calendar.addYears(1); calendar.month = 2; calendar.day = 14;
        results += getFormattedCalendarDateTime(calendar);
        for (i in timeZones) {
            calendar.changeTimeZone(timeZones[i]);
            results += getFormattedCalendarDateTime(calendar);
        }
        results += "\n";
        calendar.changeTimeZone(localTimeZone);

        // Show a time on 14th day of 10th month of next year in local, GMT, New York and Indian Time Zones
        // This will show if there were day light savings in time
        results += "Same time on 14th day of tenth month of next year -" + "\n";
        calendar.addMonths(8);
        results += getFormattedCalendarDateTime(calendar);
        for (i in timeZones) {
            calendar.changeTimeZone(timeZones[i]);
            results += getFormattedCalendarDateTime(calendar);
        }
        results += "\n";
        calendar.changeTimeZone(localTimeZone);

        // Display the result.
        document.getElementById("output").innerText = results;
    };

    /// This is a helper function to display calendar's date-time in presentable format
    function getFormattedCalendarDateTime(calendar) {
        var returnString = "In " + calendar.getTimeZone() + " TimeZone:    " +
                           calendar.dayOfWeekAsString() + "   " +
                           calendar.monthAsString() + " " +
                           calendar.dayAsPaddedString(2) + ", " +
                           calendar.yearAsString() + "   " +
                           calendar.hourAsPaddedString(2) + ":" +
                           calendar.minuteAsPaddedString(2) + ":" +
                           calendar.secondAsPaddedString(2) + " " +
                           calendar.periodAsString() + "  " +
                           calendar.timeZoneAsString(3) + "\n";
        return returnString;
    }
})();
