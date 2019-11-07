//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            element.querySelector("#shuffle").addEventListener("click", shuffleTiles, false);
            element.querySelector("#removeSelected").addEventListener("click", removeSelected, false);
            element.querySelector("#swapSelected").addEventListener("click", swapSelected, false);
            element.querySelector("#addTile").addEventListener("click", addTile, false);

            initTiles();
        }
    });

    var _tIndex = 0;
    //Create an array of the letters in the alphabet
    var _letterSrc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".split("");

    // Initializes the first set of tiles
    // Uses an array that will be converted into a Binding.List
    function initTiles() {
        var letters = [];
        for (var i = 0; i < 7; i++) {
            letters[i] = generateTile();
        }
        var lettersList = new WinJS.Binding.List(letters);
        var list2 = document.getElementById("listView3").winControl;

        list2.itemDataSource = lettersList.dataSource;
        list2.itemTemplate = document.getElementById("tileTemplate");
        list2.forceLayout();
    }

    // Generates a tile with a random letter and counter
    function generateTile() {
        var tile = {
            letter: _letterSrc[Math.floor(Math.random() * _letterSrc.length)],
            counter: _tIndex
        };
        _tIndex++;

        return tile;
    }

    // Shuffles the order of the tiles
    function shuffleTiles() {
        var ds = document.getElementById("listView3").winControl.itemDataSource;

        //Get the count of the tiles (async)
        ds.getCount().done(function (count) {
            if (count > 0) {

                // Create a list binding which provides the query functionality
                var binding = ds.createListBinding();

                // Item manipulation requires a key, so convert the indices to keys
                // Form an array of all the item keys, and promises
                var keys = [], p = [];
                for (var i = 0; i < count; i++) {

                    // Need to wrap the loop variable in a function to ensure correct value when the async call actually occurs
                    (function (j) {

                        // Get the items based on the index (async)
                        p[j] = binding.fromIndex(j).then(function (currentItem) {
                            keys[j] = currentItem.key;
                        });
                    })(i);
                }

                // The fromIndex's can be async, so use a join on the collection of promises to wait for them all to be complete
                WinJS.Promise.join(p).done(function () {

                    // We're done with the binding so it can be released
                    binding.release();

                    // Start a batch for the edits
                    ds.beginEdits();

                    // To shuffle the items, we pick one from the array of keys, then move it to the front of the list
                    // and then continue for the remaining items
                    for (var itemIndex = 0; itemIndex < count; itemIndex++) {

                        // find an item from the array of keys
                        var randomIndex = Math.floor(Math.random() * (count - itemIndex));
                        if (randomIndex < 0 || randomIndex === keys.length) { debugger; }
                        var key = keys[randomIndex];

                        //remove key from the array
                        keys.splice(randomIndex, 1);

                        //move the item to the start
                        ds.moveToStart(key);
                    }

                    // End the batch of edits
                    ds.endEdits();
                });
            }
        });
    }

    // Remove the selected tile
    function removeSelected() {
        // Get the control, itemDataSource and selected items
        var list2 = document.getElementById("listView3").winControl;
        var ds = list2.itemDataSource;

        if (list2.selection.count() > 0) {
            list2.selection.getItems().done(function (items) {

                // Start a batch for the edits
                ds.beginEdits();

                // To remove the items, call remove on the itemDataSource passing in the key
                items.forEach(function (currentItem) {
                    ds.remove(currentItem.key);
                });

                // End the batch of edits
                ds.endEdits();
            });
        }
    }

    // Demonstrates how to modify data for the selected items
    function swapSelected() {
        // Get the control, itemDataSource and selected items
        var list2 = document.getElementById("listView3").winControl;
        var ds = list2.itemDataSource;

        if (list2.selection.count() > 0) {
            list2.selection.getItems().done(function (items) {
                // Start a batch for the edits
                ds.beginEdits();

                // To edit the item, call change on the itemDataSource passing in the key and new data
                items.forEach(function (currentItem) {
                    ds.change(currentItem.key, generateTile());
                });

                // End the batch of edits
                ds.endEdits();
            });
        }
    }

    // Adds a new tile to the end of the collection
    function addTile() {
        var ds = document.getElementById("listView3").winControl.itemDataSource;

        //Begin a batch for the edits
        ds.beginEdits();

        //Create the data for the tile
        var tile = generateTile();

        //Insert it, and ignore the key as that is created by the data source
        ds.insertAtEnd(null, tile);

        // End the batch of edits
        ds.endEdits();
    }

})();
