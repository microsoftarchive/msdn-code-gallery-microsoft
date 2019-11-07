//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var myData = new WinJS.Binding.List([
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 3 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 1 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 2 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 3 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 5 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 3 }, 
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 1 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 5 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 5 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 2 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 3 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 2 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 3 },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 3 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 1 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 2 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 3 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 5 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 3 }, 
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 1 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 5 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 5 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 2 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 3 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 2 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 3 },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 3 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 1 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 2 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 3 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 5 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 3 }, 
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 1 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 5 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 2 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 3 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 2 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 3 },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 3 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 1 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 2 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 3 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 5 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 3 }, 
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", rating: 1 },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", rating: 5 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 5 },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", rating: 2 },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", rating: 3 },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", rating: 2 },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", rating: 3 }
]);



(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/listview.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });




})();

