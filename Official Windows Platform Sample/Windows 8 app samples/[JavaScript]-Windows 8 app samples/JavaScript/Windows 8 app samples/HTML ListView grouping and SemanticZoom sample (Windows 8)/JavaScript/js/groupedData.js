//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Data used in the ListView for the sample
var myList = new WinJS.Binding.List([
    { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
    { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
    { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
    { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
    { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
    { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
    { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
    { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" },
    { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
    { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
    { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
    { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" },
    { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
    { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
    { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
    { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" },
    { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
    { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
    { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
    { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
    { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
    { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
    { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
    { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
    { title: "Orangy Orange", text: "Sorbet", picture: "images/60Orange.png" },
    { title: "Orangy Orange", text: "Sorbet", picture: "images/60Orange.png" },
    { title: "Absolutely Orange", text: "Sorbet", picture: "images/60Orange.png" },
    { title: "Absolutely Orange", text: "Sorbet", picture: "images/60Orange.png" },
    { title: "Triple Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
    { title: "Triple Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
    { title: "Double Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
    { title: "Double Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
    { title: "Double Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
    { title: "Green Mint", text: "Gelato", picture: "images/60Mint.png" }
]);

// Create a grouped list for the ListView from the item data and the grouping functions
var myGroupedList = myList.createGrouped(getGroupKey, getGroupData, compareGroups);

// Function used to sort the groups by first letter
function compareGroups(left, right) {
    return left.toUpperCase().charCodeAt(0) - right.toUpperCase().charCodeAt(0);
}

// Function which returns the group key that an item belongs to
function getGroupKey(dataItem) {
    return dataItem.title.toUpperCase().charAt(0);
}

// Function which returns the data for a group
function getGroupData(dataItem) {
    return {
        title: dataItem.title.toUpperCase().charAt(0)
    };
}
