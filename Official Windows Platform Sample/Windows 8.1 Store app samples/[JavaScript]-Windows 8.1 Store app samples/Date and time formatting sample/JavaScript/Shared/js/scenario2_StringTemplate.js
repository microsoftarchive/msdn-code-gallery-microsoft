//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_StringTemplate.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
        // to format a date/time via a string template.  Note that the order specifed in the string pattern does
        // not determine the order of the parts of the formatted string.  The current language and region value will
        // determine the pattern of the date returned based on the specified parts. 

        // Get the current application language
        var currentLanguage = Windows.Globalization.ApplicationLanguages.languages[0];

        // Formatters for dates.
        var mydatefmt1 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month day");
        var mydatefmt2 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month year");
        var mydatefmt3 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month day year");
        var mydatefmt4 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month day dayofweek year");
        var mydatefmt5 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("dayofweek.abbreviated");
        var mydatefmt6 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("month.abbreviated");
        var mydatefmt7 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("year.abbreviated");

        // Formatters for times.
        var mytimefmt1 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("hour minute");
        var mytimefmt2 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("hour minute second");
        var mytimefmt3 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("hour");
                
        // Formatters for timezone.
        var mytzfmt1 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("timezone");
        var mytzfmt2 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("timezone.full");
        var mytzfmt3 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("timezone.abbreviated");
                
        // Formatters for combinations.
        var mycombofmt1 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("hour minute second timezone.full");
        var mycombofmt2 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("day month year hour minute timezone");
        var mycombofmt3 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("dayofweek day month year hour minute second");
        var mycombofmt4 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("dayofweek.abbreviated day month hour minute");
        var mycombofmt5 = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("dayofweek day month year hour minute second timezone.abbreviated");

        // Obtain the date that will be formatted.
        var dateToFormat = new Date();

        // Perform the actual formatting.
        var mydate1 = mydatefmt1.format(dateToFormat);
        var mydate2 = mydatefmt2.format(dateToFormat);
        var mydate3 = mydatefmt3.format(dateToFormat);
        var mydate4 = mydatefmt4.format(dateToFormat);
        var mydate5 = mydatefmt5.format(dateToFormat);
        var mydate6 = mydatefmt6.format(dateToFormat);
        var mydate7 = mydatefmt7.format(dateToFormat);
        var mytime1 = mytimefmt1.format(dateToFormat);
        var mytime2 = mytimefmt2.format(dateToFormat);
        var mytime3 = mytimefmt3.format(dateToFormat);
        var mytz1 = mytzfmt1.format(dateToFormat);
        var mytz2 = mytzfmt2.format(dateToFormat);
        var mytz3 = mytzfmt3.format(dateToFormat);
        var mycombo1 = mycombofmt1.format(dateToFormat);
        var mycombo2 = mycombofmt2.format(dateToFormat);
        var mycombo3 = mycombofmt3.format(dateToFormat);
        var mycombo4 = mycombofmt4.format(dateToFormat);
        var mycombo5 = mycombofmt5.format(dateToFormat);

        // Display the results.
        var results = "Current application context language:   " + currentLanguage + "\n\n" +
                      "Formatted Dates:\n" +
                      mydatefmt1.template + ":   " + mydate1 + "\n" +
                      mydatefmt2.template + ":   " + mydate2 + "\n" +
                      mydatefmt3.template + ":   " + mydate3 + "\n" +
                      mydatefmt4.template + ":   " + mydate4 + "\n" +
                      mydatefmt5.template + ":   " + mydate5 + "\n" +
                      mydatefmt6.template + ":   " + mydate6 + "\n" +
                      mydatefmt7.template + ":   " + mydate7 + "\n\n" +
                      "Formatted Times:\n" +
                      mytimefmt1.template + ":   " + mytime1 + "\n" +
                      mytimefmt2.template + ":   " + mytime2 + "\n" +
                      mytimefmt3.template + ":   " + mytime3 + "\n\n" +
                      "Formatted timezones:\n" +
                      mytzfmt1.template + ":   " + mytz1 + "\n" +
                      mytzfmt2.template + ":   " + mytz2 + "\n" +
                      mytzfmt3.template + ":   " + mytz3 + "\n\n" +
                      "Formatted Date and Time Combinations:\n" +
                      mycombofmt1.template + ":   " + mycombo1 + "\n" +
                      mycombofmt2.template + ":   " + mycombo2 + "\n" +
                      mycombofmt3.template + ":   " + mycombo3 + "\n" +
                      mycombofmt4.template + ":   " + mycombo4 + "\n" +
                      mycombofmt5.template + ":   " + mycombo5 + "\n\n";

        document.getElementById("output").innerText = results;
    }
})();
