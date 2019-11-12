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
            var listView = element.querySelector('#listView').winControl;

            function reportSelection() {

                // Get the number of currently selected items
                var selectionCount = listView.selection.count();

                // Report the number
                if (selectionCount > 0) {

                    // Get the actual selected items
                    listView.selection.getItems().done(function (currentSelection) {
                        var itemsSelectedString = selectionCount + " ";
                        if (selectionCount === 1) {

                            // only one selected
                            itemsSelectedString += "item is currently selected. Specifically, item ";

                        } else {

                            // more than one selected
                            itemsSelectedString += "items are currently selected. Specifically, items ";
                        }

                        // Iterate through all selected items adding them to the output
                        var numItemsReported = 0;
                        currentSelection.forEach(function (selectedItem) {
                            numItemsReported++;
                            if (numItemsReported !== selectionCount) {

                                // Print the item index followed by a ','
                                // since there are more items after this one
                                itemsSelectedString += selectedItem.index + ", ";

                            } else {

                                // Print something different when at the last item
                                itemsSelectedString += selectedItem.index;

                                // Since this is the last item, print the ending phrase
                                if (selectionCount === 1) {

                                    // only one item was selected
                                    itemsSelectedString += " is selected.";

                                } else {

                                    // more than one item was selected
                                    itemsSelectedString += " are selected.";
                                }
                            }
                        });

                        WinJS.log && WinJS.log(itemsSelectedString, "sample", "status");
                    });

                } else {

                    //no items selected
                    WinJS.log && WinJS.log("No items are currently selected.", "sample", "status");
                }
            }

            function selectAll() {
                WinJS.log && WinJS.log(" ", "sample", "status");
                listView.selection.selectAll();
            }

            function clearSelection() {
                WinJS.log && WinJS.log(" ", "sample", "status");
                listView.selection.clear();
            }

            element.querySelector("#reportSelection").addEventListener("click", reportSelection, false);
            element.querySelector("#selectAll").addEventListener("click", selectAll, false);
            element.querySelector("#clearSelection").addEventListener("click", clearSelection, false);
        }
    });
})();
