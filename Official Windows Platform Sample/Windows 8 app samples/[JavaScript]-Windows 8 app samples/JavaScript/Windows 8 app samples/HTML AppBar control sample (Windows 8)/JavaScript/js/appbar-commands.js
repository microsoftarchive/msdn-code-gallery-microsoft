//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/appbar-commands.html", {
        ready: function (element, options) {
            document.getElementById('scenarioShowBar').addEventListener("click", doShowBar, false);
            document.getElementById('scenarioShowButtons').addEventListener("click", doShowItems, false);
            document.getElementById('scenarioHideButtons').addEventListener("click", doHideItems, false);
            document.getElementById('cmdAdd').addEventListener("click", doClickAdd, false);
            document.getElementById('cmdRemove').addEventListener("click", doClickRemove, false);
            document.getElementById('cmdDelete').addEventListener("click", doClickDelete, false);
            document.getElementById('cmdCamera').addEventListener("click", doClickCamera, false);
            WinJS.log && WinJS.log("To show the bar, press the Show Bar button, swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, swipe, right-click, or press Windows Logo + z again.", "sample", "status");
            // Set the default state of scenario buttons
            document.getElementById('scenarioShowButtons').disabled = true;
            document.getElementById('scenarioHideButtons').disabled = true;
            // Set the default state of all the AppBar
            document.getElementById('commandsAppBar').winControl.sticky = true;
            // Listen for the AppBar events and enable and disable the buttons if the bar is shown or hidden
            document.getElementById('commandsAppBar').winControl.addEventListener("aftershow", scenarioBarShown, false);
            document.getElementById('commandsAppBar').winControl.addEventListener("beforehide", scenarioBarHidden, false);
            
        },
        unload: function () {
            AppBarSampleUtils.removeAppBars();
        }
    });

    // Command button functions
    function doClickAdd() {
        WinJS.log && WinJS.log("Add button pressed", "sample", "status");
    }

    function doClickRemove() {
        WinJS.log && WinJS.log("Remove button pressed", "sample", "status");
    }

    function doClickDelete() {
        WinJS.log && WinJS.log("Delete button pressed", "sample", "status");
    }

    function doClickCamera() {
        WinJS.log && WinJS.log("Camera button pressed", "sample", "status");
    }

    function doShowBar() {
        document.getElementById('commandsAppBar').winControl.show();
    }


    // These functions are used by the scenario to show and hide elements
    function doShowItems() {
        document.getElementById('commandsAppBar').winControl.showCommands([cmdAdd, cmdRemove, appBarSeparator, cmdDelete]);
        document.getElementById('scenarioShowButtons').disabled = true;
        document.getElementById('scenarioHideButtons').disabled = false;
    }

    function doHideItems() {
        document.getElementById('commandsAppBar').winControl.hideCommands([cmdAdd, cmdRemove, appBarSeparator, cmdDelete]);
        document.getElementById('scenarioHideButtons').disabled = true;
        document.getElementById('scenarioShowButtons').disabled = false;
    }

    // These functions are used by the scenario to disable and enable the scenario buttons when the AppBar shows and hides
    function scenarioBarShown() {
        document.getElementById('scenarioShowBar').disabled = true;
        if (document.getElementById('cmdAdd').style.visibility === "hidden") {
            document.getElementById('scenarioShowButtons').disabled = false;
        } else {
            document.getElementById('scenarioHideButtons').disabled = false;
        }
    }

    function scenarioBarHidden() {
        document.getElementById('scenarioShowBar').disabled = false;
        document.getElementById('scenarioShowButtons').disabled = true;
        document.getElementById('scenarioHideButtons').disabled = true;
    }

})();
