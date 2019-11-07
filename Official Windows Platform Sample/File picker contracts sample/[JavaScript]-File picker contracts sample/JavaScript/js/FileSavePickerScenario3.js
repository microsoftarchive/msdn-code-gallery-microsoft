//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var fileSavePickerUI;

    var page = WinJS.UI.Pages.define("/html/fileSavePickerScenario3.html", {
        processed: function (element, options) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Do any initialization work that is required to set up the initial UI.
        },
        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.

            fileSavePickerUI = options.fileSavePickerUI;
            //to add the event listener to fileSavePickerUI when the page is loaded
            fileSavePickerUI.addEventListener("targetfilerequested", onTargetFileRequested, false);
        },
        unload: function () {
            //to remove the event listener from fileSavePickerUI when the page is unloaded
            fileSavePickerUI.removeEventListener("targetfilerequested", onTargetFileRequested, false);
        }
    });

    function onTargetFileRequested(e) {
        var deferral;
        deferral = e.request.getDeferral();

        // Create a file to provide back to the Picker
        Windows.Storage.ApplicationData.current.localFolder.createFileAsync(fileSavePickerUI.fileName, Windows.Storage.CreationCollisionOption.replaceExisting).done(function (file) {
        Windows.Storage.Provider.CachedFileUpdater.setUpdateInformation(file, "CachedFile", Windows.Storage.Provider.ReadActivationMode.notNeeded, Windows.Storage.Provider.WriteActivationMode.afterWrite, Windows.Storage.Provider.CachedFileOptions.requireUpdateOnAccess);

            // Assign the resulting file to the targetFile property and complete the deferral to indicate success
            e.request.targetFile = file;
            deferral.complete();
        }, function () {
            // Set the targetFile property to null and complete the deferral to indicate failure
            e.request.targetFile = null;
            deferral.complete();
        });
    };

    WinJS.Namespace.define("savePickerScenario1", {
        fileSavePickerUI: fileSavePickerUI
    });
})();
