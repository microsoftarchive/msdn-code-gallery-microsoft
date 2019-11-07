//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var appBar;

    var page = WinJS.UI.Pages.define("/html/custom-content.html", {
        ready: function (element, options) {
            appBar = document.getElementById("customContentAppBar").winControl;           
            appBar.getCommandById("cmdAdd").addEventListener("click", doClickAdd, false);
            appBar.getCommandById("cmdRemove").addEventListener("click", doClickRemove, false);
            appBar.getCommandById("cmdFavorites").addEventListener("click", doClickFavorites, false);

        }
    });

    // Command button functions
    function doClickAdd() {
        WinJS.log && WinJS.log("Add button pressed", "sample", "status");
    }

    function doClickRemove() {
        WinJS.log && WinJS.log("Remove button pressed", "sample", "status");
    }

    function doClickFavorites() {
        WinJS.log && WinJS.log("Favorites button pressed", "sample", "status");
    }
})();
