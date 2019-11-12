//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3_ParametrizedTemplate.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
        // to format a date/time by specifying a template via parameters.  Note that the current language
        // and region value will determine the pattern of the date returned based on the
        // specified parts.

        // Get the current application language
        var currentLanguage = Windows.Globalization.ApplicationLanguages.languages[0];

        // Namespace declaration for easy creation of the template.
        var dateTimeFormatting = Windows.Globalization.DateTimeFormatting;

        // Formatters for dates.
        var mydatefmt1 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.YearFormat.full,
            dateTimeFormatting.MonthFormat.abbreviated,
            dateTimeFormatting.DayFormat.default,
            dateTimeFormatting.DayOfWeekFormat.abbreviated);

        var mydatefmt2 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.YearFormat.abbreviated,
            dateTimeFormatting.MonthFormat.abbreviated,
            dateTimeFormatting.DayFormat.default,
            dateTimeFormatting.DayOfWeekFormat.none);

        var mydatefmt3 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.YearFormat.full,
            dateTimeFormatting.MonthFormat.full,
            dateTimeFormatting.DayFormat.none,
            dateTimeFormatting.DayOfWeekFormat.none);

        var mydatefmt4 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.YearFormat.none,
            dateTimeFormatting.MonthFormat.full,
            dateTimeFormatting.DayFormat.default,
            dateTimeFormatting.DayOfWeekFormat.none);

        // Formatters for times.
        var mytimefmt1 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.HourFormat.default,
            dateTimeFormatting.MinuteFormat.default,
            dateTimeFormatting.SecondFormat.default);

        var mytimefmt2 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.HourFormat.default,
            dateTimeFormatting.MinuteFormat.default,
            dateTimeFormatting.SecondFormat.none);

        var mytimefmt3 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.HourFormat.default,
            dateTimeFormatting.MinuteFormat.none,
            dateTimeFormatting.SecondFormat.none);

        // Obtain the date that will be formatted.
        var dateToFormat = new Date();

        // Perform the actual formatting.
        var mydate1 = mydatefmt1.format(dateToFormat);
        var mydate2 = mydatefmt2.format(dateToFormat);
        var mydate3 = mydatefmt3.format(dateToFormat);
        var mydate4 = mydatefmt4.format(dateToFormat);
        var mytime1 = mytimefmt1.format(dateToFormat);
        var mytime2 = mytimefmt2.format(dateToFormat);
        var mytime3 = mytimefmt3.format(dateToFormat);

        // Display the results.
        var results = "Current application context language: " + currentLanguage + "\n\n" +
                      "Formatted Dates:\n\n" +
                      mydatefmt1.template + ": " + mydate1 + "\n" +
                      mydatefmt2.template + ": " + mydate2 + "\n" +
                      mydatefmt3.template + ": " + mydate3 + "\n" +
                      mydatefmt4.template + ": " + mydate4 + "\n\n" +
                      "Formatted Times:\n\n" +
                      mytimefmt1.template + ": " + mytime1 + "\n" +
                      mytimefmt2.template + ": " + mytime2 + "\n" +
                      mytimefmt3.template + ": " + mytime3 + "\n";

        document.getElementById("output").innerText = results;
    }
})();
