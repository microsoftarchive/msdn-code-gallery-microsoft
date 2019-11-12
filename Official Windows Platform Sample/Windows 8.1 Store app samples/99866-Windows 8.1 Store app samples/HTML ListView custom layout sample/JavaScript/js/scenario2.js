//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var cssClassSizeMap = {
        statusItemSize: { height: 90 }, // plus 10 pixels to incorporate the margin-top
        photoItemSize: { height: 260 }  // plus 10 pixels to incorporate the margin-top
    };

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            // Wire up ListView properties
            this._statusTemplate = element.querySelector(".statusTemplate").winControl;
            this._photoTemplate = element.querySelector(".photoTemplate").winControl;
            this._listView = element.querySelector(".listView").winControl;
            this._listView.layout = new SDKSample.Scenario2.StatusLayout({ itemInfo: this._itemInfo, cssClassSizeMap: cssClassSizeMap });
            this._listView.itemTemplate = this._statusRenderer.bind(this);
            this._listView.itemDataSource = Data.list.dataSource;

            // Display the visible first and last index of the ListView after scrolling
            this._listView.addEventListener("loadingstatechanged", function (ev) {
                if (this._listView.loadingState === "complete") {
                    WinJS.log && WinJS.log("indexOfFirstVisible: " + this._listView.indexOfFirstVisible + ", indexOfLastVisible: " + this._listView.indexOfLastVisible, "sample", "status");
                }
            }.bind(this));
        },

        // Conditional renderer that chooses between statusTemplate and photoTemplate
        _statusRenderer: function (itemPromise) {
            var that = this;
            return itemPromise.then(function (item) {
                if (item.data.type === "photo") {
                    return that._photoTemplate.renderItem(itemPromise);
                }
                return that._statusTemplate.renderItem(itemPromise);
            });
        },

        // Function used by StatusLayout to detemine what CSS class needs to be placed on the item to size it
        _itemInfo: function (itemIndex) {
            var item = Data.list.getItem(itemIndex);
            var cssClass = "statusItemSize";
            if (item.data.type === "photo") {
                cssClass = "photoItemSize";
            }

            return cssClass;
        }
    });

    WinJS.Namespace.define("SDKSample.Scenario2", {
        StatusLayout: WinJS.Class.define(function (options) {
            this._site = null;
            this._surface = null;

            options = options || {};
            if (options.itemInfo) {
                this._itemInfo = options.itemInfo;
            }
            if (options.cssClassSizeMap) {
                this._cssClassSizeMap = options.cssClassSizeMap;
            }
        },
        {
            // This sets up any state and CSS layout on the surface of the layout
            initialize: function (site) {
                this._site = site;
                this._surface = this._site.surface;
                this._itemCache = [];

                // Add a CSS class to control the surface level layout
                WinJS.Utilities.addClass(this._surface, "statusLayout");

                return WinJS.UI.Orientation.vertical;
            },

            // Reset the layout to its initial state
            uninitialize: function () {
                WinJS.Utilities.removeClass(this._surface, "statusLayout");
                this._site = null;
                this._surface = null;
                this._itemCache = null;
            },

            // Responsible for sizing and positioning items in the tree
            // This function applies a CSS class to the item element returned from the itemInfo function
            layout: function (tree, changedRange, modifiedElements, modifiedGroups) {
                this._itemCache = [];
                var items = tree[0].itemsContainer.items;

                var itemsLength = items.length;
                for (var i = 0; i < itemsLength; i++) {
                    // Get CSS class by item index
                    var cssClass = this._itemInfo(i);
                    WinJS.Utilities.addClass(items[i], cssClass);

                    // Cache the height and index to be used for virtualization
                    this._itemCache.push({
                        height: this._cssClassSizeMap[cssClass].height,
                        index: i
                    });
                }

                return WinJS.Promise.wrap();// A Promise or {realizedRangeComplete: Promise, layoutComplete: Promise};
            },

            // Implements virtualization by determining what item indices are needed for a given pixel range 
            // This function steps through the itemCache to determine if item heights fall witin the given pixel range
            itemsFromRange: function (firstPixel, lastPixel) {
                var totalLength = 0;

                // Initialize firstIndex and lastIndex to be an empty range
                var firstIndex = 0;
                var lastIndex = -1;

                var firstItemFound = false;

                var itemCacheLength = this._itemCache.length;
                for (var i = 0; i < itemCacheLength; i++) {
                    var item = this._itemCache[i];
                    totalLength += item.height;

                    // Find the firstIndex
                    if (!firstItemFound && totalLength >= firstPixel) {
                        firstIndex = item.index;
                        lastIndex = firstIndex;
                        firstItemFound = true;
                    } else if (totalLength >= lastPixel) {
                        // Find the lastIndex
                        lastIndex = item.index;
                        break;
                    } else if (firstItemFound && i === itemCacheLength - 1) {
                        // If we are at the end of the cache and we have found the firstItem, the lastItem is in the range
                        lastIndex = item.index;
                    }
                }

                return { firstIndex: firstIndex, lastIndex: lastIndex };
            },

            // Default implementation of the itemInfo
            _itemInfo: function(itemIndex) {
                return "statusItemSize";
            },

            // Set/Get itemInfo function
            itemInfo: {
                get: function () {
                    return this._itemInfo;
                },
                set: function (itemInfoFunction) {
                    this._itemInfo = itemInfoFunction;
                }
            },

            // Used for looking up the sizes of items during layout
            cssClassSizeMap: {
                get: function () {
                    return this._cssClassSizeMap;
                },
                set: function (value) {
                    this._cssClassSizeMap = value;
                }
            }
        })
    });
})();
