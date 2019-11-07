//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        processed: function (element, options) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Do any initialization work that is required to set up the initial UI.
        },
        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.
            document.getElementById("openPicker").addEventListener("click", launchOpenPicker, false);
        }
    });

    function launchOpenPicker() {
        var outputDiv = document.getElementById("output");
        outputDiv.innerHTML = "";

        // Set up the picker
        var picker = Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.replaceAll([".png"]);
        picker.viewMode = Windows.Storage.Pickers.PickerViewMode.thumbnail;

        // Launch the picker in open mode
        picker.pickMultipleFilesAsync().then(function (files) {
            files.forEach(function (file) {
                // At this point, this app can read from the selected file
                outputDiv.innerHTML += file.name + " was picked. <br />";
            });
        });
    };
})();
