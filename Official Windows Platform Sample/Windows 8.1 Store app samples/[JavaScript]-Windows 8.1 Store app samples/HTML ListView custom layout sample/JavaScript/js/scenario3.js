//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var cssClassSizeMap = {
        statusItemSize: { height: 90 }, // plus 10 pixels to incorporate the margin-bottom
        photoItemSize: { height: 260 }  // plus 10 pixels to incorporate the margin-bottom
    };

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            // Wire up ListView properties
            this._statusTemplate = element.querySelector(".statusTemplate").winControl;
            this._photoTemplate = element.querySelector(".photoTemplate").winControl;
            this._groupHeaderTemplate = element.querySelector(".headerTemplate");
            this._listView = element.querySelector(".listView").winControl;
            this._listView.layout = new SDKSample.Scenario3.StatusLayout({ itemInfo: this._itemInfo, cssClassSizeMap: cssClassSizeMap });
            this._listView.itemTemplate = this._statusRenderer.bind(this);
            this._listView.groupHeaderTemplate = this._groupHeaderTemplate;
            this._listView.itemDataSource = Data.groupedList.dataSource;
            this._listView.groupDataSource = Data.groupedList.groups.dataSource;
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

    WinJS.Namespace.define("SDKSample.Scenario3", {
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

                var itemsLength = items.length;
                for (var i = 0; i < itemsLength; i++) {
                    // Get CSS class by item index
                    var cssClass = this._itemInfo(i + offset);
                    WinJS.Utilities.addClass(items[i], cssClass);
                }
            },

            // This sets up any state and CSS layout on the surface of the layout
            initialize: function (site) {
                this._site = site;
                this._surface = this._site.surface;

                // Add a CSS class to control the surface level layout
                WinJS.Utilities.addClass(this._surface, "statusLayout");

                return WinJS.UI.Orientation.vertical;
            },

            // Reset the layout to its initial state
            uninitialize: function () {
                WinJS.Utilities.removeClass(this._surface, "statusLayout");
                this._site = null;
                this._surface = null;
            },

            // Responsible for sizing and positioning items in the tree
            // Calls _layoutGroup to layout the items of each group
            layout: function (tree, changedRange, modifiedElements, modifiedGroups) {
                var offset = 0;

                var treeLength = tree.length;
                for (var i = 0; i < treeLength; i++) {
                    this._layoutGroup(tree[i], offset);
                    offset += tree[i].itemsContainer.items.length;
                }

                return WinJS.Promise.wrap();// A Promise or {realizedRangeComplete: Promise, layoutComplete: Promise};
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
