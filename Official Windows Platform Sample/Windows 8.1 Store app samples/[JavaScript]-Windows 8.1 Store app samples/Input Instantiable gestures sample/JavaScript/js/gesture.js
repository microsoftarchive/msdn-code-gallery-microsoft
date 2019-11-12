//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/gesture.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", initialize, false);
        }
    });

// This sample shows how gestures can be handled on different levels of the element tree.
// The overall scenario is a prototype for a jigsaw puzzle game, scoped down to focus on 
// how to manage interactions, but with comments pointing to how the sample could be expanded 
// into a full game app.
//     1) Only four pieces are presented (in a real game, this would be much larger)
//     2) The pieces are simple monochromatically colored squares (in a real game, they 
//        would have irregular edges and be painted with a scene)
//     3) Pieces can be independently and concurrently manipulated (panned, rotated and zoomed) 
//        (in a real game, it wouldn’t make sense to independently zoom pieces)
//     4) Pieces don’t interact with each other (in a real game, you would have logic to detect 
//        when pieces were close enough to be snapped together)

// The element tree consists of one element (a conceptual table top) which contains a set of 
// tile elements (the pieces). From an eventing perspective, we need to handle pointer down to 
// know when to create gesture objects and to target them to the tiles; we can do all this 
// work at the level of the container element.
//     1) In general, pointerdown means either:
//        a) If a gesture object already exists for the pointer target element, we add the 
//           new pointer to that gesture
//        b) If not, then we create a gesture object for the pointer target element and add 
//           the pointer to that gesture, and add event listeners for MSGesturexxx events to 
//           that element
//     2) Each tile will have a gesture object that incorporates all active pointers that 
//        target that tile; the tile will use that gesture to manipulate the tile
//     3) In addition, the container has a gesture object that incorporates all active pointers; 
//        the container will use that gesture to zoom in some cases
//
// In the simple scenario, gesture objects are created statically on all appropriate objects
// at load time. Since this wouldn't scale well to a real puzzle app in which there might be
// hundreds of pieces, in this sample we show how to create gestures dynamically on pointer
// down (and how to clean them up on gesture end). This scales well to any number of pieces,
// but does incur some unnecessary overhead to create and release the objects. In a real
// application, the developer could eliminate this by statically allocating a small number
// of gestures and managing his own pool of reusable objects.
    
// Managing gestures on the container element:
//   1) If all contacts target tiles, the container ignores any gestures and the tiles respond
//      to the gestures that target them.
//   2) If any contact targets the container, the container responds to its gestures and the
//      tiles ignore their gestures.
//   3) We differentiate these two cases by maintaining an expando property on the container's
//      gesture object that is a list of any active contacts that target the container rather
//      than any of the tiles.

// Scenarios supported:
//   1) Tiles can be dragged, rotated and zoomed; scaling is capped on the bottom end so the 
//      user can't accidentally shrink a tile past the point of being visible. Tap restores
//      a tile to it's original size in case it gets too small to zoom.
//   2) The background can be zoomed and dragged but not rotated. Tap restores it to starting
//      position and scale.

// Managing event handling on the tiles:
//   1) With only four tiles, it is simpler and as performant to create gestures and event
//      listeners at load time.  With a hundred tiles, creating gesture objects and event
//      handlers for all of them all of the time is an unwarranted cost.
//   2) Accordingly, this sample shows how to dynamically create gestures and register 
//      event handlers on demand (pointer down) and how to release them when done.

