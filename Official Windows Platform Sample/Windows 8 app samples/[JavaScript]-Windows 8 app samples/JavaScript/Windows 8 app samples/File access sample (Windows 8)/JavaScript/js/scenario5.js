//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        ready: function (element, options) {
            document.getElementById("showProperties").addEventListener("click", showProperties, false);
            // To test if "sample.dat" is created.
            SdkSample.validateFileExistence();
        }
    });

    var dateAccessedProperty = "System.DateAccessed";
    var fileOwnerProperty    = "System.FileOwner";

    function showProperties() {
        if (SdkSample.sampleFile !== null) {
            var outputDiv = document.getElementById("output");
            // Get top level file properties
            outputDiv.innerHTML = "Filename: " + SdkSample.sampleFile.name + "<br />";
            outputDiv.innerHTML += "File type: " + SdkSample.sampleFile.fileType + "<br />";

            // Get basic properties
            SdkSample.sampleFile.getBasicPropertiesAsync().then(function (basicProperties) {
                outputDiv.innerHTML += "Size: " + basicProperties.size + " bytes<br />";
                outputDiv.innerHTML += "Date modified: " + basicProperties.dateModified + "<br />";

                // Get extra properties
                return SdkSample.sampleFile.properties.retrievePropertiesAsync([fileOwnerProperty, dateAccessedProperty]);
            }).done(function (extraProperties) {
                var propValue = extraProperties[dateAccessedProperty];
                if (propValue !== null) {
                    outputDiv.innerHTML += "Date accessed: " + propValue + "<br />";
                }
                propValue = extraProperties[fileOwnerProperty];
                if (propValue !== null) {
                    outputDiv.innerHTML += "File owner: " + propValue;
                }
            },
            function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
        }
    }
})();
