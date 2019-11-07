//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/PostMessageBlob.html", {
        ready: function (element, options) {
            document.getElementById("load").addEventListener("click", /*@static_cast(EventListener)*/postMessageBlob, false);
            reset();
        },
        
        unload: function () {
            reset();
        }
    });

    function postMessageBlob() {
        // Verify that we are currently not snapped, or that we can unsnap to open the picker
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        if (currentState === Windows.UI.ViewManagement.ApplicationViewState.snapped && !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
            // fail gracefully if we can't unsnap
            WinJS.log && WinJS.Log("This functionality requires the app to not be snapped", "sample", "status");
            return;
        }

        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.fileTypeFilter.replaceAll([".jpg", ".bmp", ".gif", ".png"]);
        picker.pickSingleFileAsync().then(function (file) {
            // Check that the picker returned a file. The picker will return null if the user clicked cancel
            if (file) {
                file.openAsync(Windows.Storage.FileAccessMode.read).done(function (stream) {
                    // The blob takes ownership of stream and manages lifetime. The stream will be closed when the blob is closed.
                    var blob = MSApp.createBlobFromRandomAccessStream(file.contentType, stream);
                    document.getElementById("webFrame").contentWindow.postMessage(blob, "ms-appx-web://" + document.location.hostname);
                    blob.msClose();
            
                }, BlobSample.asyncError);
            }
            else {
                WinJS.log && WinJS.log("No file was selected", "sample", "status");
            }
        }, BlobSample.asyncError);
    }

    function reset() {
        document.getElementById("webFrame").contentWindow.postMessage("reset", "ms-appx-web://" + document.location.hostname);
    }
})();
