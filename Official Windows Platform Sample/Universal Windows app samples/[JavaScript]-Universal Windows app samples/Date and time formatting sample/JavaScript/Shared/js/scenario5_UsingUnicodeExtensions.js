//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5_UsingUnicodeExtensions.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
        // to format a date/time by using a formatter that uses unicode extenstion in the specified
        // language name

        // Namespace declaration for easy creation of the template.
        var dateTimeFormatting = Windows.Globalization.DateTimeFormatting;

        // Get the current application language
        var currentLanguage = Windows.Globalization.ApplicationLanguages.languages[0];        
        // We keep results in this variable
        var results = "Current default application context language: " + currentLanguage + "\n\n";

        // Create formatters using various types of constructors specifying Language list with unicode extension in language names
        // Default application context 
        var mydatefmt1 = new dateTimeFormatting.DateTimeFormatter("longdate longtime");
        // Telugu language, Gregorian Calendar and Latin Numeral System
        var mydatefmt2 = new dateTimeFormatting.DateTimeFormatter("longdate longtime", ["te-in-u-ca-gregory-nu-latn", "en-US"]);
        // Hebrew language and Arabic Numeral System - calendar NOT specified in constructor
        var mydatefmt3 = new dateTimeFormatting.DateTimeFormatter(
                                dateTimeFormatting.YearFormat.default,
                                dateTimeFormatting.MonthFormat.default,
                                dateTimeFormatting.DayFormat.default,
                                dateTimeFormatting.DayOfWeekFormat.default,
                                dateTimeFormatting.HourFormat.default,
                                dateTimeFormatting.MinuteFormat.default,
                                dateTimeFormatting.SecondFormat.default,
                                ["he-IL-u-nu-arab", "en-US"]);
        // Hebrew language and calendar - calendar specified in constructor
        // also, which overrides the one specified in Unicode extension
        var mydatefmt4 = new dateTimeFormatting.DateTimeFormatter(
                                dateTimeFormatting.YearFormat.default,
                                dateTimeFormatting.MonthFormat.default,
                                dateTimeFormatting.DayFormat.default,
                                dateTimeFormatting.DayOfWeekFormat.default,
                                dateTimeFormatting.HourFormat.default,
                                dateTimeFormatting.MinuteFormat.default,
                                dateTimeFormatting.SecondFormat.default,
                                ["he-IL-u-ca-hebrew-co-phonebk", "en-US"],
                                "US",
                                Windows.Globalization.CalendarIdentifiers.gregorian,
                                Windows.Globalization.ClockIdentifiers.twentyFourHour);

        // Create an array of the formatter objects
        var formatterArray = [mydatefmt1, mydatefmt2, mydatefmt3, mydatefmt4];

        // Obtain the date that will be formatted.
        var dateToFormat = new Date();

        // Format and display date/time along with other relevant properites for each formatter object    
        for (var i in formatterArray) {
            results += "Using DateTimeFormatter with Language List:   " + formatterArray[i].languages.join(", ") + "\n";
            results += "     Template:   " + formatterArray[i].template + "\n";
            results += "     Resolved Language:   " + formatterArray[i].resolvedLanguage + "\n";
            results += "     Calendar System:   " + formatterArray[i].calendar + "\n";
            results += "     Numeral System:   " + formatterArray[i].numeralSystem + "\n";
            results += "Formatted DateTime:   " + formatterArray[i].format(dateToFormat) + "\n\n";
        }

        // Display the results.
        document.getElementById("output").innerText = results;
    }
})();
