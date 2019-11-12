//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/CurrentInputLanguage.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.Language class to obtain the user's current 
        // input language.  The language tag returned reflects the current input language specified 
        // by the user.
        var userInputLanguage = Windows.Globalization.Language.currentInputMethodLanguageTag;

        // Display the results
        var results = "User's current input language: " + userInputLanguage;

        WinJS.log && WinJS.log(results, "sample", "status");
    }
})();
