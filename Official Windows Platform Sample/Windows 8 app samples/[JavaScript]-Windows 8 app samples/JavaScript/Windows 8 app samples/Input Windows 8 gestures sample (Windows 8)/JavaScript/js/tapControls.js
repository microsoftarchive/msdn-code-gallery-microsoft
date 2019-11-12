//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//// PARTICULAR PURPOSE. 
//// 
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    /// These functions are used by the tap.html page to demonstrate features of tapping
    /// and appropriate sizes of controls for tapping.
    WinJS.Namespace.define("TapControls", {
        handleClick: function () {
            /// <summary>
            /// Handles a click event on a tap control.
            /// Sets the tap box to the correct color.
            /// </summary>

            // Unclicked is blue, clicked is orange
            var clicked = "rgb(255, 153, 0)";
            var unclicked = "#09f";
            if (this.style.backgroundColor === clicked) {
                this.style.backgroundColor = unclicked;
            }
            else {
                this.style.backgroundColor = clicked;
            }            
        },

        ChangeButtonSize: function (size) {
            /// <summary>
            /// Returns a function to change the size of the tap buttons.
            /// Also marks the correct selection button.
            /// </summary>
            /// <param name="size" type="Number">
            /// The size to which the tap buttons will be changed.
            /// </param>
            /// <returns type="Function">
            /// Function that can be called to set the size of the tap buttons.
            /// </returns>

            return (function () {
                // Set the tap buton size
                var buttons = document.getElementsByClassName("tap-button");
                for (var i = 0; i < buttons.length; ++i) {
                    buttons[i].style.width = size + "mm";
                    buttons[i].style.height = size + "mm";
                }

                // Mark the correct selection button
                var unselected = "#09f";
                var selected = "#06c";
                document.getElementById("tap-cram").style.backgroundColor = unselected;
                document.getElementById("tap-min").style.backgroundColor = unselected;
                document.getElementById("tap-accurate").style.backgroundColor = unselected;

                switch (size) {
                    case 5:
                        document.getElementById("tap-cram").style.backgroundColor = selected;
                        break;
                    case 7:
                        document.getElementById("tap-min").style.backgroundColor = selected;
                        break;
                    case 9:
                        document.getElementById("tap-accurate").style.backgroundColor = selected;
                        break;
                    default:
                        break;
                }
            });
        },

        RegisterButtonSizeEventHandlers: function (elm, size) {
            /// <summary>
            /// Registers the event handlers for the change tap control size buttons.
            /// </summary>
            /// <param name="elm" type="Object">
            /// The html element to which the events will be registered
            /// </param>
            /// <param name="size" type="Number">
            /// The size to which the tap buttons will be changed.
            /// </param>

            elm.addEventListener("click", TapControls.ChangeButtonSize(size), false);

            elm.addEventListener("MSPointerDown", function () {
                WinJS.UI.Animation.pointerDown(this);
            }, false);
            elm.addEventListener("MSPointerUp", function () {
                WinJS.UI.Animation.pointerUp(this);
            }, false);
        }
    });
})();