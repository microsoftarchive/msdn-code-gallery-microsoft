//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var fileSavePickerUI;

    var page = WinJS.UI.Pages.define("/html/fileSavePickerScenario2.html", {
        ready: function (element, options) {
            savePickerScenario1.fileSavePickerUI = null;
            fileSavePickerUI = options.fileSavePickerUI;
            //to add the event listener to fileSavePickerUI when the page is loaded
            fileSavePickerUI.addEventListener("targetfilerequested", onTargetFileRequestedFail, false);
        },
        unload: function () {
            //to remove the event listener from fileSavePickerUI when the page is unloaded
            fileSavePickerUI.removeEventListener("targetfilerequested", onTargetFileRequestedFail, false);
        }
    });

    function onTargetFileRequestedFail(e) {
        var deferral;
        // Request a deferral so that the app can call another asynchronous method and complete the request at a later time
        deferral = e.request.getDeferral();

        // Display a dialog indicating to the user that a corrective action needs to occur
        var messageDialog = new Windows.UI.Popups.MessageDialog("If the app needs the user to correct a problem before the app can save the file, the app can use a message like this to tell the user about the problem and how to correct it.");
        messageDialog.showAsync().done(function () {
            // Set the targetFile property to null and complete the deferral to indicate failure once the user has closed the
            // dialog.  This will allow the user to take any neccessary corrective action and click the Save button once again.
            e.request.targetFile = null;
            deferral.complete();
        });
    };

    WinJS.Namespace.define("savePickerScenario2", {
        fileSavePickerUI: fileSavePickerUI
    });
})();