// Mixed input mode considerations:
//   1) We can't mix different pointer types on a gesture, so we track the first pointer
//      type added to each gesture, and filter out any subsequent pointers that don't match
//   2) If touch is active and pen comes down, touch sill be cancelled, so we have to
//      listen for pointercancel and treat it like pointer up for contact tracking purposes
//   3) Tracking the gesture input type requires us to handle gesture end and tap for cleanup

    function getColorFromIndex(index) {               // manages tile colors
        var colors = ["rgb(0, 140, 0)", "rgb(0, 98, 140)", "rgb(194, 74, 0)", "rgb(89, 0, 140)", "rgb(191, 150, 0)", "rgb(140, 0, 0)"];
        return colors[index % colors.length];
    }

    function getNextColorFromColor(color) {           // cycles through the tile colors
        var colors = ["rgb(0, 140, 0)", "rgb(0, 98, 140)", "rgb(194, 74, 0)", "rgb(89, 0, 140)", "rgb(191, 150, 0)", "rgb(140, 0, 0)"];
        return getColorFromIndex(colors.indexOf(color) + 1);
    }

    function initialize() {
        //  Lay out the pieces programmatically (scaleable to larger number of pieces)
        //    1) Make everything fit into a 640x360 space
        //    2) Centers distributed around a circle of radius = size/sqrt(2) (just far enough 
        //       out to keep the inner corners from overlapping) + 1.0 (for borders)
        //    3) Start at angle = pi radians/number of pieces
        //    4) Set up an initial z-order (which will be modified later by user interaction)
        //  (N.b.: a full-fledged puzzle app would do some random distribution of pieces.)
        //
        var width = 640;
        var height = 360;

        var size = Math.min(width,height)*0.5;
        var radius = size/Math.SQRT2+1.0;
        var pieces = document.getElementsByClassName("PuzzlePiece");
        var angleDelta = 2.0*Math.PI/pieces.length;
        var angleStart = angleDelta/2.0;
        
        for (var i=0; i<pieces.length; i++) {
            //  To position the tiles so that they can be easily manipulated by gestures,
            //  set a 2D transform on them, expressed in the coordinate system of their parent.
            //  (MSGesture events provide incremental scale factors, rotations, and translations,
            //  all expressed in the coordinate system of the the target element's parent.)
            //
            var angle = angleStart+i*angleDelta;
            pieces[i].style.transform = (new MSCSSMatrix()).translate(-size/2.0,-size/2.0).
            translate(width / 2.0 + radius * Math.cos(angle), height / 2.0 + radius * Math.sin(angle));
            pieces[i].startingTransform = pieces[i].style.transform;  // expando property for resetting
            pieces[i].style.backgroundColor = getColorFromIndex(i);
            pieces[i].startingColorIndex = i;                           // expando property for resetting
            pieces[i].style.width = size+"px";
            pieces[i].style.height = size+"px";
            pieces[i].style.zIndex = i;
            pieces[i].gestureObject = null; // expando property: tracks whether gesture is underway
        }

        //
        //  Set up the tabletop position and size; since it will also be manipulable,
        //  position it with a 2D transform as well.
        //
        var tableTop = document.getElementsByClassName("TableTop")[0];
        tableTop.style.width = width+"px";
        tableTop.style.height = height+"px";
        tableTop.style.backgroundColor = "rgb(0, 0, 0)";
        tableTop.style.transform = (new MSCSSMatrix());

        //  
        //  At load time we only set up some basic event handling for the tabletop.
        //  For a real puzzle game, setting up event handlers and gestures for every piece
        //  at load time would be wasteful and not performant; instead, we will use the 
        //  tabletop event handlers to dynamically set up gestures and handlers for the pieces.
        //  (We will tear them back down again when gestures are completed.)
        //
        tableTop.addEventListener("pointerdown",onTableTopPointerDown, true);
        tableTop.addEventListener("pointerup", onTableTopPointerUp, true);
        tableTop.addEventListener("pointercancel", onTableTopPointerUp, true);

        tableTop.gestureObject = new MSGesture(); // expando on element: tracks the tabletop gesture
        tableTop.gestureObject.target = tableTop;
        tableTop.gestureObject.pointerType = null;  // expando on gesture: filter against mixed pointer types
        tableTop.targetedContacts = [];           // expando on element: list of contacts that target the tabletop
        tableTop.topmostZ = 3;                      // expando on element: used for quicky and dirty z-reordering

        tableTop.addEventListener("MSGestureChange", onTableTopGestureChange, false);
        tableTop.addEventListener("MSGestureTap", onTableTopGestureTap, false);
        tableTop.addEventListener("MSGestureEnd", onTableTopGestureEnd, false);
        tableTop.addEventListener("MSGestureHold", onTableTopGestureHold, false);
    }

    function onTableTopPointerDown(e) {
        //    Update the tabletop gesture object and gesture handling:
        //    1) The table top gesture object is persistent from load time on, but its pointerType field
        //       is null when there are no contacts
        //    2) pointerType is used to filter additional pointers coming down, since the gesture recognizer
        //       can't handle mixed pointer types, we use the first pointer we see to reject any pointers of
        //       a different type for the duration of the current table top gesture
        //    3) targetedContacts tracks those contacts that actually target the tabletop (as opposed to
        //       a piece).  This is used to decide whether the tabletop responds to the interaction, or 
        //       whether pieces respond to the interaction.
        //
        //    4) **Note**: we need to capture the pointer to the tabletop element so that we're guaranteed to
        //       receive a pointer up; without that, the targeted contacts tracking can be broken. A design
        //       alternative would be to not capture the pointer, but listen for pointerout events to clean
        //       up the contact tracking.

        if (e.currentTarget.gestureObject.pointerType === null) {               // First contact!
            e.currentTarget.setPointerCapture(e.pointerId);
            e.currentTarget.gestureObject.pointerType = e.pointerType;
            e.currentTarget.gestureObject.addPointer(e.pointerId);
            if (e.target === e.currentTarget) {
                e.target.targetedContacts.push(e.pointerId);
            }
        }
        else if (e.currentTarget.gestureObject.pointerType === e.pointerType) { // Subsequent contact of similar type!
            e.currentTarget.setPointerCapture(e.pointerId);
            e.currentTarget.gestureObject.addPointer(e.pointerId);
            if (e.target === e.currentTarget) {
                e.target.targetedContacts.push(e.pointerId);
            }
        }
        else {                                                                  // Subsequent contact of different type!
            return;
        }

        //
        //  Create or update the tile gesture object and handling:
        //    1) Create a gesture, associate it with the pointer target element, contact, and pointer type
        //    2) Add event listeners for gesture events
        //    3) Pop this element to the top of the z-order (quick and dirty algorithm - see comment below)
        //  N.b.: code to defend against mixed pointer type is technically unnecessary since it has already 
        //  filtered above, but that would be relatively fragile, as it depends on the fact that the tabletop
        //  gesture listens to all active contacts.
        //

        if (!e.target.gestureObject) {                                      //  First contact on this element!
            e.target.gestureObject = new MSGesture();
            e.target.gestureObject.target = e.target;
            e.target.gestureObject.pointerType = e.pointerType;
            e.target.addEventListener("MSGestureChange", onPieceGestureChange, false);
            e.target.addEventListener("MSGestureEnd", onPieceGestureEnd, false);
            e.target.addEventListener("MSGestureTap", onPieceGestureTap, false);
            e.target.addEventListener("MSGestureHold", onPieceGestureHold, false);
            e.target.gestureObject.pointerType === e.pointerType;
            e.target.gestureObject.addPointer(e.pointerId);
            e.target.parentElement.topmostZ += 1;
            e.target.style.zIndex = e.target.parentElement.topmostZ;
        }
        else if (e.target.gestureObject.pointerType === e.pointerType) {    // Subsequent contact of same kind!
            e.target.gestureObject.addPointer(e.pointerId);
            //
            //  To pop the element to the top, we just keep track of the topmostZ index, increment it by one,
            //  and assign that value to the target element.  This would eventually wrap around or overflow.
            //  A better algorithm here would test topmostZ against some threshold and, when it is hit, take 
            //  time to remap the zIndex's back to 0 .. n.
            //
            e.target.parentElement.topmostZ += 1;
            e.target.style.zIndex = e.target.parentElement.topmostZ;
        }
    }

    function onTableTopPointerUp(e) {
        //  Called on either pointer up or pointer cancel (which can easily happen to touch if a pen comes in range,
        //  for example.)  Remove the contact from the list of contacts that target the tabletop.

        var tableTop;
        if (e.target === e.currentTarget) {  // pointer up on the tabletop
            tableTop=e.target;
        }
        else {                               // pointer up on a tile, but it may have originally gone down on the tabletop!
            tableTop=e.target.parentElement;
        }
        var i = tableTop.targetedContacts.indexOf(e.pointerId);
        if (i !== -1) {
            tableTop.targetedContacts.splice(i,1);
        }
    }

    function onTableTopGestureEnd(e) {
        //  Clear the gesture input pointer type
        e.gestureObject.pointerType=null;
    }

    function onTableTopGestureChange(e) {
        //  Only handle the gesture if table top is the target and 
        //  at least one active contact is targeting the table top:

        if ((e.target === e.currentTarget) && (e.target.targetedContacts.length !== 0)) {
            // Update the scale factor on the container element.  The gesture event contains
            // incremental scale, rotation, and translation since the last event.  Scaling and rotation
            // take place around the gesture "pivot point" (also provided in the gesture event as the
            // offsetX and Y properties). The element must be translated to that point before applying
            // the zoom and rotation, then translated back.  All of these transformations can be
            // concatenated to the current transform, so:
            //   1) Get the current 2D transform and translate it to the pivot point of the gesture
            //   2) Apply incremental translation and scale (by design, no rotation on the table top)
            //   3) Translate back from the pivot point of the gesture
            //
            var currentXform = new MSCSSMatrix(e.target.style.transform);
            e.target.style.transform = currentXform.
                translate(e.offsetX, e.offsetY).
                translate(e.translationX, e.translationY).
                rotate(e.rotation * 180 / Math.PI).
                scale(e.scale).
                translate(-e.offsetX, -e.offsetY);
        }
    }

    function onTableTopGestureTap(e) {
        //  Restore original size - if table top is the target and at least one active contact is targeting the table top:
        if ((e.target === e.currentTarget) && (e.target.targetedContacts.length !== 0)) {
            e.target.style.transform = new MSCSSMatrix();
        }

        //  Tap is similar to End from a cleanup perspective, so clear the gesture input pointer type
        e.gestureObject.pointerType=null;
    }

    function onTableTopGestureHold(e) {
        if (e.detail === e.MSGESTURE_FLAG_END) {
            //  Restore tabletop and all the pieces:
            if ((e.target === e.currentTarget) && (e.target.targetedContacts.length !== 0)) {
                initialize();
            }
            
            //  Press and hold is similar to End from a cleanup perspective, so clear the gesture input pointer type
            e.gestureObject.pointerType = null;
        }
    }

    function onPieceGestureChange(e) {
        //    Bail out if the table top is being manipulated
        if (e.target.parentElement.targetedContacts.length !== 0) {
            return;
        }

        //  Update the transform on this element; similar to tabletop transform above, except we
        //  do support rotation on the pieces.
        var currentXform = new MSCSSMatrix(e.target.style.transform);

        //  Keep scale from getting too small (else user can lose track of it!)
        var currentScale = Math.sqrt(currentXform.m11 * currentXform.m22 - currentXform.m12 * currentXform.m21);
        if (e.scale * currentScale >= 0.1) {
            e.target.style.transform = currentXform.translate(e.offsetX, e.offsetY).
            translate(e.translationX, e.translationY).
            rotate(e.rotation * 180 / Math.PI).
            scale(e.scale).
            translate(-e.offsetX, -e.offsetY);
        }
        else {
            e.target.style.transform = currentXform.translate(e.offsetX, e.offsetY).
            translate(e.translationX, e.translationY).
            rotate(e.rotation * 180 / Math.PI).
            translate(-e.offsetX, -e.offsetY);
        }
    }

    function cleanUpTileGesture(elt) {
        elt.gestureObject.target = null;       // Clear target so the gesture object can be garbage collected
        elt.gestureObject = null;
        elt.removeEventListener("MSGestureChange", onPieceGestureChange);
        elt.removeEventListener("MSGestureEnd", onPieceGestureEnd);
        elt.removeEventListener("MSGestureTap", onPieceGestureTap);
    }

    function onPieceGestureEnd(e) {
        //  Clean up gesture handling; not necessary with only four pieces, but
        //  important if this were scaled to a real puzzle app. Just release the
        //  reference and the gesture object itself will eventually be garbage collected.
        cleanUpTileGesture(e.target);
    }

    function onPieceGestureTap(e) {
        //  Cycle the tile color
        e.target.style.backgroundColor = getNextColorFromColor(e.target.style.backgroundColor);

        //  As for the tabletop, Tap is like End from a cleanup perspective:
        cleanUpTileGesture(e.target);
    }

    function onPieceGestureHold(e) {
        if (e.detail === e.MSGESTURE_FLAG_END) {
            //  Restore original transform and color
            e.target.style.transform = e.target.startingTransform;
            e.target.style.backgroundColor = getColorFromIndex(e.target.startingColorIndex);

            //  As for the tabletop, Tap is like End from a cleanup perspective:
            cleanUpTileGesture(e.target);
        }
    }
})();
