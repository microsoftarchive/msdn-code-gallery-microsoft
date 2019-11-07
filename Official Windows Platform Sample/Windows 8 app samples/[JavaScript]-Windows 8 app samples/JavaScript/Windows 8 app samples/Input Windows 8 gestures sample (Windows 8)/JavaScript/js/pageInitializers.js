//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//// PARTICULAR PURPOSE. 
//// 
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("PageInitializers", {
        // When the PageInitializers Namespace is created _initializer hooks up all appbar buttons to their 
        // respective functionality and sets initially visible buttons, hooks up all events that need to be
        // registered once all DOM objects have been loaded
            _initializer: (function () {
                WinJS.UI.Pages.define("default.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the default.html page has been loaded.
                        /// It sets up the correct appbar buttons
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The html element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>
                        var appBarHost = document.getElementById("appbar");
                        var appBar = appBarHost.winControl;
                        appBar.showOnlyCommands(appBarHost.querySelectorAll(".appbar-appedgy"));

                        // Get the corresponding appbar buttons for this page
                        var btnLinks = document.getElementById("btn-misclinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-misclinks").winControl.show(btnLinks);
                                                            },
                                                            false);
                    }
                });

                WinJS.UI.Pages.define("/html/appEdgy.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the appEdge.html page has loaded
                        /// It sets up the correct appbar buttons
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>
                        var btnLinks = document.getElementById("btn-appedgylinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-appedgylinks").winControl.show(btnLinks);
                                                            },
                                                            false);
                    },
                    unload: function () {
                    }
                });

                WinJS.UI.Pages.define("/html/systemEdgy.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the systemEdgy.html page has been loaded.
                        /// It sets up the correct appbar buttons
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>
                        var btnLinks = document.getElementById("btn-systemedgylinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-systemedgylinks").winControl.show(btnLinks);
                                                            },
                                                            false);
                    },
                    unload: function () {
                    }
                });

                WinJS.UI.Pages.define("/html/zoom.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the zoom.html page has been loaded.
                        /// It sets up the correct appbar buttons
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>
                        var btnLinks = document.getElementById("btn-semanticzoomlinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-semanticzoomlinks").winControl.show(btnLinks);
                                                            },
                                                            false);
                    },
                    unload: function () {
                    }
                });

                WinJS.UI.Pages.define("/html/pressandhold.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the pressandhold.html page has been loaded.
                        /// It sets up the correct appbar buttons
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>
                        var tooltipButton = document.getElementById("tooltip-button-control").winControl;
                        tooltipButton.addEventListener("click",
                                                                function () {
                                                                    // generate a random hex color for the button text
                                                                    this.style.color = "#" +
                                                                        (function (num) {
                                                                            // left pad the number with zeros to be length 6
                                                                            return new Array(7 - num.length).join("0") + num;
                                                                        } (Math.floor((Math.random() * 0xFFFFFF) + 1).toString(16)));
                                                                },
                                                                false);

                        var btnLinks = document.getElementById("btn-pressandholdlinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-pressandholdlinks").winControl.show(btnLinks);
                                                            },
                                                            false);
                    },
                    unload: function () {
                    }
                });

                WinJS.UI.Pages.define("/html/tap.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the tap.html page has been loaded.
                        /// It sets up the correct appbar buttons for the page, and 
                        /// hooks up tap handlers to the tappable squares in the sample.
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>

                        // hook up tap handlers to tappable buttons in grid
                        var buttons = document.getElementsByClassName("tap-button");
                        for (var i = 0; i < buttons.length; ++i) {
                            buttons[i].addEventListener("click", TapControls.handleClick, false);
                            buttons[i].addEventListener("MSPointerDown", function () {
                                WinJS.UI.Animation.pointerDown(this);
                            }, false);
                            buttons[i].addEventListener("MSPointerUp", function () {
                                WinJS.UI.Animation.pointerUp(this);
                            }, false);
                        }

                        // hook up hanlders for size control buttons
                        TapControls.RegisterButtonSizeEventHandlers(document.getElementById("tap-cram"), 5);
                        TapControls.RegisterButtonSizeEventHandlers(document.getElementById("tap-min"), 7);
                        TapControls.RegisterButtonSizeEventHandlers(document.getElementById("tap-accurate"), 9);
                        
                        // set initial size
                        TapControls.ChangeButtonSize(5)();

                        // hook up appbar buttons
                        var btnLinks = document.getElementById("btn-taplinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-taplinks").winControl.show(btnLinks);
                                                            },
                                                            false);
                    }
                });

                WinJS.UI.Pages.define("/html/swipe.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the swipe.html page has been loaded.
                        /// It sets up the correct appbar buttons for the swipe page.
                        /// Also configures appbar to be shown correctly for selections
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>

                        // Register the links for this page
                        var btnLinks = document.getElementById("btn-swipelinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-swipelinks").winControl.show(btnLinks);
                                                            },
                                                            false);

                        // Add the event handler to the clear selection appbar button
                        var btnClearSlection = document.getElementById("btn-swipe-clearSelection").winControl;
                        btnClearSlection.addEventListener("click",
                                                                    function () {
                                                                        document.getElementById("galleryList").winControl.selection.clear();
                                                                    },
                                                                    false);

                        // Make the appbar show when a selection is made
                        var swipeListView = document.getElementById("galleryList").winControl;

                        // Modify appbar based on selection
                        swipeListView.onselectionchanged = function () {
                            if (swipeListView.selection.count() > 0) {
                                // if there is at least one selection made, show the appbar and make sticky
                                var appbar = document.getElementById("appbar").winControl;
                                appbar.sticky = true;
                                appbar.show();
                            }
                            else {
                                // if there is no selection, then un-sticky the appbar
                                document.getElementById("appbar").winControl.sticky = false;
                            }
                        };

                        // Register a view change handler to reconfigure the appbar sticky when the view is changed
                        document.getElementById("view-zoomedin").winControl.registerViewChangeHandler(function (view, pageId) {
                            if (pageId === "swipe") {
                                // If we are scrolling to the swipe page, then make sure the appbar has the correct sticky setting
                                // Store the previous value before changing
                                document.getElementById("galleryList").winControl.onselectionchanged();
                            } else {
                                // If we are scrolling to another page, make sure to return the appbar to it's required setting
                                document.getElementById("appbar").winControl.sticky = false;
                            }
                        });

                    }
                });

                WinJS.UI.Pages.define("/html/objectZoom.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the objectZoom.html page has been loaded.
                        /// It sets up the correct appbar buttons for the page.
                        /// Also sets up the object for manipulation.
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>

                        // Get the image and parent
                        var elm = document.getElementById("objectzoom-zoomable");
                        var elmParent = document.getElementById("objectzoom-parent");
                        var boundingParent = document.getElementById("playarea");
                        var moveable = new ItemMovers.Manipulable();

                        // When the image is fully loaded, set the manipulation filter
                        elm.onload = function () {
                            // Configure for manipulation (moveable, not rotatable, zoomable, use inertia)
                            // and set the initial tranformations to put the image in the center of the viewport
                            var initialScale = 0.5;
                            var initialTranslation = {
                                x: (boundingParent.offsetWidth - (elm.offsetWidth * initialScale)) / 2,
                                y: (boundingParent.offsetHeight - (elm.offsetHeight * initialScale)) / 2
                            };
                            moveable.configure(true, false, true, true,
                                               initialScale, 0, initialTranslation);
                            moveable.setElement(elm);
                            moveable.setParent(elmParent);

                            // Create a manipulation filter using the helper function
                            ManipulationFilters.CreateAndApplyManipulationFilter(boundingParent, elm, moveable, initialScale, initialTranslation);

                            // Call reset to apply the initial scaling and translation
                            moveable.resetAllTransforms();
                        };

                        // Register appbar buttons
                        var btnLinks = document.getElementById("btn-objectzoomlinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-objectzoomlinks").winControl.show(btnLinks);
                                                            },
                                                            false);

                        var btnReset = document.getElementById("btn-objectzoom-reset").winControl;
                        btnReset.addEventListener("click",
                                                            function () {
                                                                moveable.resetAllTransforms();
                                                            },
                                                            false);
                    },
                    unload: function () {
                    }
                });

                WinJS.UI.Pages.define("/html/rotate.html", {
                    ready: function (element, options) {
                        /// <summary>
                        /// Function that is called when the rotate.html page has been loaded.
                        /// It sets up the correct appbar buttons for the page.
                        /// Also sets up the two objects for manipulation.
                        /// </summary>
                        /// <param name="element" type="Object">
                        /// The element that has been loaded.
                        /// Should be the page in this case.
                        /// </param>
                        /// <param name="options" type="Object">
                        /// Any options associated with the page
                        /// </param>                        

                        // Setup the rotateable only object
                        var moveable1 = new ItemMovers.Manipulable();
                        moveable1.configure(false, true, false, true);
                        moveable1.setElement(document.getElementById("rotateableonly"));
                        moveable1.setParent(document.getElementById("rotateableonly-parent"));
                        moveable1.registerMoveHandler({ x: 100, y: 100 }, ManipulationFilters.FixPivot.MoveHandler);


                        // Setup the rotateable and move object
                        var elm = document.getElementById("rotatetranslateable");
                        var elmParent = document.getElementById("rotatetranslateable-container");
                        var moveable2 = new ItemMovers.Manipulable();

                        // When the image is fully loaded, set the manipulation filter
                        elm.onload = function () {
                            var initialScale = 0.8;
                            var initialTranslate = { x: 120, y: 20 };
                            // Configure the object for rotation and translation, but no scaling                        
                            moveable2.configure(false, true, true, true,
                                                initialScale, 0, initialTranslate);

                            moveable2.setElement(elm);
                            moveable2.setParent(elmParent);

                            // Apply a move filter so that the object cannot be moved outside its parent
                            ManipulationFilters.CreateAndApplyManipulationFilter(elmParent, elm, moveable2, initialScale, initialTranslate);
                            // Call reset to apply the initial scaling and translation
                            moveable2.resetAllTransforms();
                        };

                        // Register the appbar buttons
                        // Hook up the appbar button for links
                        var btnLinks = document.getElementById("btn-rotatelinks").winControl;
                        btnLinks.addEventListener("click",
                                                            function () {
                                                                document.getElementById("flyout-rotatelinks").winControl.show(btnLinks);
                                                            },
                                                            false);

                        // Add event listener to reset button
                        var btnReset = document.getElementById("btn-rotatelinks-reset").winControl;
                        btnReset.addEventListener("click",
                                                            function () {
                                                                moveable1.resetAllTransforms();
                                                                moveable2.resetAllTransforms();
                                                            },
                                                            false);
                    }
                });
            })()
        });
})();