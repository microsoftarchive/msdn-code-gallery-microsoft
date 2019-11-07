//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Array passed as the standard store data for the sample
var myStoreData = new WinJS.Binding.List([
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", stock: "out_stock" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png", stock: "in_stock" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png", stock: "in_stock" },
        { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png", stock: "in_stock" },
        { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png", stock: "in_stock" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png", stock: "in_stock" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png", stock: "out_stock" },
]);

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {

            // Get the ListView control object
            var listView = element.querySelector("#listView1").winControl;
            listView.forceLayout();

            // Adds the items that have been selected in the ListView to the cart
            function addSelectionToCart() {

                // Check for selection
                if (listView.selection.count() === 0) {
                    WinJS.log && WinJS.log("No Items Selected!", "sample", "error");

                } else {
                    listView.selection.getItems().done(function (items) {
                        var selectedItemsString = "You have added ";

                        // Iterate through the items that are selected
                        // printing a string that describes the set
                        for (var i = 0; i < items.length; i++) {
                            if (i !== 0 & i === items.length - 1) {
                                selectedItemsString += "and ";
                            }

                            // Display name of the item
                            selectedItemsString += items[i].data.title;

                            if (i !== items.length - 1) {
                                selectedItemsString += ", ";
                            }
                        }

                        selectedItemsString += " to the cart.";
                        WinJS.log && WinJS.log(selectedItemsString, "sample", "status");

                        // Clear the selection so the user can continue shopping
                        listView.selection.clear();
                    });
                }
            }

            function itemInvokedHandler(eventObject) {
                eventObject.detail.itemPromise.done(function (invokedItem) {
                    var itemData = invokedItem.data;
                    WinJS.log && WinJS.log("You have been navigated to the product detail page for " + itemData.title + " - " + itemData.text, "sample", "status");
                });
            }

            element.querySelector("#scenario1Cart").addEventListener("click", addSelectionToCart, false);
            listView.addEventListener("iteminvoked", itemInvokedHandler, false);
        }
    });
})();
