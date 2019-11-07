//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var appBar;

    var page = WinJS.UI.Pages.define("/html/appbar-commands.html", {
        ready: function (element, options) {
            document.getElementById('scenarioShowButtons').addEventListener("click", doShowItems, false);
            document.getElementById('scenarioHideButtons').addEventListener("click", doHideItems, false);

            appBar = document.getElementById("commandsAppBar").winControl;
            appBar.getCommandById('cmdAdd').addEventListener("click", doClickAdd, false);
            appBar.getCommandById('cmdRemove').addEventListener("click", doClickRemove, false);
            appBar.getCommandById('cmdFavorite').addEventListener("click", doClickFavorite, false);
            appBar.getCommandById('cmdCamera').addEventListener("click", doClickCamera, false);
            // Set the default state of scenario buttons
            document.getElementById('scenarioShowButtons').disabled = true;
            document.getElementById('scenarioHideButtons').disabled = false;
            // Set the default state of all the AppBar
            appBar.sticky = true;
        }
    });

    // Command button functions
    function doClickAdd() {
        WinJS.log && WinJS.log("Add button pressed", "sample", "status");
    }

    function doClickRemove() {
        WinJS.log && WinJS.log("Remove button pressed", "sample", "status");
    }

    function doClickFavorite() {
        WinJS.log && WinJS.log("Favorite button pressed", "sample", "status");
    }

    function doClickCamera() {
        WinJS.log && WinJS.log("Camera button pressed", "sample", "status");
    }

    // These functions are used by the scenario to show and hide elements
    function doShowItems() {
        document.getElementById('commandsAppBar').winControl.showCommands([cmdFavorite, cmdCamera]);
        document.getElementById('scenarioShowButtons').disabled = true;
        document.getElementById('scenarioHideButtons').disabled = false;
    }

    function doHideItems() {
        document.getElementById('commandsAppBar').winControl.hideCommands([cmdFavorite, cmdCamera]);
        document.getElementById('scenarioHideButtons').disabled = true;
        document.getElementById('scenarioShowButtons').disabled = false;
    }
})();
