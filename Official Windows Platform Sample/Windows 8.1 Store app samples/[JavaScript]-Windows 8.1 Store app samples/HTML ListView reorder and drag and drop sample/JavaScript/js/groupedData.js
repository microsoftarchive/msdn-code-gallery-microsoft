//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Data used in the ListView for the sample.  Data in different groups should not interleave.
    var flavors = [
        { title: "Banana Blast", kind: "FY", picture: "images/60Banana.png" },
        { title: "Banana Blast", kind: "FY", picture: "images/60Banana.png" },
        { title: "Succulent Strawberry", kind: "FY", picture: "images/60Strawberry.png" },
        { title: "Banana Blast", kind: "FY", picture: "images/60Banana.png" },
        { title: "Marvelous Mint", kind: "FY", picture: "images/60Mint.png" },
        { title: "Succulent Strawberry", kind: "FY", picture: "images/60Strawberry.png" },
        { title: "Marvelous Mint", kind: "FY", picture: "images/60Mint.png" },
        { title: "Banana Blast", kind: "FY", picture: "images/60Banana.png" },
        { title: "Marvelous Mint", kind: "GO", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", kind: "GO", picture: "images/60Mint.png" },
        { title: "Banana Blast", kind: "GO", picture: "images/60Banana.png" },
        { title: "Marvelous Mint", kind: "GO", picture: "images/60Mint.png" },
        { title: "Succulent Strawberry", kind: "GO", picture: "images/60Strawberry.png" },
        { title: "Marvelous Mint", kind: "GO", picture: "images/60Mint.png" },
        { title: "Succulent Strawberry", kind: "GO", picture: "images/60Strawberry.png" },
        { title: "Banana Blast", kind: "IC", picture: "images/60Banana.png" },
        { title: "Marvelous Mint", kind: "IC", picture: "images/60Mint.png" },
        { title: "Succulent Strawberry", kind: "IC", picture: "images/60Strawberry.png" },
        { title: "Marvelous Mint", kind: "IC", picture: "images/60Mint.png" },
        { title: "Succulent Strawberry", kind: "IC", picture: "images/60Strawberry.png" },
        { title: "Banana Blast", kind: "IC", picture: "images/60Banana.png" },
        { title: "Banana Blast", kind: "IC", picture: "images/60Banana.png" },
        { title: "Banana Blast", kind: "IC", picture: "images/60Banana.png" },
        { title: "Marvelous Mint", kind: "IC", picture: "images/60Mint.png" },
        { title: "Very Vanilla", kind: "IC", picture: "images/60Vanilla.png" },
        { title: "Succulent Strawberry", kind: "ST", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", kind: "ST", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", kind: "ST", picture: "images/60Strawberry.png" },
    ];

    var desertStrings = {
        "IC": { title: "Ice Cream" },
        "FY": { title: "Low-fat frozen yogurt" },
        "ST": { title: "Sorbet" },
        "GO": { title: "Gelato" }
    };

    function initData() {
        // Returns the group key that an item belongs to
        function getGroupKey(dataItem) {
            return dataItem.kind;
        }
        WinJS.Utilities.markSupportedForProcessing(getGroupKey);

        // Returns the title for a group
        function getGroupData(dataItem) {
            return {
                // Lookup the full group title from the groups list
                title: desertStrings[dataItem.kind].title
            };
        }
        WinJS.Utilities.markSupportedForProcessing(getGroupData);

        // Sorts the groups for display in the ListView
        function compareGroups(leftKey, rightKey) {
            return leftKey.charCodeAt(0) - rightKey.charCodeAt(0);
        }
        WinJS.Utilities.markSupportedForProcessing(compareGroups);

        // Create the datasources that will then be set on the datasource
        var myBindingList = new WinJS.Binding.List(flavors);
        var myGroupedList = myBindingList.createGrouped(getGroupKey, getGroupData, compareGroups);
        return myGroupedList;
    }

    WinJS.Namespace.define("GroupedData", {
        initData: initData
    });
})();