//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/add.html", {
        ready: function (element, options) {
            document.getElementById("PickPlaylistButton").addEventListener("click", loadPlaylist, false);
            document.getElementById("PickAudioButton").addEventListener("click", addSong, false);
        }
    });

    function loadPlaylist() {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;
        picker.fileTypeFilter.replaceAll(SdkSample.playlistExtensions);

        picker.pickSingleFileAsync()
            .then(function (file) {
                if (file) {
                    return Windows.Media.Playlists.Playlist.loadAsync(file);
                }
                return WinJS.Promise.wrapError("No file picked");
            }, function (error) {
                WinJS.log && WinJS.log("Error in picking file.", "sample", "error");
            })
            .done(function (playlist) {
                SdkSample.playlist = playlist;
                WinJS.log && WinJS.log("Playlist loaded.", "sample", "status");
            }, function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
    }

    function addSong() {
        if (SdkSample.playlist) {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;
            picker.fileTypeFilter.replaceAll(SdkSample.audioExtensions);

            var numFilesPicked = 0;
            picker.pickMultipleFilesAsync()
                .then(function (files) {
                    if (files.size > 0) {
                        numFilesPicked = files.size;

                        files.forEach(function (file) {
                            SdkSample.playlist.files.append(file);
                        });

                        return SdkSample.playlist.saveAsync();
                    }
                    else {
                        return WinJS.Promise.wrapError("No files picked.");
                    }
                })
                .done(function (file) {
                    WinJS.log && WinJS.log(numFilesPicked + " files added to playlist.", "sample", "status");
                }, function (error) {
                    WinJS.log && WinJS.log(error, "sample", "error");
                });
        }
        else {
            WinJS.log && WinJS.log("Pick playlist first.", "sample", "error");
        }
    }
})();
