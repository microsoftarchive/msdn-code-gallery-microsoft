//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3_CalendarEnumerationAndMath.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.Calendar class to enumerate through a calendar and
        // perform calendar math

        // Clock and calendar identifiers
        var CalendarIdentifiers = Windows.Globalization.CalendarIdentifiers;
        var ClockIdentifiers = Windows.Globalization.ClockIdentifiers;

        // Store results here.
        var results = "The number of years in each era of the Japanese era calendar is not regular. It is determined by the length of the given imperial era:\n\n";

        // This section demonstrates enumeration through the segments of a calendar, using the Japanese era calendar as an example.
        // Create Japanese calendar.
        var calendar = new Windows.Globalization.Calendar(["en-US"], CalendarIdentifiers.japanese, ClockIdentifiers.twentyFourHour);

        // Enumerate all supported years in all supported Japanese eras.
        for (calendar.era = calendar.firstEra; true; calendar.addYears(1)) {
            // Process current era.
            results = results + "Era " + calendar.eraAsString() + " contains " + calendar.numberOfYearsInThisEra + " year(s)\n";
            
            // Enumerate all years in this era.
            for (calendar.year = calendar.firstYearInThisEra; true; calendar.addYears(1)) {
                // Begin sample processing of current year.

                // Move to first day of year. Change of month can affect day so order of assignments is important.
                calendar.month = calendar.firstMonthInThisYear;
                calendar.day = calendar.firstDayInThisMonth;

                // Set time to midnight (local).
                calendar.period = calendar.firstPeriodInThisDay;    // All days have 1 or 2 periods depending on clock type
                calendar.hour = calendar.firstHourInThisPeriod;     // Hours start from 12 or 0 depending on clock type
                calendar.minute = 0;
                calendar.second = 0;
                calendar.nanosecond = 0;

                if (calendar.year % 1000 === 0)
                {
                    results = results + "\n";
                }
                else if (calendar.year % 10 === 0)
                {
                    results = results + ".";
                }

                // End sample processing of current year.

                // Break after processing last year.
                if (calendar.year === calendar.lastYearInThisEra) {
                    break;
                }
            }
            results = results + "\n";

            // Break after processing last era.
            if (calendar.era === calendar.lastEra) {
                break;
            }
        }

        // This section shows enumeration through the hours in a day to demonstrate that the number of time units in a given period (hours in a day, minutes in an hour, etc.)
        // should not be regarded as fixed. With Daylight Saving Time and other local calendar adjustments, a given day may have not have 24 hours, and
        // a given hour may not have 60 minutes, etc.
        results = results + "\nThe number of hours in a day is not invariable. The US calendar transitions from DST to standard time on 4 November 2012:\n\n";

        // Create a DateTimeFormatter to display dates
        var displayDate = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longdate");

        // Create a gregorian calendar for the US with 12-hour clock format
        var currentCal = new Windows.Globalization.Calendar(["en-US"], CalendarIdentifiers.gregorian, ClockIdentifiers.twentyFourHour, "america/los_angeles");

        // Set the calendar to a the date of the Daylight Saving Time-to-Standard Time transition for the US in 2012.
        // DST ends in the US at 02:00 on 4 November 2012
        var dstDate = new Date(2012, 10, 4);  
        currentCal.setDateTime(dstDate);

        // Set the current calendar to one day before DST change. Create a second calendar for comparision and set it to one day after DST change.
        var endDate = currentCal.clone();
        currentCal.addDays(-1);
        endDate.addDays(1);

        // Enumerate the day before, the day of, and the day after the 2012 DST-to-Standard time transition
        while (currentCal.day <= endDate.day) {
            // Process current day.
            var date = currentCal.getDateTime();
            results = results + displayDate.format(date) + " contains " + currentCal.numberOfHoursInThisPeriod + " hour(s)\n";

            // Enumerate all hours in this day.
            // Create a calendar to represent the following day.
            var nextDay = currentCal.clone();
            nextDay.addDays(1);
            for (currentCal.hour = currentCal.firstHourInThisPeriod; true; currentCal.addHours(1)) {
                // Display the hour for each hour in the day.             
                results = results + currentCal.hourAsPaddedString(2) + " ";

                // Break upon reaching the next period (i.e. the first period in the following day).
                if (currentCal.day === nextDay.day && currentCal.period === nextDay.period) {
                    break;
                }
            }

            results = results + "\n";
        }

        // Display results.
        document.getElementById("output").innerText = results;
    }
})();
