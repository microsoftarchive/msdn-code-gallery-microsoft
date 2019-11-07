//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var itemArray = [
        { type: "status", picture: "/images/60Banana.png", title: "Banana Blast", text: "Lorem ipsum dolor sit amet, consectetur adipiscing elit." },
        { type: "photo", picture: "/images/60Lemon.png", title: "Lavish Lemon Ice", text: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In ligula nisi, vehicula nec eleifend vel, rutrum non dolor." },
        { type: "photo", picture: "/images/60Mint.png", title: "Marvelous Mint", text: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In ligula nisi, vehicula nec eleifend vel, rutrum non dolor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Curabitur elementum scelerisque accumsan." },
        { type: "status", picture: "/images/60Orange.png", title: "Creamy Orange", text: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In ligula nisi, vehicula nec eleifend vel, rutrum non dolor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Curabitur elementum scelerisque accumsan. In hac habitasse platea dictumst." },
        { type: "status", picture: "/images/60Strawberry.png", title: "Succulent Strawberry", text: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In ligula nisi, vehicula nec eleifend vel, rutrum non dolor." },
        { type: "photo", picture: "/images/60Vanilla.png", title: "Very Vanilla", text: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In ligula nisi, vehicula nec eleifend vel, rutrum non dolor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Curabitur elementum scelerisque accumsan." }
    ];

    var items = [];

    // Generate 120 items
    for (var i = 0; i < 20; i++) {
        itemArray.forEach(function (item) {
            items.push(item);
        });
    }
    var list = new WinJS.Binding.List(items);

    // Sort the items in localized order so that the items within the groups
    // appear in the correct order
    var sortedItems = items.sort(function (left, right) {
        return right.title.localeCompare(left.title);
    });

    // Build a Binding.List to hold the sorted data.
    var sortedList = new WinJS.Binding.List(sortedItems);

    // For globalized grouping data
    var charGroups = Windows.Globalization.Collation.CharacterGroupings();

    // Function which returns the group key that an item belongs to
    function getGroupKey(dataItem) {

        // This sample uses globalization data to determine the grouping
        return charGroups.lookup(dataItem.title); // Ensure this always returns a string
    }

    // Function which returns the data for a group
    function getGroupData(dataItem) {

        // In this case, just use the group key
        var key = getGroupKey(dataItem);
        return {
            groupTitle: key,
        };
    }

    // Create a grouped list for the ListView from the item data and the grouping functions
    var groupedList = sortedList.createGrouped(getGroupKey, getGroupData);

    WinJS.Namespace.define("Data", {
        list: list,
        groupedList: groupedList
    });
})();