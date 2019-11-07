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

    var page = WinJS.UI.Pages.define("/html/cachedFileUpdaterScenario2.html", {
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

            document.getElementById("overwriteButton").addEventListener("click", onOverwrit, false);
            document.getElementById("RenameButton").addEventListener("click", onRename, false);
        }
    });

    function onOverwrit() {

        // update the remote version of file...
        // Printing the file content
        printFileAsync(fileUpdateRequest.file);
        fileUpdateRequest.status = Windows.Storage.Provider.FileUpdateStatus.complete;
        fileUpdateRequestDeferral.complete();
        updateUI(cachedFileUpdaterUI.uiStatus);
    };

    function onRename() {
        fileUpdateRequest.file.copyAsync(Windows.Storage.ApplicationData.current.localFolder, fileUpdateRequest.file.name, Windows.Storage.CreationCollisionOption.generateUniqueName).then(function (file) {
            Windows.Storage.Provider.CachedFileUpdater.setUpdateInformation(file, "CachedFile", Windows.Storage.Provider.ReadActivationMode.notNeeded, Windows.Storage.Provider.WriteActivationMode.afterWrite, Windows.Storage.Provider.CachedFileOptions.requireUpdateOnAccess);
            fileUpdateRequest.updateLocalFile(file);
            printFileAsync(file);
            fileUpdateRequest.status = Windows.Storage.Provider.FileUpdateStatus.completeAndRenamed;
            fileUpdateRequestDeferral.complete();
            updateUI(cachedFileUpdaterUI.uiStatus);
        }, WinJS.log);
    }

    function updateUI(uiStatus) {
        if (uiStatus === Windows.Storage.Provider.UIStatus.complete) {
            overwriteButton.disabled = true;
            RenameButton.disabled = true;
        }
    }

    function printFileAsync(file) {
        Windows.Storage.FileIO.readTextAsync(file).then(function (fileContent) {
            WinJS.log && WinJS.log("File Name: " + file.name + "\n" + "File Content:" + "\n" + fileContent, "sample", "status");
        }, WinJS.log);
    }
})();
