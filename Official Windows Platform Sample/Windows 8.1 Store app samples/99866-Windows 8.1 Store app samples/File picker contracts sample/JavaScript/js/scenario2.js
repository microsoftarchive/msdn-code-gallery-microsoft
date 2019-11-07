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
            document.getElementById("saveButton").addEventListener("click", launchSavePicker, false);
        }
    });

    function launchSavePicker() {
        var outputDiv = document.getElementById("output");
        outputDiv.innerHTML = "";

        // Set up the picker
        var picker = Windows.Storage.Pickers.FileSavePicker();
        picker.fileTypeChoices.insert("PNG", [".png"]);

        // Launch the picker in save mode
        picker.pickSaveFileAsync().then(function (file) {
            if (file) {
                // At this point, the app can write to the provided file
                outputDiv.innerHTML += file.name + " was saved. <br />";
            }
        });
    };
})();
