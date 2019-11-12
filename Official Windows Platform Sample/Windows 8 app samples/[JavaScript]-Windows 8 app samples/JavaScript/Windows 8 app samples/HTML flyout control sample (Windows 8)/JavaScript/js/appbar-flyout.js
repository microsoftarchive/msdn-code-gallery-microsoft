//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/appbar-flyout.html", {
        ready: function (element, options) {
            // Attach event handlers for the buttons in this scenario, including in the AppBar and on the menu
            document.getElementById("confirmDeleteButton").addEventListener("click", confirmDelete, false);
            document.getElementById("alwaysSaveMenuItem").addEventListener("click", alwaysSave, false);
            document.getElementById("replyMenuItem").addEventListener("click", reply, false);
            document.getElementById("replyAllMenuItem").addEventListener("click", replyAll, false);
            document.getElementById("forwardMenuItem").addEventListener("click", forward, false);
            document.getElementById("appBar").addEventListener("beforeshow", clearStatus, false);

            WinJS.log && WinJS.log("To show the bar, swipe up from the bottom of the screen, right-click, or press Windows Key+Z. To dismiss the bar, tap in the application, swipe, right-click, or press Windows Logo+Z again.", "sample", "status");
        
        },
        unload: function() {
            // Unload the appbar when the user has navigated away so that it does not show in other scenarios.
            var appbar = document.getElementById("appBar");
            appbar.parentNode.removeChild(appbar);
        }
    });

    // Hide the appbar and the flyout
    function hideFlyoutAndAppBar() {
        document.getElementById("respondFlyout").winControl.hide();
        document.getElementById("appBar").winControl.hide();
    }

    // When delete is clicked, hide the appbar and flyout because the user is likely done with them
    function confirmDelete() {
        WinJS.log && WinJS.log("You have deleted the item.", "sample", "status");
        hideFlyoutAndAppBar();
    }

    // When always save is toggled, hide the appbar and flyout because the user is likely done with them
    function alwaysSave() {
        var alwaysSaveState = document.getElementById("alwaysSaveMenuItem").winControl.selected;
        WinJS.log && WinJS.log("The Always save option is now set to: " + alwaysSaveState + ".", "sample", "status");

        hideFlyoutAndAppBar();
    }

    // When reply is clicked, hide the appbar and flyout because the user is likely done with them
    function reply() {
        WinJS.log && WinJS.log("You replied to the message.", "sample", "status");
        hideFlyoutAndAppBar();
    }

    // When reply all is clicked, hide the appbar and flyout because the user is likely done with them
    function replyAll() {
        WinJS.log && WinJS.log("You replied all the message.", "sample", "status"); 
        hideFlyoutAndAppBar();
    }

    // When forward is clicked, hide the appbar and flyout because the user is likely done with them
    function forward() {
        WinJS.log && WinJS.log("You forwarded the message.", "sample", "status");
        hideFlyoutAndAppBar();
    }

    function clearStatus() {
        WinJS.log && WinJS.log("", "sample", "status");
    }

})();
