//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario2Listview.html", {
        ready: function (element, options) {
            document.querySelector(".win-backbutton").addEventListener("click", function (e) {
                WinJS.Navigation.navigate("/html/scenario2.html");
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
        // Shallow query to get the file hierarchy
        queryOptions.folderDepth = Windows.Storage.Search.FolderDepth.shallow;
        queryOptions.sortOrder.clear();
        // Order items by type so folders come first
        queryOptions.sortOrder.append({ ascendingOrder: false, propertyName: "System.IsFolder" });
        queryOptions.sortOrder.append({ ascendingOrder: true, propertyName: "System.ItemName" });
        queryOptions.indexerOption = Windows.Storage.Search.IndexerOption.useIndexerWhenAvailable;

        var fileQuery = library.createItemQueryWithOptions(queryOptions);
        var dataSourceOptions = {
            mode: Windows.Storage.FileProperties.ThumbnailMode.picturesView,
            requestedThumbnailSize: 190,
            thumbnailOptions: Windows.Storage.FileProperties.ThumbnailOptions.none
        };

        var dataSource = new WinJS.UI.StorageDataSource(fileQuery, dataSourceOptions);

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
        var img, overlay, overlayText;
        if (!element) {
            // dom is not recycled, so create inital structure
            element = document.createElement("div");
            element.innerHTML = "<img /><div class='overlay'><div class='overlayText'></div></div>";
        }
        img = element.querySelector("img");
        overlay = element.querySelector(".overlay");
        overlayText = element.querySelector(".overlayText");
        img.style.opacity = 0;

        return {
            // returns the placeholder
            element: element,
            // and a promise that will complete when the item is fully rendered
            renderComplete: itemPromise.then(function (item) {
                // now do easy work
                if (item.data.isOfType(Windows.Storage.StorageItemTypes.folder)) {
                    overlay.style.visibility = "visible";
                    overlayText.innerText = item.data.name;
                } else {
                    overlay.style.visibility = "hidden";
                }
                return item.ready;
            }).then(function (item) {
                // wait until item.ready before doing expensive work
                return WinJS.UI.StorageDataSource.loadThumbnail(item, img);
            })
        };
    }
})();
