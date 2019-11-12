//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var g_dmsServers;
    var g_mediaFiles;
    var g_playToLocalElement = null;

    var page = WinJS.UI.Pages.define("/html/locatevideo.html", {
        ready: function (element, options) {
            id("dmsRefreshButton").addEventListener("click", dmsInit, false);
            id("dmsSelect").addEventListener("change", onDMSChange, false);
            id("mediaSelect").addEventListener("change", onMediaChange, false);

            dmsInit();
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function showMsgInSelect(select, msg) {
        var n = select.options.length;
        for (var i = 0; i < n; i++) {
            select.options.remove(0);
        }
        var option = document.createElement("OPTION");
        option.value = 0;
        option.innerText = msg;
        select.options.add(option);
    }

    function handleError(error) {
        WinJS.log && WinJS.log("Error: " + error.message, "Media Server", "error");
    }

    function populateSelect(select, objects) {
        var num = select.options.length;
        for (var i = 0; i < num; i++) {
            select.options.remove(0);
        }

        if (objects) {
            if (objects.length > 0) {
                for (i = 0; i < objects.length; i++) {
                    var option = document.createElement('OPTION');
                    option.value = i;
                    option.innerText = objects[i].name;
                    select.options.add(option);
                }
            }
            else {
                showMsgInSelect(select, "Found 0 items.");
            }
        }
    }

    function dmsInit() {
        g_dmsServers = null;
        Windows.Storage.KnownFolders.mediaServerDevices.getFoldersAsync().then(function (servers) {
            populateSelect(id("dmsSelect"), servers);
            g_dmsServers = servers;
        },
        handleError);
    }

    function onMediaChange() {
        if (g_mediaFiles.length > 0) {
            var mediaSelect = id("mediaSelect");
            var video = id("localVideo4");
            var file = g_mediaFiles[mediaSelect.selectedIndex];
            video.src = URL.createObjectURL(file, { oneTimeOnly: true });
            video.play();
        }
    }

    function iterateOnArrayOfFolders(folders, folderIndex, collectedMediaItems, maxFilesToRetrieve) {
        return new WinJS.Promise(function (complete) {
            if (folderIndex < folders.length) {
                browseForVideoFiles(folders[folderIndex], collectedMediaItems, maxFilesToRetrieve).then(function (numberOfVideosFound) {
                    iterateOnArrayOfFolders(folders, folderIndex + 1, collectedMediaItems, maxFilesToRetrieve - numberOfVideosFound).then(function (nextFolderNumberOfVideosFound) {
                        complete(nextFolderNumberOfVideosFound + numberOfVideosFound);
                    });
                });
            }
            else {
                complete(0);
            }
        });
    }

    function browseForVideoFiles(folder, collectedMediaItems, maxFilesToRetrieve) {
        return new WinJS.Promise(function (complete) {
            folder.getFilesAsync(Windows.Storage.Search.CommonFileQuery.defaultQuery, 0, maxFilesToRetrieve).then(function (files) {
                var videoFilesFound = 0;

                for (var fileIndex = 0; fileIndex < files.length; ++fileIndex) {
                    if (files[fileIndex].contentType.length > 5 && files[fileIndex].contentType.substring(0, 5) === "video") {
                        ++videoFilesFound;
                        collectedMediaItems.push(files[fileIndex]);
                        if (maxFilesToRetrieve < videoFilesFound) {
                            break;
                        }
                    }
                }

                folder.getFoldersAsync().then(function (folders) {
                    iterateOnArrayOfFolders(folders, 0, collectedMediaItems, maxFilesToRetrieve).then(function (filesRetrieved) {
                        complete(videoFilesFound + filesRetrieved);
                    });
                });
            });
        });
    }

    function onDMSChange() {
        var dmsSelect = id("dmsSelect");
        id("localVideo4").pause();

        if (g_dmsServers && dmsSelect.selectedIndex < g_dmsServers.length) {
            g_mediaFiles = null;
            showMsgInSelect(id("mediaSelect"), "Retrieving media files...");
            var mediaServer = g_dmsServers[dmsSelect.selectedIndex];
            // Find "Videos" .
            var queryOptions = new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery);
            queryOptions.folderDepth = Windows.Storage.Search.FolderDepth.deep;
            queryOptions.userSearchFilter = "System.Kind:=video";

            if (mediaServer.areQueryOptionsSupported(queryOptions)) {
                mediaServer.createFileQueryWithOptions(queryOptions).getFilesAsync(0, 25).then(function (files) {
                    populateSelect(id("mediaSelect"), files);
                    g_mediaFiles = files;
                    if (files.length > 0) {
                        id("mediaSelect").selectedIndex = 0;
                        onMediaChange();
                    }
                });
            }
            else {
                var mediaFiles = new Array();
                browseForVideoFiles(mediaServer, mediaFiles, 25).then(function (numberOfFilesFound) {
                    populateSelect(id("mediaSelect"), mediaFiles);
                    g_mediaFiles = mediaFiles;
                    if (mediaFiles.length > 0) {
                        id("mediaSelect").selectedIndex = 0;
                        onMediaChange();
                    }
                });
            }
        } else {
            WinJS.log && WinJS.log("Error browsing DMS.", "Media Server", "error");
        }
    }

})();
