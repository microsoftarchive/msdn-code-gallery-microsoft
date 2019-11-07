//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function pickMediaFile(filters, videos, oneTimeOnly) {
        var Pickers = Windows.Storage.Pickers,
            picker = new Pickers.FileOpenPicker();

        picker.suggestedStartLocation = Pickers.PickerLocationId.videosLibrary;
        picker.fileTypeFilter.replaceAll(filters);
        picker.pickSingleFileAsync().done(
            function (file) {
                if (file) {
                    videos.forEach(function (video, i) {
                        video.src = URL.createObjectURL(file, { oneTimeOnly: oneTimeOnly });
                    });
                }
            },
            function (e) {
                SdkSample.displayError("Error while selecting a file: " + e.message);
            });
    }

    WinJS.Namespace.define("SdkSample", {
        pickMediaFile: pickMediaFile
    });
})();
