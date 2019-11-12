//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Helper variables.
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;

    var groupedItems;
    var errorDiv;
    var listViewIn;
    var semanticZoom;
    var page;
    var listViewState = {
        previousSelectionIndices: [],
        previousItemIndex: 0
    };

    ui.Pages.define("/pages/news/news.html", {
        initializeLayout: function () {
            /// <summary>This function updates the ListView with new layouts.</summary>
            if (document.documentElement.offsetWidth <= 480) {
                semanticZoom.zoomedOut = false;
                semanticZoom.locked = true;
                listViewIn.itemDataSource = groupedItems.groups.dataSource;
                listViewIn.groupDataSource = null;
                listViewIn.itemTemplate = document.querySelector("#feedTileTemplate");
                listViewIn.layout = new ui.ListLayout();
            } else {
                semanticZoom.locked = false;
                listViewIn.itemDataSource = groupedItems.dataSource;
                listViewIn.groupDataSource = groupedItems.groups.dataSource;
                listViewIn.itemTemplate = Render.renderArticleTile;
                listViewIn.layout = new ui.CellSpanningLayout({
                    groupHeaderPosition: "top",
                    groupInfo: {
                        enableCellSpanning: true,
                        cellWidth: 250,
                        cellHeight: 120
                    },
                    itemInfo: page.getItemInfo
                });
            }

            // If the app is in an error state, alert the user.
            if (Status.error) {
                listViewIn.itemDataSource = null;
                listViewIn.groupDataSource = null;
                errorDiv.style.display = "";
                errorDiv.innerText = Status.message;
            } else {
                errorDiv.style.display = "none";
            }
        },

        getItemInfo: function (index) {
            /// <summary>
            /// Given an index return the height and width of the tile corresponding to the item.
            /// </summary>
            /// <param name="index">Index of item in the ListView.</param>
            /// <returns>Height and width of the tile.</returns>
            var result;
            var item = groupedItems.getAt(index);

            // Set the dimensions of the title based on the type of tile (error, large, or normal).
            if (item.error) {
                result = {
                    height: 120,
                    width: 250,
                    newColumn: true
                };
            } else if (item.title === item.group.largeTileTitle) {
                result = {
                    height: 510,
                    width: 510,
                    newColumn: true
                };
            } else {
                result = {
                    height: 250,
                    width: 250,
                    newColumn: false
                };
            }

            return result;
        },

        itemInvoked: function (args) {
            /// <summary>
            /// This function is called when an item is invoked.  The page to navigate to depends on the view state.
            /// </summary>
            /// <param name="args">Event args.</param>
            if (document.documentElement.offsetWidth <= 480) {
                // If the page is snapped, the user invoked a group.
                var feed = groupedItems.groups.getAt(args.detail.itemIndex);
                nav.navigate("/pages/feed/feed.html", { feedKey: feed.key });
            } else {
                // If the page is not snapped, the user invoked an item.
                var article = groupedItems.getAt(args.detail.itemIndex);
                listViewState.previousSelectionIndices = Render.selectionIndices;
                listViewState.previousItemIndex = args.detail.itemIndex;
                nav.navigate("/pages/article/article.html", { article: article });
            }
        },

        ready: function (element, options) {
            /// <summary>
            /// This function is called whenever a user navigates to this page. It populates the
            /// page elements with the app's data.
            /// </summary>
            /// <param name="element">
            /// The DOM element that contains all the content for the page.
            /// </param>
            /// <param name="options">
            /// An object that contains one or more property/value pairs to apply to the PageControl.
            /// </param>
            page = this;

            // Get the grouped articles.
            groupedItems = Render.getGroupedArticleItems(Data.subFeeds, ListViewLimits.maxItems);

            // Force the semantic zoom layout.
            semanticZoom = element.querySelector("#articlesListViewArea").winControl;
            semanticZoom.forceLayout();

            errorDiv = element.querySelector("#errorDiv");

            // Setup the zoomed-in list view.
            listViewIn = element.querySelector("#articlesListView-in").winControl;
            listViewIn.groupHeaderTemplate = element.querySelector("#feedHeaderTemplate");
            listViewIn.oniteminvoked = this.itemInvoked.bind(this);
            // If the user comes back to this page after navigating to an item, ensure
            // that the previous item is visible when the ListView is loaded.
            if (listViewState.previousSelectionIndices.toString() === Render.selectionIndices.toString() &&
                listViewState.previousItemIndex > 0) {
                setImmediate(function () {
                    listViewIn.ensureVisible(listViewState.previousItemIndex);
                    listViewState.previousItemIndex = 0;
                });
            }
            this.initializeLayout();

            listViewIn.element.focus();

            // Setup the zoomed-out list view.
            var listViewOut = element.querySelector("#articlesListView-out").winControl;
            listViewOut.itemDataSource = groupedItems.groups.dataSource;
            listViewOut.itemTemplate = element.querySelector("#feedTileTemplate");
            listViewOut.layout = new ui.GridLayout();

            document.getElementById("refreshContentCmd").addEventListener("click", refreshFeeds, false);
        },

        unload: function () {
            /// <summary>This function is called when the user navigates away from the page.</summary>
            document.getElementById("refreshContentCmd").removeEventListener("click", refreshFeeds, false);
        },

        updateLayout: function (element) {
            /// <summary>This function updates the page layout in response to window size changes.</summary>
            /// <param name="element">The DOM element that contains all the content for the page.</param>
            this.initializeLayout();
        },

        updateResources: function (element, e) {
            /// <summary>
            /// Called by _contextChanged event handler in navigator.js when a resource 
            /// qualifier (language, scale, contrast, etc.) has changed. The element 
            /// passed is the root of this page. 
            ///
            /// Since this sample app currently doesn't have any assets with variants
            /// for scale/contrast/language/etc., the lines below that do the actual 
            /// work are commented out. This is provided here to model how to handle 
            /// scale or other resource context changes if this app were expanded to 
            /// include resources with assets for such variantions.
            /// </summary>

            // Will filter for changes to specific qualifiers.
            if (e.detail.qualifier === "Scale" || e.detail.qualifier === "Contrast") {
                // if there are string resources bound to properties using data-win-res,
                // the following line will update those properties: 

                //WinJS.Resources.processAll(element);

                // Background images from the app package with variants for scale, etc. 
                // are automatically reloaded by the platform when a resource context 
                // qualifier has changed. That is not done, however, for img elements. 
                // The following will make sure those are updated:

                //var imageElements = document.getElementsByTagName("img");
                //for (var i = 0, l = imageElements.length; i < l; i++) {
                //    var previousSource = imageElements[i].src;
                //    var uri = new Windows.Foundation.Uri(document.location, previousSource);
                //    if (uri.schemeName === "ms-appx") {
                //        imageElements[i].src = "";
                //        imageElements[i].src = previousSource;
                //    }
                //}
            }
        }

    });

    function refreshFeeds() {
        /// <summary>Refreshes the feeds displayed on the page.</summary>
        Data.subFeeds.forEach(function (feed) {
            feed.refreshArticlesAsync();
        });

        groupedItems = Render.getGroupedArticleItems(Data.subFeeds, ListViewLimits.maxItems);
        page.initializeLayout();
    }
})();