//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//// PARTICULAR PURPOSE. 
//// 
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    
    /// <summary>
    /// Implements the IZoomableView interface to provide a custom control
    /// That can be used with the symantic zoom
    /// </summary>
    var ZoomableView = WinJS.Class.define(function (pageControl) {
        /// <summary>
        /// Constructs a new ZoomableView object
        /// </summary>
        /// <param name="pageControl" type="Object">
        /// The PageControl.ZoomablePages object that will be used by this implementation.
        /// </param>

        this._pageControl = pageControl;
    }, {
        // Public methods to implement IZoomableView
        getPanAxis: function () {
            return this._pageControl._getPanAxis();
        },

        configureForZoom: function (isZoomedOut, isCurrentView, triggerZoom, prefetchedPages) {
            this._pageControl._configureForZoom(isZoomedOut, isCurrentView, triggerZoom, prefetchedPages);
        },

        setCurrentItem: function (x, y) {
            this._pageControl._setCurrentItem(x, y);
        },

        getCurrentItem: function () {
            return this._pageControl._getCurrentItem();
        },

        beginZoom: function () {
            this._pageControl._beginZoom();
        },

        positionItem: function (item, position) {
            return this._pageControl._positionItem(item, position);
        },

        endZoom: function (isCurrentView) {
            this._pageControl._endZoom(isCurrentView);
        },

        handlePointer: function (pointerId) {
            this._pageControl._handlePointer(pointerId);
        }
    });

    WinJS.Namespace.define("PageControl", {
        /// <summary>
        /// Object to handle functionality of the custom zoomable view implementation
        /// used for the semantic zoom.
        /// </summary>
        ZoomablePages: WinJS.Class.define(function (element, options) {
            /// <summary>
            /// Constructs a new ZoomablePages object.
            /// </summary>
            /// <param name="element" type="Object">
            /// The html element associated with this object.
            /// </param>
            /// <param name="options" type="Object">
            /// The options passed when constructing the object.
            /// </param>
            this._element = element;
            this._viewport = document.createElement("div");
            this._viewport.className = "pagezoom-view";
            this._element.appendChild(this._viewport);

            this._canvas = document.createElement("div");
            this._canvas.className = "pagezoom-canvas";
            this._viewport.appendChild(this._canvas);

            this._pages = 0;
            this._preview = false;

            this._isBouncingIn = false;
            this._oldScrollOffset = 0;
            this.snapZoomEvent = false;
            
            this._indexToId = [];
            this._viewChangeHandlers = [];

            if (options) {
                if (typeof options.preview === "boolean") {
                    this._preview = options.preview;
                }
            }
                        
            if (!this._preview) {
                // If this is not the zoomed out view, then configure specific settings
                var that = this;
                this._viewport.style.msScrollSnapX = "mandatory snapInterval(0px, 100%)";
                this._viewport.addEventListener("scroll", 
                    function () {
                        /// <summary>
                        /// Function that is called whenever the zoomed in view is scrolled.
                        /// Takes care of showing the correct appbar buttons and 
                        /// makes sure that the appbar is in the correct setting given the swipe selects.
                        /// Also calls any view change handler functions that may have been registered.
                        /// </summary>

                        // Call any viewChangeHandlers that were registered
                        PageControl.ZoomablePages.HandleViewChange(that);

                        // Set the correct appbar commands
                        var semanticZoom = document.getElementById("semanticZoom").winControl;
                        var appBarHost = document.getElementById("appbar");
                        var appBar = appBarHost.winControl;
                        if (semanticZoom.zoomedOut) {
                            appBar.showOnlyCommands(appBarHost.querySelectorAll(".appbar-misc"));
                        }
                    },
                    false);
            }
            
        }, {
            // Public members
            zoomableView: {
                /// <summary>
                /// Handle to the ZoomableView object associated with this ZoomablePages object
                /// </summary>
                get: function () {
                    if (!this._zoomableView) {
                        this._zoomableView = new ZoomableView(this);
                    }

                    return this._zoomableView;
                }
            },

            addPage: function (uri, pageName) {
                /// <summary>
                /// Adds a page to this zoomable view
                /// </summary>
                /// <param name="uri" type="String">
                /// The location of the page to be added
                /// </param>
                /// <param name="pageName" type="String">
                /// The name of the page to be added
                /// </param>

                this._pages++;

                var container = document.createElement("div");
                
                if (this._preview) {
                    // If this zoomable view is a preview then configure the div correctly
                    container.className = "pagezoom-previewcontainer";
                    container.style.width = "280px";
                    this._canvas.style.width = 280 * this._pages + this._viewport.offsetWidth - 280 + "px";
                    var that = this;
                    container.addEventListener("click", function () {
                        if (that._isZoomedOut) {
                            that._triggerZoom();
                        }
                    });

                    // Add pointerdown and pointerup events
                    // need to use second nested child so that the animations don't change position info that
                    // is used by the semantic zoom positioning
                    container.addEventListener("MSPointerDown", function (evt) {
                        WinJS.UI.Animation.pointerDown(container.children[0].children[0]);
                    }, false);                    
                    container.addEventListener("MSPointerUp", function (evt) {
                        WinJS.UI.Animation.pointerUp(container.children[0].children[0]);
                    }, false);
                    container.addEventListener("MSPointerOut", function (evt) {
                        WinJS.UI.Animation.pointerUp(container.children[0].children[0]);
                    }, false);
                }
                else {
                    // If this is not a preview view, then configure appropriate width
                    container.className = "pagezoom-pagecontainer";

                    var childWidth = (100 / this._pages) + "%";
                    for (var i = 0; i < this._canvas.children.length; ++i) {
                        this._canvas.children[i].style.width = childWidth;
                    }
                    container.style.width = childWidth;
                    this._canvas.style.width = (this._pages * 100) + "%";
                }

                this._canvas.appendChild(container);
                var page = new WinJS.UI.HtmlControl(container, { uri: uri });

                // Store the name of the page to be used later
                this._indexToId.push(pageName);
            },

            registerViewChangeHandler: function (handler) {
                /// <summary>
                /// Register a view change handler function.
                /// These functions are called anytime the view is scrolled.
                /// </summary>
                this._viewChangeHandlers.push(handler);
            },
                       
            // implement ZoomableView
            _getPanAxis: function () {
                return "horizontal";
            },

            _configureForZoom: function (isZoomedOut, isCurrentView, triggerZoom, prefectchedPages) {
                this._isZoomedOut = isZoomedOut;
                this._triggerZoom = triggerZoom;
            },

            _setCurrentItem: function (x, y) {
                // Reset the bouncing toggle since we are zooming in or out
                this._isBouncingIn = false;

                // Convert the position into canvas coordinates
                x += this._viewport.scrollLeft;

                // Now choose the closest element
                var leftPos = this._canvas.firstElementChild.offsetLeft;

                var i = Math.floor((x - leftPos) / this._canvas.children[0].offsetWidth);
                i = Math.max(0, Math.min(this._pages - 1, i));

                // Set focus on the element
                var element = this._canvas.children[i];
                element.focus();
            },

            _beginZoom: function () {
                // If we are snapped then need to unsnap before zooming
                if (!this.snapZoomEvent && this._preview && Windows.UI.ViewManagement.ApplicationView.value === Windows.UI.ViewManagement.ApplicationViewState.snapped) {
                    if (!Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {
                        document.getElementById("semanticZoom").winControl.locked = true;
                    }
                }

                if (this._preview) {
                    this.snapZoomEvent = false;
                }

                // Hide the scrollbar and extend the content beyond the viewport
                var scrollOffset = -this._viewport.scrollLeft;

                // Set the bouncing toggle so that when the unbounce
                // _beginZoom is called, we don't reset the offset
                if (!this._isBouncingIn) {
                    this._oldScrollOffset = scrollOffset;
                    this._isBouncingIn = true;
                }
                else {
                    scrollOffset = this._oldScrollOffset;
                    this._isBouncingIn = false;
                }

                this._viewport.style.overflowX = "visible";
                this._canvas.style.left = scrollOffset + "px";

                this._viewport.style.overflowY = "visible";
            },

            _getCurrentItem: function () {
                // Get the element with focus

                var focusedElement = document.activeElement;

                // Calculate the current item index
                var item = Math.floor(focusedElement.offsetLeft / this._canvas.children[0].offsetWidth);
                item = Math.max(0, Math.min(this._pages - 1, item));

                // Get the position of the element with focus
                var pos = {
                    left: focusedElement.offsetLeft + parseInt(this._canvas.style.left, 10),
                    top: focusedElement.offsetTop,
                    width: focusedElement.offsetWidth,
                    height: focusedElement.offsetHeight
                };

                return WinJS.Promise.wrap({ item: item, position: pos });
            },

            _positionItem: function (/*@override*/item, position) {
                // Reset the bouncing toggle since we are zooming in or out
                this._isBouncingIn = false;

                // Get the corresponding item for the element
                var element = this._canvas.children[Math.max(0, Math.min(item, this._pages - 1))];

                // Ensure the element ends up within the viewport
                var viewportWidth = this._viewport.offsetWidth,
                    offset = Math.max(0, Math.min(viewportWidth - element.offsetWidth, position.left));

                var scrollPosition = element.offsetLeft - offset;

                // Ensure the scroll position is valid
                var adjustedScrollPosition = Math.max(0, Math.min(this._canvas.offsetWidth - viewportWidth, scrollPosition));

                // Since a zoom is in progress, adjust the div position
                this._canvas.style.left = -adjustedScrollPosition + "px";

                element.focus();

                // Return the adjustment that will be needed to align the item
                return WinJS.Promise.wrap({ x: adjustedScrollPosition - scrollPosition, y: 0 });
            },

            _endZoom: function (isCurrentView, setFocus) {
                // Crop the content again and re-enable the scrollbar
                var scrollOffset = parseInt(this._canvas.style.left, 10);

                this._viewport.style.overflowX = "auto";
                this._canvas.style.left = "0px";

                this._viewport.style.overflowY = "hidden";

                this._viewport.scrollLeft = -scrollOffset;

                // call view change handlers with this view and the active element
                if (isCurrentView) {
                    if (this._preview) {
                        for (var i = 0; i < this._viewChangeHandlers.length; ++i) {
                            this._viewChangeHandlers[i]("zoomedout", "");
                        }
                    }
                    else {
                        PageControl.ZoomablePages.HandleViewChange(this);
                    }
                }
            },

            _handlePointer: function (pointerId) {
                // Let the viewport handle panning gestures               
                this._viewport.msSetPointerCapture(pointerId);
            }
        }, {
            HandleViewChange: function (that) {
                /// <summary>
                /// Call the view change handlers
                /// </summary>
                /// <param name="that" type="Object">
                /// The ZoomablePages object to call the view change handlers
                /// </param>

                // calculate the current page index
                var index = Math.min(that._pages - 1, Math.max(Math.floor((that._viewport.scrollLeft * 1.05) / that._viewport.offsetWidth), 0));

                for (var i = 0; i < that._viewChangeHandlers.length; ++i) {
                    that._viewChangeHandlers[i]("zoomedin", that._indexToId[index]);
                }
            }
        })
    });
})();