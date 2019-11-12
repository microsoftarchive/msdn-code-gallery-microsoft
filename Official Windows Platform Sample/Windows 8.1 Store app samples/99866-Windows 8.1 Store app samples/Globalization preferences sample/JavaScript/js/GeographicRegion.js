//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/GeographicRegion.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    function doDisplay() {
        // This scenario uses the Windows.Globalization.GeographicRegion class to
        // obtain the geographic region characteristics.
        var userGeoRegion = new Windows.Globalization.GeographicRegion();

        // This obtains the geographic region characteristics by providing a country or region code.
        var exampleGeoRegion = new Windows.Globalization.GeographicRegion("AU");

        // Display the results
        var userGeoRegionCharacteristics = "User's Preferred Geographic Region\n" +
                                           "Display Name: " + userGeoRegion.displayName + "\n" +
                                           "Native Name: " + userGeoRegion.nativeName + "\n" +
                                           "Currencies in use: " + userGeoRegion.currenciesInUse + "\n" +
                                           "Codes: " + userGeoRegion.codeTwoLetter + ", " + userGeoRegion.codeThreeLetter + ", " + userGeoRegion.codeThreeDigit;

        var exampleGeoRegionCharacteristics = "Example Geographic Region by region/country code (AU)\n" +
                                              "Display Name: " + exampleGeoRegion.displayName + "\n" +
                                              "Native Name: " + exampleGeoRegion.nativeName + "\n" +
                                              "Currencies in use: " + exampleGeoRegion.currenciesInUse + "\n" +
                                              "Codes: " + exampleGeoRegion.codeTwoLetter + ", " + exampleGeoRegion.codeThreeLetter + " , " + exampleGeoRegion.codeThreeDigit;

        var results = userGeoRegionCharacteristics + "\n\n" + exampleGeoRegionCharacteristics;

        WinJS.log && WinJS.log(results, "sample", "status");
    }
})();
