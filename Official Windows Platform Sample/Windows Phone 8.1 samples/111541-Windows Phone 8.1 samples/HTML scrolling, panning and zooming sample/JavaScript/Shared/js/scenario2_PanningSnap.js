//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var scrollElement;
    var page = WinJS.UI.Pages.define("/html/scenario2_PanningSnap.html", {
        processed: function (element, options) {
            // WinJS.UI.processAll() is automatically called by the Pages infrastructure by the time this
            // function gets called, and it has processed the div with data-win-control="WinJS.UI.FlipView"

            // Attach Necessary Event Handlers to control changing the snap point type
            document.getElementById("snapTypeSelect").addEventListener("change", changeSnapType, false);
            scrollElement = document.getElementById("Scenario2_Scroller");
            attachMouseKeyboard("Scenario2_Scroller");
        }
    });

    
    // Function adds/removes classes in order to change the snap types.
    function changeSnapType(changedEvent) {
        if (WinJS.Utilities.hasClass(scrollElement, "MandatorySnapInterval")) {

            WinJS.Utilities.removeClass(scrollElement, "MandatorySnapInterval");
            WinJS.Utilities.addClass(scrollElement, "ProximitySnapList");

        } else {

            WinJS.Utilities.removeClass(scrollElement, "ProximitySnapList");
            WinJS.Utilities.addClass(scrollElement, "MandatorySnapInterval");

        }
    }
})();
