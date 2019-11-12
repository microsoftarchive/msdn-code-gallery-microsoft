//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/orientationAndItemSpacing.html", {
        processed: function (element, options) {
            // WinJS.UI.processAll() is automatically called by the Pages infrastructure by the time this
            // function gets called, and it has processed the div with data-win-control="WinJS.UI.FlipView"

            // Attach necessary Event Handlers to control modifying FlipView properties.
            document.getElementById("OrientationButton").addEventListener("click", swapOrientation, false);
            document.getElementById("SpaceRange").addEventListener("change", updateItemGaps, false);
        }
    });

    // This function swaps between different values of orientation of the FlipView
    function swapOrientation(evt) {

        // First step is to get the FlipView we want to change
        var myFlipView = document.getElementById("orientationAndItemSpacing_FlipView").winControl;

        // Next Step is to check the current orientation, switch it to
        // the other orientation and then switch the button text.
        if (myFlipView.orientation === "horizontal") {
            myFlipView.orientation = "vertical";
            evt.srcElement.innerText = "Horizontal";

        } else {
            myFlipView.orientation = "horizontal";
            evt.srcElement.innerText = "Vertical";
        }
    }

    // This event handler retrieves the value from the range control and
    // applies it to the FlipView.
    function updateItemGaps(evt) {

        // First Step is to get the FlipView we want to change
        var myFlipView = document.getElementById("orientationAndItemSpacing_FlipView").winControl;

        // Next step is to get the new Item Gap Size
        var space = evt.srcElement.value;

        // Final step is to set the new spacing on the FlipView
        myFlipView.itemSpacing = parseInt(space);
    }
})();
