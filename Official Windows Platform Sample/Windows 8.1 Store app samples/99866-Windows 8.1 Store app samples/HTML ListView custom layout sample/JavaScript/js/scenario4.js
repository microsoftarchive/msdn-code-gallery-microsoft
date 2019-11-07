//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var cssClassSizeMap = {
        statusItemSize: { height: 90 }, // plus 10 pixels to incorporate the margin-top
        photoItemSize: { height: 260 }, // plus 10 pixels to incorporate the margin-top
        groupHeaderSize: { height: 86, type: "header" }
    };

    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            // Wire up ListView properties
            this._statusTemplate = element.querySelector(".statusTemplate").winControl;
            this._photoTemplate = element.querySelector(".photoTemplate").winControl;
            this._groupHeaderTemplate = element.querySelector(".headerTemplate");
            this._listView = element.querySelector(".listView").winControl;
            this._listView.layout = new SDKSample.Scenario4.StatusLayout({ itemInfo: this._itemInfo, cssClassSizeMap: cssClassSizeMap });
            this._listView.itemTemplate = this._statusRenderer.bind(this);
            this._listView.groupHeaderTemplate = this._groupHeaderTemplate;
            this._listView.itemDataSource = Data.groupedList.dataSource;
            this._listView.groupDataSource = Data.groupedList.groups.dataSource;

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
            var item = Data.groupedList.getItem(itemIndex);
            var cssClass = "statusItemSize";
            if (item.data.type === "photo") {
                cssClass = "photoItemSize";
            }

            return cssClass;
        }
    });

    WinJS.Namespace.define("SDKSample.Scenario4", {
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
            // Private function that is responsible for laying out just one group
            _layoutGroup: function (tree, offset) {
                var items = tree.itemsContainer.items;

                // Set the size of a group header container
                WinJS.Utilities.addClass(tree.header, "groupHeaderSize");

                // Set the size of the item containers
                var itemsLength = items.length;
                for (var i = 0; i < itemsLength; i++) {
                    // Get CSS class by item index
                    var cssClass = this._itemInfo(i + offset);
                    WinJS.Utilities.addClass(items[i], cssClass);

                    // Cache the height and index to be used for virtualization
                    this._itemCache.push({
                        height: this._cssClassSizeMap[cssClass].height,
                        type: this._cssClassSizeMap[cssClass].type,
                        index: i + offset
                    });
                }
            },

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
            // Calls _layoutGroup to layout the items of each group
            layout: function (tree, changedRange, modifiedElements, modifiedGroups) {
                this._itemCache = [];
                var offset = 0;

                var treeLength = tree.length;
                for (var i = 0; i < treeLength; i++) {
                    // Cache the height and index of the group to be used for virtualization
                    this._itemCache.push({
                        height: this._cssClassSizeMap["groupHeaderSize"].height,
                        type: this._cssClassSizeMap["groupHeaderSize"].type
                    });

                    // Layout items for the group
                    this._layoutGroup(tree[i], offset);
                    offset += tree[i].itemsContainer.items.length;
                }

                return WinJS.Promise.wrap();// A Promise or {realizedRangeComplete: Promise, layoutComplete: Promise};
            },

            // Supports virtualization by determining what item indices are needed for a given pixel range
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

                    // Headers are excluded from the range
                    if (item.type !== "header") {

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
                }

                return { firstIndex: firstIndex, lastIndex: lastIndex };
            },

            // Default implementation of the itemInfo
            _itemInfo: function (itemIndex) {
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
