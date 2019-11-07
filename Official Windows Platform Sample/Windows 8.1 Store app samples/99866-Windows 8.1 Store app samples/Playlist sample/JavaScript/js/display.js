//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/display.html", {
        ready: function (element, options) {
            document.getElementById("PickPlaylistButton").addEventListener("click", displayPlaylist, false);
        }
    });

    function displayPlaylist() {
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.musicLibrary;
        picker.fileTypeFilter.replaceAll(SdkSample.playlistExtensions);

        var promiseCount = 0;

        picker.pickSingleFileAsync()
            .then(function (item) {
                if (item) {
                    return Windows.Media.Playlists.Playlist.loadAsync(item);
                }
                return WinJS.Promise.wrapError("No file picked.");
            })
            .then(function (playlist) {
                SdkSample.playlist = playlist;
                var promises = {};

                // Request music properties for each file in the playlist.
                playlist.files.forEach(function (file) {
                    promises[promiseCount++] = file.properties.getMusicPropertiesAsync();
                });

                // Print the music properties for each file. Due to the asynchronous
                // nature of the call to retrieve music properties, the data may appear
                // in an order different than the one specified in the original playlist.
                // To guarantee the ordering we use Promise.join with an associative array
                // passed as a parameter, containing an index for each individual promise.
                return WinJS.Promise.join(promises);
            })
            .done(function (results) {
                var output = "Playlist content:\n\n";

                var musicProperties;
                for (var resultIndex = 0; resultIndex < promiseCount; resultIndex++) {
                    musicProperties = results[resultIndex];
                    output += "Title: " + musicProperties.title + "\n";
                    output += "Album: " + musicProperties.album + "\n";
                    output += "Artist: " + musicProperties.artist + "\n\n";
                }

                if (resultIndex === 0) {
                    output += "(playlist is empty)";
                }

                WinJS.log && WinJS.log(output, "sample", "status");
            }, function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
    }
})();
