//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("createFile").addEventListener("click", createFile, false);
        }
    });

    function createFile() {
        Windows.Storage.KnownFolders.documentsLibrary.createFileAsync("sample.dat", Windows.Storage.CreationCollisionOption.replaceExisting).done(
        function (file) {
            SdkSample.sampleFile = file;
            var outputDiv = document.getElementById("output");
            outputDiv.innerHTML = "The file '" + SdkSample.sampleFile.name + "' was created.";
        },
        function (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
        });
    }
})();
