//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_LongAndShortFormats.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
        // in order to display dates and times using basic formatters.  Note that the pattern of 
        // the date returned is determined by the current language and region value of the application context

        // Get the current application language
        var currentLanguage = Windows.Globalization.ApplicationLanguages.languages[0];

        // Formatters for dates.
        var sdatefmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shortdate");
        var ldatefmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longdate");

        // Formatters for times.
        var stimefmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("shorttime");
        var ltimefmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");

        // Obtain the date that will be formatted.
        var dateToFormat = new Date();

        // Display the results.
        var results = "Current application context language: " + currentLanguage + "\n\n";

        // Perform the actual formatting.
        results = results + sdatefmt.template + ": " + sdatefmt.format(dateToFormat) + "\n";
        results = results + ldatefmt.template + ": " + ldatefmt.format(dateToFormat) + "\n";
        results = results + stimefmt.template + ": " + stimefmt.format(dateToFormat) + "\n";
        results = results + ltimefmt.template + ": " + ltimefmt.format(dateToFormat) + "\n";

        document.getElementById("output").innerText = results;
    }
})();
