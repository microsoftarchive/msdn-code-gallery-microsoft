//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/clear.html", {
        ready: function (element, options) {
            document.getElementById("PickPlaylistButton").addEventListener("click", clearPlaylist, false);
        }
    });

    function clearPlaylist() {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;
        picker.fileTypeFilter.replaceAll(SdkSample.playlistExtensions);

        picker.pickSingleFileAsync()
            .then(function (file) {
                if (file) {
                    return Windows.Media.Playlists.Playlist.loadAsync(file);
                }
                return WinJS.Promise.wrapError("No file picked.");
            })
            .then(function (playlist) {
                SdkSample.playlist = playlist;
                SdkSample.playlist.files.clear();
                return SdkSample.playlist.saveAsync();
            })
            .done(function (file) {
                WinJS.log && WinJS.log("Playlist cleared.", "sample", "status");
            }, function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
    }
})();
