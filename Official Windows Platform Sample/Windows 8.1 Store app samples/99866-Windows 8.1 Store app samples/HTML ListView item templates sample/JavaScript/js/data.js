//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Maps the CSS classes to item sizes to be used for determining the size of the items
// The total width of the item, include the item width, left and right padding, left and right border, and left and right margin
// The total height of the item, include the item height, top and bottom padding, top and bottom border, and top and bottom margin
// For sizing items that span cells ensure to include the margin value between the spanning cells
var sizeMap = {
    smallListIconTextItem: { width: 310, height: 80 },
    mediumListIconTextItem: { width: 310, height: 170 },
    largeListIconTextItem: { width: 310, height: 260 },
    defaultSize: { width: 310, height: 80 }
};

// Enable cell spanning in ListView and specify
// the cellWidth and cellHeight for the items
var groupInfo = WinJS.Utilities.markSupportedForProcessing(function groupInfo() {
    return {
        enableCellSpanning: true,
        cellWidth: 310,
        cellHeight: 80
    };
});

// Item info function that returns the size of a cell spanning item
var itemInfo = WinJS.Utilities.markSupportedForProcessing(function itemInfo(itemIndex) {
    var size = sizeMap.defaultSize;

    // Get the item from the data source
    var item = myCellSpanningData.getAt(itemIndex);
    if (item) {
        // Get the size based on the item type
        size = sizeMap[item.type];
    }

    return size;
});

var myData = new WinJS.Binding.List([
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" }
    ]);

var myCellSpanningData = new WinJS.Binding.List([
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", type: "smallListIconTextItem" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", type: "mediumListIconTextItem" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", type: "largeListIconTextItem" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", type: "mediumListIconTextItem" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", type: "smallListIconTextItem" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", type: "smallListIconTextItem" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", type: "mediumListIconTextItem" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", type: "mediumListIconTextItem" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", type: "smallListIconTextItem" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", type: "smallListIconTextItem" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", type: "smallListIconTextItem" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", type: "smallListIconTextItem" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", type: "smallListIconTextItem" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", type: "smallListIconTextItem" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", type: "mediumListIconTextItem" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", type: "smallListIconTextItem" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", type: "largeListIconTextItem" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", type: "mediumListIconTextItem" }
]);

var myDataWithRatings = new WinJS.Binding.List([
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 3 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 1 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 1 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 2 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 4 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 4 },
]);
