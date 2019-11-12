//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var appBar;

    var page = WinJS.UI.Pages.define("/html/localize-appbar.html", {
        ready: function (element, options) {
            // Load resources and enable listener so they get updated when user changes language selection
            loadResources();
            WinJS.Resources.addEventListener("contextchanged", loadResources);

            appBar = document.getElementById("localizedAppBar").winControl;
            appBar.getCommandById("cmdCommand1").addEventListener("click", doClickCommand1, false);
            appBar.getCommandById("cmdCommand2").addEventListener("click", doClickCommand2, false);
        },
        unload: function () {
            // Remove listener, no other documents have resources
            WinJS.Resources.removeEventListener("contextchanged", loadResources);
        }
    });

    // Handles loading of resources
    function loadResources() {
        WinJS.Resources.processAll();
    }

    // Command button functions
    function doClickCommand1() {
        WinJS.log && WinJS.log("Localized Command1 button pressed", "sample", "status");
    }

    function doClickCommand2() {
        WinJS.log && WinJS.log("Localized Command2 button pressed", "sample", "status");
    }

})();
