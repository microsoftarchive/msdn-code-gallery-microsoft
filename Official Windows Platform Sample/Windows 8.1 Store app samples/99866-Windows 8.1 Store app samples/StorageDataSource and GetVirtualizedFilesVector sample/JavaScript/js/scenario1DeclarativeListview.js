//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1DeclarativeListview.html", {
        ready: function (element, options) {
            document.querySelector(".win-backbutton").addEventListener("click", function (e) {
                WinJS.Navigation.navigate("/html/scenario1.html");
            }, false);
            // Override contentHost view for the gallery
            var contentHost = document.getElementById("contentHost");
            WinJS.Utilities.addClass(contentHost, "gallery");
            // Set initial layout
            this.updateLayout(contentHost);
        },

        // This function updates the page layout in response to viewState changes
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

    function bindThumbnail(source, sourceProperty, destination, destinationProperty) {
        var image = destination;
        var item = source;

        var thumbnailUpdateHandler,
                firstImage = true,
                shouldRespondToThumbnailUpdate = false;

        // Load a thumbnail if it exists.
        var processThumbnail = function (thumbnail) {
            if (thumbnail) {
                var url = URL.createObjectURL(thumbnail, {oneTimeOnly: true});

                // If this is the first version of the thumbnail we're loading, fade it in.
                if (firstImage) {
                    image.onload = function () {
                        WinJS.UI.Animation.fadeIn(image);
                    };
                    firstImage = false;
                } else {
                    image.onload = null;
                }
                image.src = url;

                // If we have the full resolution thumbnail, we can cancel further updates and complete the promise
                // when current work is complete.
                if ((thumbnail.type !== Windows.Storage.FileProperties.ThumbnailType.icon) && !thumbnail.returnedSmallerCachedSize) {
                    item.removeEventListener("thumbnailupdated", thumbnailUpdateHandler);
                    shouldRespondToThumbnailUpdate = false;
                }
            }
        };

        thumbnailUpdateHandler = function (e) {
            // Ensure that a zombie update handler does not get invoked.
            if (shouldRespondToThumbnailUpdate) {
                processThumbnail(e.target.thumbnail);
            }
        };
        item.addEventListener("thumbnailupdated", thumbnailUpdateHandler);
        shouldRespondToThumbnailUpdate = true;

        // If we already have a thumbnail we should render it now.
        processThumbnail(item.thumbnail);
    }

    WinJS.Utilities.markSupportedForProcessing(bindThumbnail);

    WinJS.Namespace.define("DataSourceSample", {
        dataSource: new WinJS.UI.StorageDataSource("Pictures"), // Use shorthand datasource constructor
        bindThumbnail: bindThumbnail,
    });
})();
