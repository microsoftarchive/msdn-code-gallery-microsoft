//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Binding.List containing the flavors
var myFlavorsData = new WinJS.Binding.List([
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" },
    ]);

// Binding.List containing the toppings
var myToppingsData = new WinJS.Binding.List([
        { title: "Caramel Sauce", text: "Sauces", picture: "images/60SauceCaramel.png" },
        { title: "Chocolate Sauce", text: "Sauces", picture: "images/60SauceChocolate.png" },
        { title: "Strawberry Sauce", text: "Sauces", picture: "images/60SauceStrawberry.png" },
        { title: "Chocolate Sprinkles", text: "Sprinkles", picture: "images/60SprinklesChocolate.png" },
        { title: "Rainbow Sprinkles", text: "Sprinkles", picture: "images/60SprinklesRainbow.png" },
        { title: "Vanilla Sprinkles", text: "Sprinkes", picture: "images/60SprinklesVanilla.png" }
]);

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            var flavorsView = element.querySelector("#listView4a").winControl;
            var toppingsView = element.querySelector("#listView4b").winControl;

            flavorsView.forceLayout();
            toppingsView.forceLayout();

            function addItemToCart() {

                // Get the set of selected items in each of the two ListViews
                if (flavorsView.selection.count() === 0) {

                    // If no items are selected, report it
                    WinJS.log && WinJS.log("Please select a base flavor from the left panel.", "sample", "error");

                } else {
                    var selectedItemsString;

                    // Pull the base flavor from selection of first ListView
                    flavorsView.selection.getItems().
                        then(function (flavorItems) {

                            // Begin creating reporting string for shopping cart with the flavor title
                            selectedItemsString = "You have added a carton of " + flavorItems[0].data.title;

                            // Return the promise for the selected items
                            return toppingsView.selection.getItems();
                        }).
                        done(function (toppingItems) {
                            var selectedToppingsCount = toppingItems.length;
                            if (selectedToppingsCount > 0) {
                                selectedItemsString += " with ";
                                for (var i = 0; i < selectedToppingsCount; i++) {
                                    selectedItemsString += toppingItems[i].data.title;

                                    // add a comma after all items except for the last one
                                    if (i !== selectedToppingsCount - 1) {
                                        selectedItemsString += ", ";
                                    }

                                    // add 'and' afer the second to last item
                                    if (i === selectedToppingsCount - 2) {
                                        selectedItemsString += "and ";
                                    }
                                }
                            }

                            selectedItemsString += " to the cart.";
                            WinJS.log && WinJS.log(selectedItemsString, "sample", "status");

                            // Clear selection in both ListViews, allowing customer to continue to create custom cartons
                            flavorsView.selection.clear();
                            toppingsView.selection.clear();
                        });
                }
            }

            element.querySelector("#addToCartButton").addEventListener("click", addItemToCart, false);
        }
    });
})();
