//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var cachedFileUpdaterUI;
    var fileUpdateRequest;
    var fileUpdateRequestDeferral;

    var page = WinJS.UI.Pages.define("/html/cachedFileUpdaterScenario1.html", {
        processed: function (element, options) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Do any initialization work that is required to set up the initial UI.
        },
        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.

            cachedFileUpdaterUI = options.cachedFileUpdaterUI;
            fileUpdateRequest = options.fileUpdateRequest;
            fileUpdateRequestDeferral = options.fileUpdateRequestDeferral;

            document.getElementById("fileIsCurrentButton").addEventListener("click", onFileIsCurrent, false);
            document.getElementById("provideUpdatedVersionButton").addEventListener("click", onProvideUpdatedVersion, false);
        }
    });

    function onFileIsCurrent() {
        printFileAsync(fileUpdateRequest.file);
        fileUpdateRequest.status = Windows.Storage.Provider.FileUpdateStatus.complete;
        fileUpdateRequestDeferral.complete();
        updateUI(cachedFileUpdaterUI.uiStatus);
    };

    function onProvideUpdatedVersion() {
        var now = Windows.Globalization.DateTimeFormatting.DateTimeFormatter.longTime.format(new Date());
        Windows.Storage.FileIO.appendTextAsync(fileUpdateRequest.file, "\n" + "New content added @ " + now).then(function () {
            printFileAsync(fileUpdateRequest.file);
            fileUpdateRequest.status = Windows.Storage.Provider.FileUpdateStatus.complete;
            fileUpdateRequestDeferral.complete();
            updateUI(cachedFileUpdaterUI.uiStatus);
        }, WinJS.log);
    };

    function updateUI(uiStatus) {
        if (uiStatus === Windows.Storage.Provider.UIStatus.complete) {
            fileIsCurrentButton.disabled = true;
            provideUpdatedVersionButton.disabled = true;
        }
    }

    function printFileAsync(file) {
        Windows.Storage.FileIO.readTextAsync(file).then(function (fileContent) {
            WinJS.log && WinJS.log("Recieved File: " + file.name + "\n" + "File Content: " + fileContent, "sample", "status");
        }, WinJS.log);
    }
})();
