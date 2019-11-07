//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario8.html", {
        ready: function (element, options) {
            document.getElementById("delete").addEventListener("click", deleteFile, false);
            // To test if "sample.dat" is created.
            SdkSample.validateFileExistence();
        }
    });

    // Deletes 'sample.dat'
    function deleteFile() {
        if (SdkSample.sampleFile !== null) {
            var outputDiv = document.getElementById("output");
            SdkSample.sampleFile.deleteAsync().done(function () {
                SdkSample.sampleFile = null;
                outputDiv.innerHTML = "The file 'sample.dat' was deleted.";
            },
            function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
        }
    }
})();
