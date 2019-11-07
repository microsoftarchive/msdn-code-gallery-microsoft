//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Language.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.System.UserProfile.GlobalizationPreferences class to
        // obtain the user's preferred language characteristics.
        var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.languages[0];
        var userLanguage = new Windows.Globalization.Language(topUserLanguage);

        // This obtains the language characteristics by providing a BCP47 tag.
        var exampleLanguage = new Windows.Globalization.Language("en-AU");

        // Display the results
        var userLanguageCharacteristics = "User's Preferred Language\n" +
                                          "Display Name: " + userLanguage.displayName + "\n" +
                                          "Language Tag: " + userLanguage.languageTag + "\n" +
                                          "Native Name: " + userLanguage.nativeName + "\n" +
                                          "Script Code: " + userLanguage.script;

        var exampleLanguageCharacteristics = "Example Language by BCP47 tag (en-AU)\n" +
                                             "Display Name: " + exampleLanguage.displayName + "\n" +
                                             "Language Tag: " + exampleLanguage.languageTag + "\n" +
                                             "Native Name: " + exampleLanguage.nativeName + "\n" +
                                             "Script Code: " + exampleLanguage.script;

        var results = userLanguageCharacteristics + "\n\n" + exampleLanguageCharacteristics;

        WinJS.log && WinJS.log(results, "sample", "status");
    }
})();
