//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
// Array passed as the 'dataSource' argument to the ListViews in default.html

(function () {

    var listViewItems; // WinJS.Binding.List
    var appBar;

    "use strict";                   
    var page = WinJS.UI.Pages.define("/html/appbar-listview.html", {
        ready: function (element, options) {
            initAppBar();
            initListView();
        }
    });
    
    function initListView() {
        var listView = document.getElementById("scenarioListView").winControl;
        // Generate random items
        var items = [];
        for (var i = 0; i < 50; i++) {
            items[i] = generateItem();
        }
        listViewItems = new WinJS.Binding.List(items);
        listView.itemDataSource = listViewItems.dataSource;
        // Add event listeners
        document.getElementById("scenarioListView").addEventListener("selectionchanged", doSelectItem);
        document.getElementById("scenarioShowListView").addEventListener("click", doShowListView, false);
        document.getElementById("scenarioHideListView").addEventListener("click", doHideListView, false);
    }

    function initAppBar() {
        var appBarDiv = document.getElementById("scenarioAppBar");

        appBar = document.getElementById("scenarioAppBar").winControl;
        appBar.getCommandById("cmdAdd").addEventListener("click", doClickAdd, false);
        appBar.getCommandById("cmdDelete").addEventListener("click", doClickDelete, false);
        appBar.getCommandById("cmdSelectAll").addEventListener("click", doClickSelectAll, false);
        appBar.getCommandById("cmdClearSelection").addEventListener("click", doClickClearSelection, false);
        appBar.addEventListener("beforeshow", doAppBarShow, false);
        appBar.addEventListener("beforehide", doAppBarHide, false);        
        // Hide selection group of commands
        appBar.hideCommands(appBarDiv.querySelectorAll('.multiSelect'));
        // Disable AppBar until in full screen mode
        appBar.disabled = true;
    }
        
    function generateItem() {
        var type = Math.floor(Math.random() * 6);
        var tile;
        if (type === 0) {
            tile = { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "images/60Banana.png" };
        } else if (type === 1) {
            tile = { title: "Lavish Lemon Ice", text: "Sorbet", picture: "images/60Lemon.png" };
        } else if (type === 2) {
            tile = { title: "Marvelous Mint", text: "Gelato", picture: "images/60Mint.png" };
        } else if (type === 3) {
            tile = { title: "Creamy Orange", text: "Sorbet", picture: "images/60Orange.png" };
        } else if (type === 4) {
            tile = { title: "Very Vanilla", text: "Ice Cream", picture: "images/60Vanilla.png" };
        } else {
            tile = { title: "Succulent Strawberry", text: "Sorbet", picture: "images/60Strawberry.png" };
        }
        return tile;
    }

    /* AppBar functions */

    function doClickAdd() {
        WinJS.log && WinJS.log("Add button pressed", "sample", "status");
        var listView = document.getElementById("scenarioListView").winControl;
        var tile = generateItem();
        // Clear selection (if any)
        listView.selection.clear().done(function () {
            listViewItems.push(tile);
        });
    }

    function doClickDelete() {
        WinJS.log && WinJS.log("Delete button pressed", "sample", "status");
        var listView = document.getElementById("scenarioListView").winControl;
        if (listView.selection.count() > 0) {
            var indices = listView.selection.getIndices();
            for (var i = indices.length - 1; i >= 0; i--) {
                listViewItems.splice(indices[i], 1);
            }
        }
    }

    function doClickSelectAll() {
        var listView = document.getElementById("scenarioListView").winControl;
        listView.selection.selectAll();
    }

    function doClickClearSelection() {
        var listView = document.getElementById("scenarioListView").winControl;
        listView.selection.clear();
    }

    /* This function slides the ListView scrollbar into view if occluded by the AppBar (in sticky mode) */
    function doAppBarShow() {
        var listView = document.getElementById("scenarioListView");
        var appBarHeight = appBar.offsetHeight;
        // Move the scrollbar into view if appbar is sticky
        if (appBar.sticky) {
            var listViewTargetHeight = "calc(100% - " + appBarHeight + "px)";
            var transition = {
                property: 'height',
                duration: 367,
                timing: "cubic-bezier(0.1, 0.9, 0.2, 0.1)",
                to: listViewTargetHeight
            };
            WinJS.UI.executeTransition(listView, transition);
        }
    }

    /* This function slides the ListView scrollbar back to its original position */
    function doAppBarHide() {
        var listView = document.getElementById("scenarioListView");
        var appBarHeight = appBar.offsetHeight;
        // Move the scrollbar into view if appbar is sticky
        if (appBar.sticky) {
            var listViewTargetHeight = "100%";
            var transition = {
                property: 'height',
                duration: 367,
                timing: "cubic-bezier(0.1, 0.9, 0.2, 0.1)",
                to: listViewTargetHeight
            };
            WinJS.UI.executeTransition(listView, transition);
        }
    }

    /* ListView functions */
    
    function doSelectItem() {
        var appBarDiv = document.getElementById("scenarioAppBar");
        var listView =  document.getElementById("scenarioListView").winControl;
        var count = listView.selection.count();
        if (count > 0) {
            // Show selection commands in AppBar
            appBar.showCommands(appBarDiv.querySelectorAll('.multiSelect'));
            appBar.sticky = true;
            appBar.show();
        } else {
            // Hide selection commands in AppBar
            appBar.hide();
            appBar.hideCommands(appBarDiv.querySelectorAll('.multiSelect'));
            appBar.sticky = false;
        }
    }

    function doShowListView() {
        document.getElementById("scenarioFullscreen").style.visibility = 'visible';
        // Show the AppBar in full screen mode
        document.getElementById("scenarioAppBar").winControl.disabled = false;
    }

    function doHideListView() {
        document.getElementById("scenarioFullscreen").style.visibility = 'hidden';
        // Clear the ListView selection when exiting full screen mode
        doClickClearSelection();
        // Hide the AppBar when not in full screen mode
        document.getElementById("scenarioAppBar").winControl.disabled = true;
    }

})();
