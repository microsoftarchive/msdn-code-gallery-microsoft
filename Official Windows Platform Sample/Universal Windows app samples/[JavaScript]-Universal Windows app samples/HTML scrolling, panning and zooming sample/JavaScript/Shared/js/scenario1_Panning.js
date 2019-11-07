//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_Panning.html", {
        processed: function (element, options) {
            // WinJS.UI.processAll() is automatically called by the Pages infrastructure by the time this
            // function gets called, and it has processed the div with data-win-control="WinJS.UI.FlipView"

            // Attach Necessary Event Handlers to control changing the panning type
            document.getElementById("PanningType").addEventListener("change", changePanningType, false);
            document.getElementById("ScrollbarStyle").addEventListener("change", changeOverflowStyle, false);
        }
    });

    // 'changePanningType(event)'
    //
    //      Purpose: Event handler for select control change event.  The
    //               function swaps CSS classes to demonstrate different values for 
    //               overflow and -ms-scroll-rails.
    function changePanningType(changedEvent) {

        // First step is to get the element we want to change
        var myScrollElement = document.getElementById("Scenario1_Scroller");

        // Next, get the index so we know what scenario we are setting
        var panType = changedEvent.target.options.value;

        // Finally, figure out and set the class which will in turn set
        // the overflow and rails properties. 
        myScrollElement.className = "ManipulationContainer " + panType;
    }

    // 'changePanningType(event)'
    //
    //      Purpose: Event handler for select control change event.  The
    //               function swaps CSS classes to demonstrate different values for 
    //               overflow and -ms-scroll-rails.
    function changeOverflowStyle(changedEvent) {
        // First step is to get the element we want to change
        var myScrollElement = document.getElementById("Scenario1_Scroller");

        // Set the overflow style to what was selected.
        myScrollElement.style["-ms-overflow-style"] = changedEvent.target.options.value;
    }
})();
