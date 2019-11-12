//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1ProgrammaticListview.html", {
        ready: function (element, options) {
            document.querySelector(".win-backbutton").addEventListener("click", function (e) {
                WinJS.Navigation.navigate("/html/scenario1.html");
            }, false);
            loadListViewControl();
            // Override contentHost view for the gallery
            var contentHost = document.getElementById("contentHost");
            WinJS.Utilities.addClass(contentHost, "gallery");
            // Set initial layout
            this.updateLayout(contentHost);
        },

        updateLayout: function (element) {
            var listView = document.getElementById("listviewDiv").winControl;
            if (window.innerWidth < 768) {
                listView.layout = new WinJS.UI.ListLayout();
            } else {
                listView.layout = new WinJS.UI.GridLayout();
            }
            listView.layout.backdropColor = "#333333"; // Need to match up with the item background color.
        },

        unload: function () {
            // Remove override styles
            var contentHost = document.getElementById("contentHost");
            WinJS.Utilities.removeClass(contentHost, "gallery");
        }
    });

    function loadListViewControl() {
        // Build datasource from the pictures library
        var library = Windows.Storage.KnownFolders.picturesLibrary;
        var queryOptions = new Windows.Storage.Search.QueryOptions;
        queryOptions.folderDepth = Windows.Storage.Search.FolderDepth.deep;
        queryOptions.indexerOption = Windows.Storage.Search.IndexerOption.useIndexerWhenAvailable;

        var fileQuery = library.createFileQueryWithOptions(queryOptions);
        var dataSourceOptions = {
            mode: Windows.Storage.FileProperties.ThumbnailMode.picturesView,
            requestedThumbnailSize: 190,
            thumbnailOptions: Windows.Storage.FileProperties.ThumbnailOptions.none
        };

        var dataSource = new WinJS.UI.StorageDataSource(fileQuery, dataSourceOptions);
        // An equivalent datasource can be created with all the default options above by simply calling
        // var dataSource = new WinJS.UI.StorageDataSource("Pictures");

        var container = document.getElementById("listviewDiv");
        var listViewOptions = {
            itemDataSource: dataSource,
            itemTemplate: storageRenderer,
            layout: new WinJS.UI.GridLayout(),
            selectionMode: "single"
        };

        var listViewControl = new WinJS.UI.ListView(container, listViewOptions);
    };

    function storageRenderer(itemPromise, element) {
        var img, itemStatus;
        if (!element) {
            // dom is not recycled, so create inital structure
            element = document.createElement("div");
            element.className = "FileTemplate";
            element.appendChild(document.createElement("img"));
        }
        img = element.querySelector("img");
        img.style.opacity = 0;

        return {
            // returns the placeholder
            element: element,
            // and a promise that will complete when the item is fully rendered
            renderComplete: itemPromise.then(function (item) {
                // now do cheap work (none here, so we return item ready)
                return item.ready;
            }).then(function (item) {
                // wait until item.ready before doing expensive work
                return WinJS.UI.StorageDataSource.loadThumbnail(item, img).then(function (image) {
                    // perform any operation that requires the thumbnail to be available
                });
            })
        };
    }
})();
