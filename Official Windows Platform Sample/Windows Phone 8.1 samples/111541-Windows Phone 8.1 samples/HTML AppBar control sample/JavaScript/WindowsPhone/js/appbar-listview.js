//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var appBar;

    var page = WinJS.UI.Pages.define("/html/appbar-listview.html", {
        ready: function (element, options) {
            initAppBar();
            initListView();

            // Listen to hardware back button press
            WinJS.Application.addEventListener("backclick", handleBack);
        },
        dispose: function () {
            WinJS.Application.removeEventListener("backclick", handleBack);
        }
    });
    
    function initListView() {
        var scenarioDiv = document.getElementById("scenarioFullscreen");

        scenarioDiv.addEventListener('click', function (ev) {
            var src = ev.srcElement;
            if (WinJS.Utilities.hasClass(src, "hitTarget")) {
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

        scenarioDiv.addEventListener('selectionchanged', function (ev) {
            var src = ev.srcElement;
            if (WinJS.Utilities.hasClass(src, "win-listview")) {
                var listView = src.winControl;
                if (listView.selection.count() === 0) {
                    listView.shouldExitSelectionMode = true;
                } else {
                    listView.shouldExitSelectionMode = false;
                }
                updateAppBar();
            }
        });

        scenarioDiv.addEventListener('iteminvoked', function (ev) {
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
    }

    function toggleSelectionMode(listView) {
        if (listView.selectionModeActive) {
            listView.selectionModeActive = false;
            listView.tapBehavior = WinJS.UI.TapBehavior.invokeOnly;
            listView.selectionMode = WinJS.UI.SelectionMode.none;
            listView.selection.clear();
        } else {
            listView.selectionModeActive = true;
            listView.tapBehavior = WinJS.UI.TapBehavior.toggleSelect;
            listView.selectionMode = WinJS.UI.SelectionMode.multi;
        }
        updateAppBar();
        return listView.selectionModeActive;
    };

    function handleBack(ev) {
        var handled = ev.handled;
        if (!handled) {
            var listView = document.querySelector(".win-listview").winControl;

            if (listView.selectionModeActive) {
                toggleSelectionMode(listView);
                handled = true;
            } 
        }
        // When handled is true, WinJS.Navigation will ignore the backbutton so that we can dismiss transient UI without causing a navigation
        return handled;
    }

    /* AppBar functions */

    function initAppBar() {
        appBar = document.getElementById("scenarioAppBar").winControl;
        appBar.getCommandById("cmdSelect").addEventListener("click", doClickSelect, false);
        appBar.getCommandById("cmdSync").addEventListener("click", doClickSync, false);
        appBar.getCommandById("cmdDelete").addEventListener("click", doClickDelete, false);
        appBar.getCommandById("cmdMove").addEventListener("click", doClickMove, false);
        appBar.getCommandById("cmdSettings").addEventListener("click", doClickSettings, false);
    }

    function updateAppBar() {
        var listViewEl = document.querySelector(".win-listview");
        appBar = document.getElementById("scenarioAppBar").winControl;

        if (listViewEl && listViewEl.winControl.selectionModeActive) {
            appBar.showOnlyCommands(['cmdDelete', 'cmdMove', 'cmdSettings']);

            var lv = listViewEl.winControl;
            if (lv.selection.count() === 0) {
                // grey-out the trash can
                appBar.getCommandById('cmdDelete').disabled = true;
                appBar.getCommandById('cmdMove').disabled = true;
            } else {
                // show trash can
                appBar.getCommandById('cmdDelete').disabled = false;
                appBar.getCommandById('cmdMove').disabled = false;
            }
        } else {
            appBar.disabled = false;
            appBar.showOnlyCommands(['cmdSelect', 'cmdSync', 'cmdSettings']);
        }
    }

    function doClickSelect() {
        var listViewEl = document.querySelector(".win-listview");
        if (listViewEl) {
            var listView = listViewEl.winControl;
            toggleSelectionMode(listView);
        }

        WinJS.log && WinJS.log("Select button pressed", "sample", "status");
    }

    function doClickSync() {
        WinJS.log && WinJS.log("Sync button pressed", "sample", "status");
    }

    function doClickDelete() {
        WinJS.log && WinJS.log("Delete button pressed", "sample", "status");
    }

    function doClickMove() {
        WinJS.log && WinJS.log("Move button pressed", "sample", "status");
    }

    function doClickSettings() {
        WinJS.log && WinJS.log("Settings button pressed", "sample", "status");
    }
})();

