//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/create.html", {
        ready: function (element, options) {
            document.getElementById("PickAudioButton").addEventListener("click", pickAudio, false);
        }
    });

    function pickAudio() {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;
        picker.fileTypeFilter.replaceAll(SdkSample.audioExtensions);

        picker.pickMultipleFilesAsync()
            .done(function (files) {
                if (files.size > 0) {
                    SdkSample.playlist = new Windows.Media.Playlists.Playlist();

                    files.forEach(function (file) {
                        SdkSample.playlist.files.append(file);
                    });

                    SdkSample.playlist.saveAsAsync(Windows.Storage.KnownFolders.musicLibrary,
                                                   "Sample",
                                                   Windows.Storage.NameCollisionOption.replaceExisting,
                                                   Windows.Media.Playlists.PlaylistFormat.windowsMedia)
                        .done(function (file) {
                            WinJS.log && WinJS.log(file.name + " was created and saved with " + SdkSample.playlist.files.size + " files.", "sample", "status");

                            // Reset playlist so subsequent SaveAsync calls don't fail
                            SdkSample.playlist = null;
                        }, function (error) {
                            WinJS.log && WinJS.log(error, "sample", "error");
                        });
                }
                else {
                    WinJS.log && WinJS.log("No files picked.", "sample", "error");
                }
            }, function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
    }
})();
