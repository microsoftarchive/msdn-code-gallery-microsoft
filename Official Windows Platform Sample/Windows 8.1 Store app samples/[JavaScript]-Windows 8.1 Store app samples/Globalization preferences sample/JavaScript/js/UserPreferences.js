//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/UserPreferences.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.System.UserProfile.GlobalizationPreferences class to
        // obtain the user's globalization preferences.
        var userLanguages = Windows.System.UserProfile.GlobalizationPreferences.languages;
        var userCalendar = Windows.System.UserProfile.GlobalizationPreferences.calendars;
        var userClock = Windows.System.UserProfile.GlobalizationPreferences.clocks;
        var userHomeRegion = Windows.System.UserProfile.GlobalizationPreferences.homeGeographicRegion;
        var userFirstDayOfWeek = Windows.System.UserProfile.GlobalizationPreferences.weekStartsOn;

        // Display the results.
        var results = "Languages: " + userLanguages + "\n" +
                      "Home Region: " + userHomeRegion + "\n" +
                      "Calendar System: " + userCalendar + "\n" +
                      "Clock: " + userClock + "\n" +
                      "First Day of the Week: " + userFirstDayOfWeek;

        WinJS.log && WinJS.log(results, "sample", "status");
    }
})();
