//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            initData();

            // Programatically create the listview specifying the datasource and templates
            var listView4 = new WinJS.UI.ListView(document.getElementById("listView4"), {
                itemDataSource: itemDataSource,
                groupDataSource: groupDataSource,
                itemTemplate: document.getElementById("mediumListIconTextTemplate"),
                groupHeaderTemplate: document.getElementById("groupTemplate"),
                layout: new WinJS.UI.GridLayout()
            });
        }
    });


    // Note: The size of data used for this sample is small, and so the technique shown here is probably
    // overkill, and would be more easily implemented using Binding.List. However the techniques shown here
    // can be used with large virtualized datasets.

    var flavors = [
        { title: "Banana Blast", kind: "IC", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "ST", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "IC", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "IC", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "ST", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "IC", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", kind: "FY", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "ST", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "GO", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "ST", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "IC", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "ST", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", kind: "FY", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "ST", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "GO", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "IC", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "ST", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "FY", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", kind: "GO", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "FY", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "IC", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "FY", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "IC", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "ST", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", kind: "IC", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "IC", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "GO", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "ST", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "FY", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "FY", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", kind: "FY", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "ST", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "FY", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "GO", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "GO", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "IC", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", kind: "IC", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "ST", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "GO", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "FY", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "FY", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "IC", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", kind: "IC", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "IC", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "FY", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "GO", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "GO", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "FY", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", kind: "FY", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", kind: "FY", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", kind: "IC", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", kind: "ST", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", kind: "ST", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", kind: "IC", picture: "images/60Vanilla.png", stock: "out_stock" },
    ];

    var desertTypes = [
        { key: "IC", type: "Ice Cream" },
        { key: "FY", type: "Low-fat frozen yogurt" },
        { key: "ST", type: "Sorbet" },
        { key: "GO", type: "Gelato" }
    ];

    var itemDataSource, groupDataSource;

    // Helper to process the data in the format that we need it
    function initData() {
        // First we need to make sure that the data is sorted by group. If interfacing with a service, 
        // then the sort needs done at the source of the data as a client side sort will involve pulling
        // down all the data.
        // For this example as our data is an array, we'll do it in memory.

        // form an array of the keys to help with the sort
        var groupKeys = [];
        for (var i = 0; i < desertTypes.length; i++) {
            groupKeys[i] = desertTypes[i].key;
        }

        var itemData = flavors;
        itemData.sort(function CompareForSort(item1, item2) {
            var first = groupKeys.indexOf(item1.kind), second = groupKeys.indexOf(item2.kind);
            if (first === second) { return 0; }
            else if (first < second) { return -1; }
            else { return 1; }
        });

        // Calculate the indexes of the first item for each group, ideally this should also be done at the source of the data
        var itemIndex = 0;
        for (var j = 0, len = desertTypes.length; j < len; j++) {
            desertTypes[j].firstItemIndex = itemIndex;
            var key = desertTypes[j].key;
            for (var k = itemIndex, len2 = itemData.length; k < len2; k++) {
                if (itemData[k].kind !== key) {
                    itemIndex = k;
                    break;
                }
            }
        }

        // Create the datasources that will then be set on the datasource
        itemDataSource = new flavorsDataSource(itemData);
        groupDataSource = new desertsDataSource(desertTypes);
    }

    //
    // Data Adapters
    //
    // This sample uses two seperate data sources, one for the groups and one for the items.
    // Each datasource is implemented using VirtualizedDataSource wrapping a custom data adapter. The
    // bulk of this sample is the implementation of the data adapters.
    //


    //
    // Flavors Data Adapter
    //
    // Data adapter for items. Follows the same pattern as the Bing Search adapter. The main concerns when
    // creating a data adapter for grouping are:
    // *  Listview works on an item-first mechanism, so the items need to be sorted and already arranged by group.
    // *  Supply the key for the group using the groupKey property for each item
    //
    var flavorsDataAdapter = WinJS.Class.define(
        function (data) {
            // Constructor
            this._itemData = data;
        },

        // Data Adapter interface methods
        // These define the contract between the virtualized datasource and the data adapter.
        // These methods will be called by virtualized datasource to fetch items, count etc.
        {
            // This example only implements the itemsFromIndex and count methods

            // Called to get a count of the items, result should be a promise for the items
            getCount: function () {
                var that = this;
                return WinJS.Promise.wrap(that._itemData.length);
            },

            // Called by the virtualized datasource to fetch items
            // It will request a specific item index and hints for a number of items either side of it
            // The implementation should return the specific item, and can choose how many either side.
            // to also send back. It can be more or less than those requested.
            //
            // Must return back an object containing fields:
            //   items: The array of items of the form:
            //      [{ key: key1, groupKey: group1, data : { field1: value, field2: value, ... }}, { key: key2, groupKey: group1, data : {...}}, ...]
            //   offset: The offset into the array for the requested item
            //   totalCount: (optional) Update the count for the collection
            itemsFromIndex: function (requestIndex, countBefore, countAfter) {
                var that = this;

                if (requestIndex >= that._itemData.length) {
                    return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.doesNotExist));
                }

                var lastFetchIndex = Math.min(requestIndex + countAfter, that._itemData.length - 1);
                var fetchIndex = Math.max(requestIndex - countBefore, 0);
                var results = [];

                // iterate and form the collection of items
                for (var i = fetchIndex; i <= lastFetchIndex; i++) {
                    var item = that._itemData[i];
                    results.push({
                        key: i.toString(), // the key for the item itself
                        groupKey: item.kind, // the key for the group for the item
                        data: item // the data fields for the item
                    });
                }

                // return a promise for the results
                return WinJS.Promise.wrap({
                    items: results, // The array of items
                    offset: requestIndex - fetchIndex, // The offset into the array for the requested item
                    totalCount: that._itemData.length // the total count
                });
            }
        });

    // Create a DataSource by deriving and wrapping the data adapter with a VirtualizedDataSource
    var flavorsDataSource = WinJS.Class.derive(WinJS.UI.VirtualizedDataSource, function (data) {
        this._baseDataSourceConstructor(new flavorsDataAdapter(data));
    });


    //
    // Groups Data Adapter
    //
    // Data adapter for the groups. Follows the same pattern as the items data adapter, but each item is a group.
    // The main concerns when creating a data adapter for groups are:
    // *  Groups can be enumerated by key or index, so the adapter needs to implement both itemsFromKey and itemsFromIndex
    // *  Each group should supply a firstItemIndexHint which is the index of the first item in the group. This enables listview
    //    to figure out the position of an item in the group so it can get the columns correct.
    //
    var desertsDataAdapter = WinJS.Class.define(
        function (groupData) {
            // Constructor
            this._groupData = groupData;
        },

        // Data Adapter interface methods
        // These define the contract between the virtualized datasource and the data adapter.
        // These methods will be called by virtualized datasource to fetch items, count etc.
        {
            // This example only implements the itemsFromIndex, itemsFromKey and count methods

            // Called to get a count of the items, this can be async so return a promise for the count
            getCount: function () {
                var that = this;
                return WinJS.Promise.wrap(that._groupData.length);
            },

            // Called by the virtualized datasource to fetch a list of the groups based on group index
            // It will request a specific group and hints for a number of groups either side of it
            // The implementation should return the specific group, and can choose how many either side
            // to also send back. It can be more or less than those requested.
            //
            // Must return back an object containing fields:
            //   items: The array of groups of the form:
            //      [{ key: groupkey1, firstItemIndexHint: 0, data : { field1: value, field2: value, ... }}, { key: groupkey2, firstItemIndexHint: 27, data : {...}}, ...
            //   offset: The offset into the array for the requested group
            //   totalCount: (optional) an update of the count of items
            itemsFromIndex: function (requestIndex, countBefore, countAfter) {
                var that = this;

                if (requestIndex >= that._groupData.length) {
                    return Promise.wrapError(new WinJS.ErrorFromName(UI.FetchError.doesNotExist));
                }

                var lastFetchIndex = Math.min(requestIndex + countAfter, that._groupData.length - 1);
                var fetchIndex = Math.max(requestIndex - countBefore, 0);
                var results = [];

                // form the array of groups
                for (var i = fetchIndex; i <= lastFetchIndex; i++) {
                    var group = that._groupData[i];
                    results.push({
                        key: group.key,
                        firstItemIndexHint: group.firstItemIndex,
                        data: group
                    });
                }
                return WinJS.Promise.wrap({
                    items: results, // The array of items
                    offset: requestIndex - fetchIndex, // The offset into the array for the requested item
                    totalCount: that._groupData.length // The total count
                });
            },

            // Called by the virtualized datasource to fetch groups based on the group's key
            // It will request a specific group and hints for a number of groups either side of it
            // The implementation should return the specific group, and can choose how many either side
            // to also send back. It can be more or less than those requested.
            //
            // Must return back an object containing fields:
            //   [{ key: groupkey1, firstItemIndexHint: 0, data : { field1: value, field2: value, ... }}, { key: groupkey2, firstItemIndexHint: 27, data : {...}}, ...
            //   offset: The offset into the array for the requested group
            //   absoluteIndex: the index into the list of groups of the requested group
            //   totalCount: (optional) an update of the count of items
            itemsFromKey: function (requestKey, countBefore, countAfter) {
                var that = this;
                var requestIndex = null;

                // Find the group in the collection
                for (var i = 0, len = that._groupData.length; i < len; i++) {
                    if (that._groupData[i].key === requestKey) {
                        requestIndex = i;
                        break;
                    }
                }
                if (requestIndex === null) {
                    return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.doesNotExist));
                }

                var lastFetchIndex = Math.min(requestIndex + countAfter, that._groupData.length - 1);
                var fetchIndex = Math.max(requestIndex - countBefore, 0);
                var results = [];

                //iterate and form the collection of the results
                for (var j = fetchIndex; j <= lastFetchIndex; j++) {
                    var group = that._groupData[j];
                    results.push({
                        key: group.key, // The key for the group
                        firstItemIndexHint: group.firstItemIndex, // The index into the items for the first item in the group
                        data: group // The data for the specific group
                    });
                }

                // Results can be async so the result is supplied as a promise
                return WinJS.Promise.wrap({
                    items: results, // The array of items
                    offset: requestIndex - fetchIndex, // The offset into the array for the requested item
                    absoluteIndex: requestIndex, // The index into the collection of the item referenced by key
                    totalCount: that._groupData.length // The total length of the collection
                });
            },

        });

    // Create a DataSource by deriving and wrapping the data adapter with a VirtualizedDataSource
    var desertsDataSource = WinJS.Class.derive(WinJS.UI.VirtualizedDataSource, function (data) {
        this._baseDataSourceConstructor(new desertsDataAdapter(data));
    });



})();
