//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4_OverrideSettings.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
        // to format a date/time by using a formatter that provides specific languages,
        // geographic region, calendar and clock

        // Get the current application language
        var currentLanguage = Windows.Globalization.ApplicationLanguages.languages[0];

        // Namespace declaration for easy creation of the template.
        var dateTimeFormatting = Windows.Globalization.DateTimeFormatting;

        // Clock and calendar identifiers
        var calendarIdentifiers = Windows.Globalization.CalendarIdentifiers;
        var clockIdentifiers = Windows.Globalization.ClockIdentifiers;

        // Formatters for dates, using basic formatters, string templates and parametrized templates.

        var ldatefmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longdate", ["ja-JP"], "JP", calendarIdentifiers.japanese, clockIdentifiers.twelveHour);
        var mydatefmt1 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month day", ["fr-FR"], "FR", calendarIdentifiers.gregorian, clockIdentifiers.twentyFourHour);
        var mydatefmt2 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.YearFormat.abbreviated,
            dateTimeFormatting.MonthFormat.abbreviated,
            dateTimeFormatting.DayFormat.default,
            dateTimeFormatting.DayOfWeekFormat.none,
            dateTimeFormatting.HourFormat.none,
            dateTimeFormatting.MinuteFormat.none,
            dateTimeFormatting.SecondFormat.none,
            ["de-DE"], "DE",
            calendarIdentifiers.gregorian, clockIdentifiers.twelveHour);

        // Formatters for times, using basic formatters, string templates and parametrized templates.

        var ltimefmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime", ["ja-JP"], "JP", calendarIdentifiers.japanese, clockIdentifiers.twelveHour);
        var mytimefmt1 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("hour minute", ["fr-FR"], "FR", calendarIdentifiers.gregorian, clockIdentifiers.twentyFourHour);
        var mytimefmt2 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
            dateTimeFormatting.YearFormat.none,
            dateTimeFormatting.MonthFormat.none,
            dateTimeFormatting.DayFormat.none,
            dateTimeFormatting.DayOfWeekFormat.none,
            dateTimeFormatting.HourFormat.default,
            dateTimeFormatting.MinuteFormat.default,
            dateTimeFormatting.SecondFormat.none,
            ["de-DE"], "DE",
            calendarIdentifiers.gregorian, clockIdentifiers.twelveHour);

        // Obtain the date that will be formatted.
        var dateToFormat = new Date();

        // Perform the actual formatting.
        var ldate = ldatefmt.format(dateToFormat);
        var mydate1 = mydatefmt1.format(dateToFormat);
        var mydate2 = mydatefmt2.format(dateToFormat);
        var ltime = ltimefmt.format(dateToFormat);
        var mytime1 = mytimefmt1.format(dateToFormat);
        var mytime2 = mytimefmt2.format(dateToFormat);

        // Display the results.
        var results = "Current application context language: " + currentLanguage + "\n\n" +
                      ldatefmt.template + ": (" + ldatefmt.resolvedLanguage + ") " + ldate + "\n" +
                      mydatefmt1.template + ": (" + mydatefmt1.resolvedLanguage + ") " + mydate1 + "\n" +
                      mydatefmt2.template + ": (" + mydatefmt2.resolvedLanguage + ") " + mydate2 + "\n" +
                      ltimefmt.template + ": (" + ltimefmt.resolvedLanguage + ") " + ltime + "\n" +
                      mytimefmt1.template + ": (" + mytimefmt1.resolvedLanguage + ") " + mytime1 + "\n" +
                      mytimefmt2.template + ": (" + mytimefmt2.resolvedLanguage + ") " + mytime2 + "\n";

        document.getElementById("output").innerText = results;
    }
})();
