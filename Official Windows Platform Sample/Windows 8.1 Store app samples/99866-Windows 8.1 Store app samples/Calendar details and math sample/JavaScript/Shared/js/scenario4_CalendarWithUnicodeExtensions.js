//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4_CalendarWithUnicodeExtensions.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.Calendar class to display the parts of a date.        

        // Create Calendar objects using different Unicode extensions for different languages.
        // NOTE: Calendar (ca) and numeral system (nu) are the only supported extensions with any others being ignored (note that collation (co) extension is ignored in the last example).
        // Create a calendar based on the current user.
        var cal1 = new Windows.Globalization.Calendar();
        // Create a calendar using Arabic language with Gregorian Calendar and Latin Numeral System extensions.
        var cal2 = new Windows.Globalization.Calendar(["ar-SA-u-ca-gregory-nu-Latn"]);
        // Create a calendar using Hebrew language with Arabic Numeral System extension.
        var cal3 = new Windows.Globalization.Calendar(["he-IL-u-nu-arab"]);
        // Create a calendar using Hebrew language with Hebrew Calendar, default Numeral System for the language and Phonebook Collation extensions.
        var cal4 = new Windows.Globalization.Calendar(["he-IL-u-ca-hebrew-co-phonebk"]);
        
        // Obtain the date parts using the default calendar system.
        var calItems1 = "User's default calendar object:\n" +
                        getCalendarProperties(cal1);

        // Obtain the date parts for the second calendar.
        var calItems2 = "Calendar object with Arabic language, Gregorian Calendar and Latin Numeral System (ar-SA-ca-gregory-nu-Latn) :\n" +
                        getCalendarProperties(cal2);

        // Obtain the date parts for the third calendar.
        var calItems3 = "Calendar object with Hebrew language, Default Calendar for that language and Arab Numeral System (he-IL-u-nu-arab) :\n" +
                        getCalendarProperties(cal3);

        // Obtain the date parts for the fourth calendar.
        var calItems4 = "Calendar object with Hebrew language, Hebrew Calendar, Default Numeral System for that language and Phonebook collation (he-IL-u-ca-hebrew-co-phonebk) :\n" +
                        getCalendarProperties(cal4);

        // Display the result.
        var results = calItems1 + "\n\n" + calItems2 + "\n\n" + calItems3 + "\n\n" + calItems4;

        document.getElementById("output").innerText = results;
    }

    /// This is a helper function to display calendar's properties in presentable format
    function getCalendarProperties(calendar) {
        var returnString = "Calendar system: " + calendar.getCalendarSystem() + "\n" +
                           "Numeral system: " + calendar.numeralSystem + "\n" +
                           "Resolved Language: " + calendar.resolvedLanguage + "\n" +
                           "Name of Month: " + calendar.monthAsString() + "\n" +
                           "Day of Month: " + calendar.dayAsPaddedString(2) + "\n" +
                           "Day of Week: " + calendar.dayOfWeekAsString() + "\n" +
                           "Year: " + calendar.yearAsString();
        return returnString;
    }

})();
