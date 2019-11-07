//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var appBar;

    var page = WinJS.UI.Pages.define("/html/transparent-appbar.html", {
        ready: function (element, options) {
            document.body.style.backgroundImage = "url('../images/60Banana.png')";
            document.querySelector('link').setAttribute('href', "../css/ui-themed.theme-light.css");

            appBar = document.getElementById("transparentAppBar").winControl;
            appBar.getCommandById("cmdAdd").addEventListener("click", doClickAdd, false);
            appBar.getCommandById("cmdRemove").addEventListener("click", doClickRemove, false);
            appBar.getCommandById("cmdEdit").addEventListener("click", doClickEdit, false);
            appBar.getCommandById("cmdCamera").addEventListener("click", doClickCamera, false);

            // Opt out from the default behavior and make system chrome overlap with the app content
            Windows.UI.ViewManagement.ApplicationView.getForCurrentView().setDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.useCoreWindow);
        }, 

        dispose: function () {
            document.body.style.backgroundImage = "";
            document.querySelector('link').setAttribute('href', "../css/ui-themed.css");

            // Revert to default behavior.  System chrome will not overlap with the app content.
            Windows.UI.ViewManagement.ApplicationView.getForCurrentView().setDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.useVisible);
        }
    });

    // Command button functions
    function doClickAdd() {
        WinJS.log && WinJS.log("Add button pressed", "sample", "status");
    }

    function doClickRemove() {
        WinJS.log && WinJS.log("Remove button pressed", "sample", "status");
    }

    function doClickEdit() {
        WinJS.log && WinJS.log("Edit button pressed", "sample", "status");
    }

    function doClickCamera() {
        WinJS.log && WinJS.log("Camera button pressed", "sample", "status");
    }
})();
