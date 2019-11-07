//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_DisplayCalendarStatistics.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.Calendar class to display the calendar
        // system statistics.

        // Clock and calendar identifiers
        var CalendarIdentifiers = Windows.Globalization.CalendarIdentifiers;
        var ClockIdentifiers = Windows.Globalization.ClockIdentifiers;

        // Obtain today's date.
        var calendarDate = new Date();

        // Calendar based on the current user.
        var cal1 = new Windows.Globalization.Calendar();
        cal1.setDateTime(calendarDate);

        // Calendar using Japanese language and Japanese calendar.
        var cal2 = new Windows.Globalization.Calendar(["ja-JP"], CalendarIdentifiers.japanese, ClockIdentifiers.twelveHour);
        cal2.setDateTime(calendarDate);

        // Calendar using Hewbrew language and Hebrew calendar.
        var cal3 = new Windows.Globalization.Calendar(["he-IL"], CalendarIdentifiers.hebrew, ClockIdentifiers.twentyFourHour);
        cal3.setDateTime(calendarDate);

        var calStats1 = "User's default calendar: " + cal1.getCalendarSystem() + "\n" +
                        "Months in this Year: " + cal1.numberOfMonthsInThisYear + "\n" +
                        "Days in this Month: " + cal1.numberOfDaysInThisMonth + "\n" +
                        "Hours in this Period: " + cal1.numberOfHoursInThisPeriod + "\n" +
                        "Era: " + cal1.eraAsString();

        var calStats2 = "Calendar system: " + cal2.getCalendarSystem() + "\n" +
                        "Months in this Year: " + cal2.numberOfMonthsInThisYear + "\n" +
                        "Days in this Month: " + cal2.numberOfDaysInThisMonth + "\n" +
                        "Hours in this Period: " + cal2.numberOfHoursInThisPeriod + "\n" +
                        "Era: " + cal2.eraAsString();

        var calStats3 = "Calendar system: " + cal3.getCalendarSystem() + "\n" +
                        "Months in this Year: " + cal3.numberOfMonthsInThisYear + "\n" +
                        "Days in this Month: " + cal3.numberOfDaysInThisMonth + "\n" +
                        "Hours in this Period: " + cal3.numberOfHoursInThisPeriod + "\n" +
                        "Era: " + cal3.eraAsString();

        // Display the result.
        var results = calStats1 + "\n\n" + calStats2 + "\n\n" + calStats3;

        document.getElementById("output").innerText = results;
    }
})();
