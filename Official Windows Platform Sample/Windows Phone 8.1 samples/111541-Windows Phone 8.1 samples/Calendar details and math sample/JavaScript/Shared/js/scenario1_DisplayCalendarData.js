//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_DisplayCalendarData.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.Calendar class to display the parts of a date.

        // Clock and calendar identifiers
        var CalendarIdentifiers = Windows.Globalization.CalendarIdentifiers;
        var ClockIdentifiers = Windows.Globalization.ClockIdentifiers;

        // Obtain today's date.
        var calendarDate = new Date();

        // Create a calendar based on the current user.
        var cal1 = new Windows.Globalization.Calendar();
        cal1.setDateTime(calendarDate);

        // Create a calendar using Japanese language and Japanese calendar.
        var cal2 = new Windows.Globalization.Calendar(["ja-JP"], CalendarIdentifiers.japanese, ClockIdentifiers.twelveHour);
        cal2.setDateTime(calendarDate);

        // Create a calendar using Hebrew language and Hebrew calendar.
        var cal3 = new Windows.Globalization.Calendar(["he-IL"], CalendarIdentifiers.hebrew, ClockIdentifiers.twentyFourHour);
        cal3.setDateTime(calendarDate);

        // Obtain the date parts using the default calendar system.
        var calItems1 = "User's default calendar: " + cal1.getCalendarSystem() + "\n" +
                        "Name of Month: " + cal1.monthAsString() + "\n" +
                        "Day of Month: " + cal1.dayAsPaddedString(2) + "\n" +
                        "Day of Week: " + cal1.dayOfWeekAsString() + "\n" +
                        "Year: " + cal1.yearAsString();

        // Obtain the date parts using the Japanese calendar system.
        var calItems2 = "Calendar system: " + cal2.getCalendarSystem() + "\n" +
                        "Name of Month: " + cal2.monthAsString() + "\n" +
                        "Day of Month: " + cal2.dayAsPaddedString(2) + "\n" +
                        "Day of Week: " + cal2.dayOfWeekAsString() + "\n" +
                        "Year: " + cal2.yearAsString();

        // Obtain the date parts using the Hebrew calendar system.
        var calItems3 = "Calendar system: " + cal3.getCalendarSystem() + "\n" +
                        "Name of Month: " + cal3.monthAsString() + "\n" +
                        "Day of Month: " + cal3.dayAsPaddedString(2) + "\n" +
                        "Day of Week: " + cal3.dayOfWeekAsString() + "\n" +
                        "Year: " + cal3.yearAsString();

        // Display the result.
        var results = calItems1 + "\n\n" + calItems2 + "\n\n" + calItems3;

        document.getElementById("output").innerText = results;
    }

})();
