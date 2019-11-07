//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {

            var pivotEl = document.getElementById("pivotScenario3");
            pivotEl.addEventListener('click', function (ev) {
                var src = ev.srcElement;
                if (WinJS.Utilities.hasClass(src, "selectionmodeHitTarget")) {
                    var listViewEl = src;
                    while (listViewEl && !WinJS.Utilities.hasClass(listViewEl, "win-listview")) {
                        listViewEl = listViewEl.parentNode;
                    }

                    if (listViewEl) {
                        var listView = listViewEl.winControl;
                        listView.selection.add(listView.indexOfElement(ev.srcElement.parentNode.parentNode));
                        toggleSelectionMode(listView);
                    }
                }
            });

            pivotEl.addEventListener('selectionchanged', function (ev) {
                var src = ev.srcElement;
                if (WinJS.Utilities.hasClass(src, "win-listview")) {
                    var listView = src.winControl;
                    if (listView.selection.count() === 0) {
                        listView.shouldExitSelectionMode = true;
                    }
                    else {
                        listView.shouldExitSelectionMode = false;
                    }
                }
            });

            pivotEl.addEventListener('iteminvoked', function (ev) {
                var listViewEl = ev.srcElement;
                while (listViewEl && !WinJS.Utilities.hasClass(listViewEl, 'win-listview')) {
                    listViewEl = listViewEl.parentNode;
                }

                if (listViewEl) {
                    var listView = listViewEl.winControl;
                    if (listView.selectionModeActive) {
                        if (listView.shouldExitSelectionMode) {
                            listView.shouldExitSelectionMode = false;
                            toggleSelectionMode(listView);
                        }

                        // Stop the invoke event from firing
                        ev.stopPropagation();
                    }
                    else {
                        //send an invoke console event
                        ev.detail.itemPromise.done(function (invokedItem) {

                            // Access item data from the itemPromise
                            WinJS.log && WinJS.log("Item at index " + invokedItem.index + " was invoked", "sample", "status");
                        });
                    }
                }
            }, true);

            function toggleSelectionMode(listView) {
                if (listView.selectionModeActive) {
                    listView.selectionModeActive = false;
                    pivotEl.winControl.locked = false;
                    listView.tapBehavior = WinJS.UI.TapBehavior.invokeOnly;
                    listView.selectionMode = WinJS.UI.SelectionMode.none;
                    listView.selection.clear();
                } else {
                    listView.selectionModeActive = true;
                    pivotEl.winControl.locked = true;
                    listView.tapBehavior = WinJS.UI.TapBehavior.toggleSelect;
                    listView.selectionMode = WinJS.UI.SelectionMode.multi;
                }
                return listView.selectionModeActive;
            };
        }
    });


})();
