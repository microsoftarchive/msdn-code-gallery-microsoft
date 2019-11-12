//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/mousewheel.html", {
        ready: function (element, options) {
            document.getElementById("button3").addEventListener("click", initialize, false);
        }
    });
    
    var colors = ["rgb(0, 140, 0)", "rgb(0, 98, 140)", "rgb(194, 74, 0)", "rgb(89, 0, 140)", "rgb(191, 150, 0)", "rgb(140, 0, 0)"];

    // Initialization
    function initialize() {
        var elt = document.getElementById("ge1");

        // Define original settings for element
        elt.style.backgroundColor = colors[0];
        elt.originalTransform = elt.style.transform;

        /*
         *  Mouse, Pen and Touch are similar "locator" type input, while mousewheel is not. In some cases, it can be
         *  very complex to have the same gesture object to handle both mousewheel and pointer input. Having a dedicated
         *  gesture object for mousewheel would be a lot easier in such cases.
         */
        var gObj = new MSGesture();           // Gesture object for Pen, Mouse, Touch, and Mouse Wheel

        // Defining gesture object for Pen, mouse and touch
        gObj.target = elt;
        elt.gesture = gObj;
        elt.gesture.pointerType = null;

        // Creating event listeners for gesture elements
        elt.addEventListener("pointerdown", onPointerDown, false);
        elt.addEventListener("MSGestureTap", onTap, false);
        elt.addEventListener("MSGestureHold", onHold, false);
        elt.addEventListener("MSGestureChange", onGestureChange, false);
        elt.addEventListener("wheel", onMouseWheel, false);
        
        // Mouse Wheel does not generate onPointerUp
        elt.addEventListener("MSGestureEnd", onGestureEnd, false); 
    }

    function onPointerDown(e) {
        if (e.target.gesture.pointerType === null) {                    // First contact
            e.target.gesture.addPointer(e.pointerId);                   // Attaches pointer to element (e.target is the element)
            e.target.gesture.pointerType = e.pointerType;
        }
        else if (e.target.gesture.pointerType === e.pointerType) {      // Contacts of similar type
            e.target.gesture.addPointer(e.pointerId);                   // Attaches pointer to element (e.target is the element)
        }
    }

    // Handler for MouseWheel Event 
    function onMouseWheel(e) {
        e.pointerId = 1;                 // Fixed pointerId for MouseWheel
        onPointerDown(e);
    }
    
    // Handler for GestureEnd event
    function onGestureEnd(e) {
        e.target.gesture.pointerType = null;
    }

    // Handler for Tap gesture on gesture elements - Elements change color onTap
    function onTap(e) {
        var elt = e.target;
        var i = (colors.indexOf(elt.style.backgroundColor) + 1) % colors.length;
        elt.style.backgroundColor = colors[i];
        
        // MSGestureEnd isn't generated at the end of Tap
        onGestureEnd(e);
    }

    // Handler for gesture movements on the element.
    function onGestureChange(e) {
        var elt = e.target;
        var m = new MSCSSMatrix(elt.style.transform);

        // No Differentiation in mousewheel gestures from normal pointer down gestures
        elt.style.transform = m.
            translate(e.offsetX, e.offsetY).
            translate(e.translationX, e.translationY).
            rotate(e.rotation * 180 / Math.PI).
            scale(e.scale).
            translate(-e.offsetX, -e.offsetY);
    }
    
    // Handler for Hold gesture on gesture elements - Elements gain original size, color and transform onHold
    function onHold(e) {
        if (e.detail === e.MSGESTURE_FLAG_END) {
            var elt = e.target;
            elt.style.backgroundColor = colors[0];
            elt.style.transform = elt.originalTransform;
            
            // MSGestureEnd isn't generated at the end of Hold
            onGestureEnd(e);
        }
    }

})();
