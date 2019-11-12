//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/instantiableGesture.html", {
        ready: function (element, options) {
            document.getElementById("button2").addEventListener("click", initialize, false);
        }
    });

    function initialize() {
        // Defining all the gesture elements capable of transformations other than the background container - Body
        var gestureElts = document.getElementsByClassName("GestureElement");
        for (var i = 0; i < gestureElts.length; i++) {
            var elt = gestureElts[i];
            var gObj = new MSGesture();             // Creating a gesture object for each element

            gObj.target = elt;
            gObj.srcElt = elt;
            elt.gesture = gObj;                     // Expando gesture poroperty for each element.
            elt.gesture.pointerType = null;         // Expando property to capture pointer type to handle multiple pointer sources

            // Creating event listeners for gesture elements
            elt.addEventListener("pointerdown", onPointerDown, false);
            elt.addEventListener("MSGestureTap", onTap, false);
            elt.addEventListener("MSGestureHold", onHold, false);
            elt.addEventListener("MSGestureChange", onGestureChange, false);
            elt.addEventListener("MSGestureEnd", onGestureEnd, false);
        }

        // Defining background container - body
        var width = 640;
        var height = 360;
        var element = document.getElementById("Container");

        // Styles and color for container - body
        element.style.width = width + "px";
        element.style.height = height + "px";
        element.style.backgroundColor = "rgb(0, 0, 0)";

        var containerObject = new MSGesture();
        containerObject.target = element;
        containerObject.srcElt = element;
        element.gesture = containerObject;
        element.gesture.pointerType = null;         // Expando property to capture pointer type to handle multiple pointer sources

        // Creating event listeners for body
        element.addEventListener("pointerdown", onPointerDownBody, false);
        element.addEventListener("MSGestureHold", onHoldBody, false);
        element.addEventListener("MSGestureTap", onTapBody, false);
        element.addEventListener("MSGestureChange", onGestureChangeBody, false);
        element.addEventListener("MSGestureEnd", onGestureEnd, false);
        layoutElts(gestureElts);
    }

    var colors = ["rgb(0, 140, 0)", "rgb(0, 98, 140)", "rgb(194, 74, 0)", "rgb(89, 0, 140)", "rgb(191, 150, 0)", "rgb(140, 0, 0)"];

    // Drawing out gesture elements
    function layoutElts(elts) {
        var container = document.getElementById("Container");
        var size = Math.min(parseInt(container.style.height), parseInt(container.style.width));
        var height = parseInt(container.style.height);
        var width = parseInt(container.style.width);

        elts[0].style.top = height / 2 - size / 2 + "px";
        elts[0].style.left = width / 2 - size / 2 + "px";
        elts[1].style.top = height / 2 - size / 2 + "px";
        elts[1].style.left = width / 2 + "px";
        elts[2].style.top = height / 2 + "px";
        elts[2].style.left = width / 2 - size / 2 + "px";
        elts[3].style.top = height / 2 + "px";
        elts[3].style.left = width / 2 + "px";

        for (var i = 0; i < elts.length; i++) {
            elts[i].style.backgroundColor = colors[i];
            elts[i].originalColorIndex = i;
            elts[i].originalTransform = elts[i].style.transform;
            elts[i].style.zIndex = i;       // For ZOrder - whichever element tapped becomes the topmost element for drag
        }
    }

    // Handler for pointer down on gesture element
    function onPointerDown(e) {
        if (e.target.gesture.pointerType === null) {                    // First contact
            e.target.gesture.addPointer(e.pointerId);                   // Attaches pointer to element (e.target is the element)
            e.target.gesture.pointerType = e.pointerType;
        }
        else if (e.target.gesture.pointerType === e.pointerType) {      // Contacts of similar type
            e.target.gesture.addPointer(e.pointerId);                   // Attaches pointer to element (e.target is the element)
        }

        // ZIndex Changes on pointer down. Element on which pointer comes down becomes topmost
        var zOrderCurr = e.target.style.zIndex;
        var elts = document.getElementsByClassName("GestureElement");
        for (var i = 0; i < elts.length; i++) {
            if (elts[i].style.zIndex === 3) {
                elts[i].style.zIndex = zOrderCurr;
            }
            e.target.style.zIndex = 3;
        }
    }

    // Handler for pointer down on body
    function onPointerDownBody(e) {
        if (e.target === this) {
            if (e.target.gesture.pointerType === null) {                    // First contact
                e.target.gesture.addPointer(e.pointerId);                   // Attaches pointer to element (e.target is the element)
                e.target.gesture.pointerType = e.pointerType;
            }
            else if (e.target.gesture.pointerType === e.pointerType) {      // Contacts of similar type
                e.target.gesture.addPointer(e.pointerId);                   // Attaches pointer to element (e.target is the element)
            }
        }
    }
    
    // Handler for gesture end for both body and elements
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

    // Handler for Tap gesture on body - container goes back to original transform onTap
    function onTapBody(e) {
        // One of the ways to prevent child (gesture elements) from executing function during bubble phase
        if (e.target.className !== "GestureElement") {
            e.target.style.transform = new MSCSSMatrix();
        }
        
        // MSGestureEnd isn't generated at the end of Tap
        onGestureEnd(e);
    }

    // Handler for Hold gesture on gesture elements - Elements gain original size, color and transform onHold
    function onHold(e) {
        if (e.detail === e.MSGESTURE_FLAG_END) {
            var elt = e.target;
            if (elt.className === "GestureElement") {
                elt.style.backgroundColor = colors[elt.originalColorIndex];
            }
            elt.style.transform = elt.originalTransform;
            
            // MSGestureEnd isn't generated at the end of Hold
            onGestureEnd(e);
        }
    }

    // Handler for Hold gesture on body - Container and elements get reintialized to original form ( equal to sample "reset")
    function onHoldBody(e) {
        if (e.detail === e.MSGESTURE_FLAG_END) {
            if (e.target.className !== "GestureElement") {
                e.target.style.transform = new MSCSSMatrix();
                var elts = document.getElementsByClassName("GestureElement");
    
                for (var i = 0; i < elts.length; i++) {
                    elts[i].style.backgroundColor = colors[elts[i].originalColorIndex];
                    elts[i].style.transform = elts[i].originalTransform;
                }
            }
            
            //MSGestureEnd isn't generated at the end of Hold
            onGestureEnd(e);
        }
    }

    // Handler for transformation on gesture elements
    function onGestureChange(e) {
        var elt = e.target;
        var m = new MSCSSMatrix(elt.style.transform);

        elt.style.transform = m.
            translate(e.offsetX, e.offsetY).
            translate(e.translationX, e.translationY).
            rotate(e.rotation * 180 / Math.PI).
            scale(e.scale).
            translate(-e.offsetX, -e.offsetY);
    }

    // Handler for transformations on body
    function onGestureChangeBody(e) {
        if (e.target.className !== "GestureElement") {
            var elt = e.target;
            var m = new MSCSSMatrix(elt.style.transform);

            e.target.style.transform = m.
                translate(e.offsetX, e.offsetY).
                translate(e.translationX, e.translationY).
                rotate(e.rotation * 180 / Math.PI).
                scale(e.scale).
                translate(-e.offsetX, -e.offsetY);
        }
    }

})();
