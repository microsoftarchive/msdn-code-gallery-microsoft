//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Set up the animations to be used for responding to the keyboard
(function () {
    // Alias for animation namespace
    var AnimationMetrics = Windows.UI.Core.AnimationMetrics;

    // This class is used later on...
    var OffsetArray = WinJS.Class.define(function (offset, defOffset) {
        // Constructor
        if (Array.isArray(offset) && offset.length > 0) {
            this.offsetArray = offset;
        } else if (offset && offset.hasOwnProperty("top") && offset.hasOwnProperty("left")) {
            this.offsetArray = [offset];
        } else if (defOffset) {
            this.offsetArray = defOffset;
        } else {
            this.offsetArray = [{ top: "0px", left: "11px", rtlflip: true }]; // Default to 11 pixel from the left (or right if RTL)
        }
    }, { // Public Members
        getOffset: function (i) {
            if (i >= this.offsetArray.length) {
                i = this.offsetArray.length - 1;
            };
            return this.offsetArray[i];
        }
    });

    function uniformizeNumber(s, flipSign, elem) {
        s = s.toString();
        for (var lastLetter = s.length; s.charAt(lastLetter - 1) > "9"; lastLetter--) {
        }
        var val = parseFloat(s.substring(0, lastLetter));
        if (val && flipSign && window.getComputedStyle(elem).direction === "rtl") {
            val = -val;
        }
        return val.toString() + s.substring(lastLetter);
    }

    // ...as is this function
    function translateCallback(offsetArray, prefix) {
        prefix = prefix || "";
        return function (i, elem) {
            var offset = offsetArray.getOffset(i);
            return prefix + "translate(" + uniformizeNumber(offset.left, offset.rtlflip, elem) + ", " + uniformizeNumber(offset.top) + ")";
        };
    }

    function roundControl(x) {
        return Math.round(x * 10) / 10;
    }

    // Get the metrics for the "ShowPanel" and "HidePanel" animation curves, which are used by the Input Pane
    var showAnimation = AnimationMetrics.AnimationDescription(AnimationMetrics.AnimationEffect.showPanel, AnimationMetrics.AnimationEffectTarget.primary).animations[0];
    var hideAnimation = AnimationMetrics.AnimationDescription(AnimationMetrics.AnimationEffect.hidePanel, AnimationMetrics.AnimationEffectTarget.primary).animations[0];

    // Create a new namespace to hold the animation functions
    WinJS.Namespace.define("KeyboardEventsSample.Animations", {

        // The semantics for these animation functions are just like the WinJS.UI.Animation functions
        // Use this function to move an element up above the soft keyboard when the soft keyboard appears
        inputPaneShowing: function (element, offset) {
            var offsetArray = new OffsetArray(offset);
            return WinJS.UI.executeAnimation(element,
            {
                property: "transform",
                delay: 0,
                duration: showAnimation.duration,
                timing: "cubic-bezier(" + roundControl(showAnimation.control1.x) + "," + roundControl(showAnimation.control1.y) + "," + roundControl(showAnimation.control2.x) + "," + roundControl(showAnimation.control2.y) + ")",
                from: "translate(0px, 0px)",
                to: translateCallback(offsetArray)
            });
        },

        // Use this function to move an element back down when the soft keyboard disappears
        inputPaneHiding: function (element, offset) {
            var offsetArray = new OffsetArray(offset);
            return WinJS.UI.executeAnimation(element,
            {
                property: "transform",
                delay: 0,
                duration: hideAnimation.duration,
                timing: "cubic-bezier(" + roundControl(hideAnimation.control1.x) + "," + roundControl(hideAnimation.control1.y) + "," + roundControl(hideAnimation.control2.x) + "," + roundControl(hideAnimation.control2.y) + ")",
                from: translateCallback(offsetArray),
                to: "translate(0px, 0px)"
            });
        }
    });
})();


