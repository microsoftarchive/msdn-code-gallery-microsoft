//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario7.html", {
        ready: function (element, options) {
            document.getElementById("copy").addEventListener("click", copy, false);
            // To test if "sample.dat" is created.
            SdkSample.validateFileExistence();
        }
    });

    // Copies 'sample.dat' to 'sample - Copy.dat'
    function copy() {
        if (SdkSample.sampleFile !== null) {
            var outputDiv = document.getElementById("output");
            SdkSample.sampleFile.copyAsync(Windows.Storage.KnownFolders.documentsLibrary, "sample - Copy.dat", Windows.Storage.NameCollisionOption.replaceExisting).done(function (sampleFileCopy) {
                outputDiv.innerHTML = "The file '" + SdkSample.sampleFile.name + "' was copied and the new file was named '" + sampleFileCopy.name + "'.";
            },
            function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
        }
    }
})();
