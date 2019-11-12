//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("writeText").addEventListener("click", writeText, false);
            document.getElementById("readText").addEventListener("click", readText, false);
            // To test if "sample.dat" is created.
            SdkSample.validateFileExistence();
        }
    });

    // Writes some text to 'sample.dat'
    function writeText() {
        if (SdkSample.sampleFile !== null) {
            var textArea = document.getElementById("textarea");
            var userContent = textArea.innerText;
            var outputDiv = document.getElementById("output");
            if (userContent !== "") {
                Windows.Storage.FileIO.writeTextAsync(SdkSample.sampleFile, userContent).done(function () {
                    outputDiv.innerHTML = "The following text was written to '" + SdkSample.sampleFile.name + "':<br /><br />" + userContent;
                },
                function (error) {
                    WinJS.log && WinJS.log(error, "sample", "error");
                });
            } else {
                outputDiv.innerHTML = "The text box is empty, please write something and then click 'Write' again.";
            }
        }
    }

    // Reads text from 'sample.dat'
    function readText() {
        if (SdkSample.sampleFile !== null) {
            Windows.Storage.FileIO.readTextAsync(SdkSample.sampleFile).done(function (fileContent) {
                var outputDiv = document.getElementById("output");
                outputDiv.innerHTML = "The following text was read from '" + SdkSample.sampleFile.name + "':<br /><br />" + fileContent;
            },
            function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
        }
    }
})();