(function () {
    "use strict";

    // This function is called whenever a user navigates to this page. It
    // populates the page elements with the app's data.
    
    function showingHandler(e) {
        if (document.activeElement.id === "customHandling") {
            keyboardShowing(e.occludedRect);

            // Be careful with this property. Once it has been set, the framework will
            // do nothing to help you keep the focused element in view.
            e.ensuredFocusedElementInView = true;
        }
    }

    // This function is called as the keyboard appears.
    // The basic design is:
    //  1. Animate the text element up above the keyboard. For this app, more than just the text area is animated.
    //     How much is animated is up to you!
    //  2. Relayout the app
    //  3. Scroll the lists so that what the user was looking at remains in view

    var displacement = 0;
    var showingAnimation = null;
    var hidingAnimation = null;
    function keyboardShowing(keyboardRect) {
        // Make sure to cancel the promise. The animation won't cancel, but it will prevent
        // the layout from changing.
        if (hidingAnimation) {
            hidingAnimation = hidingAnimation.cancel();
            hidingAnimation = null;
        }

        var elementToAnimate = document.getElementById("middleContainer");
        var elementToResize = document.getElementById("appView");
        var elementToScroll = document.getElementById("middleList");

        // Cache the amount things are moved by. It makes the math easier
        displacement = keyboardRect.height;
        var displacementString = -displacement + "px";

        // Figure out what the last visible things in the list are
        var bottomOfList = elementToScroll.scrollTop + elementToScroll.clientHeight;

        // Animate
        // Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
        // will move on its own. You should make sure the input element doesn't get occluded by the bar
        showingAnimation = KeyboardEventsSample.Animations.inputPaneShowing(elementToAnimate, { top: displacementString, left: "0px" }).then(function () {

            // After animation, layout in a smaller viewport above the keyboard
            elementToResize.style.height = keyboardRect.y + "px";

            // Scroll the list into the right spot so that the list does not appear to scroll
            elementToScroll.scrollTop = bottomOfList - elementToScroll.clientHeight;
            showingAnimation = null;
        });
    }

    function hidingHandler() {
        // The Input Pane Hiding event will be fired whenever the keyboard disappears.
        // It's good to make sure that the layout has changed before attempting to replace the old layout.
        // Also, make sure to cancel the completion of showing animation
        if (showingAnimation) {
            showingAnimation.cancel();
            showingAnimation = null;
        }

        if (displacement > 0) {
            var elementToAnimate = document.getElementById("middleContainer");
            var elementToResize = document.getElementById("appView");
            var elementToScroll = document.getElementById("middleList");

            var displacementString = -displacement + "px";
            displacement = 0;

            // Again, scroll the list
            var bottomOfList = elementToScroll.scrollTop + elementToScroll.clientHeight;

            // Layout. It's important to do this before animating, or else there could be a big, blank space
            // where the keyboard is as it's leaving
            elementToResize.style.height = "";

            // Shift the element to be animated up. This isn't really necessary, but since scrolling is an expensive operation, it
            // makes sure there's no flashing as content lays out.
            // Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
            // will move on its own. You should make sure the input element doesn't get occluded by the bar
            elementToAnimate.style.transform = "translate(0px," + displacementString + ")";
            elementToScroll.scrollTop = bottomOfList - elementToScroll.clientHeight;

            hidingAnimation = KeyboardEventsSample.Animations.inputPaneHiding(elementToAnimate, { top: displacementString, left: "0px" }).then(function () {
                elementToAnimate.style.transform = "";
            });
        }
    }

    WinJS.UI.Pages.define("/html/keyboardPage.html", {
        ready: function ready(element, options) {
            document.getElementById("closeAppView").addEventListener("click", function () {
                WinJS.Navigation.back();
            });

            // Attach the input pane event handlers
            var inputPane = Windows.UI.ViewManagement.InputPane.getForCurrentView();
            inputPane.addEventListener("showing", showingHandler, false);
            inputPane.addEventListener("hiding", hidingHandler, false);

            WinJS.Navigation.onbeforenavigate = function (navArgs) {
                if (navArgs.detail !== "/html/keyboardPage.html") {
                    inputPane.removeEventListener("showing", showingHandler);
                    inputPane.removeEventListener("hiding", hidingHandler);
                }
            };

            var fillerDivs = document.querySelectorAll(".filler");
            for (var i = 0, len = fillerDivs.length; i < len; i++) {
                var elem = fillerDivs[i];
                elem.style.height = (elem.parentElement.offsetHeight * 2) + "px";
            }
        }
    });
})();
