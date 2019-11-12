//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/transition-controls.html", {
        ready: function (element, options) {
        }
    });

    // Custom timeline control
    var ZoomableView = WinJS.Class.define(function (timeline) {
        // Constructor
        this._timeline = timeline;
    }, {
        // Public methods
        getPanAxis: function () {
            return this._timeline._getPanAxis();
        },

        configureForZoom: function (isZoomedOut, isCurrentView, triggerZoom, prefetchedPages) {
            this._timeline._configureForZoom(isZoomedOut, isCurrentView, triggerZoom, prefetchedPages);
        },

        setCurrentItem: function (x, y) {
            this._timeline._setCurrentItem(x, y);
        },

        getCurrentItem: function () {
            return this._timeline._getCurrentItem();
        },

        beginZoom: function () {
            this._timeline._beginZoom();
        },

        positionItem: function (/*@override*/item, position) {
            return this._timeline._positionItem(item, position);
        },

        endZoom: function (isCurrentView) {
            this._timeline._endZoom(isCurrentView);
        },

        handlePointer: function (pointerId) {
            this._timeline._handlePointer(pointerId);
        }
    });

    WinJS.Namespace.define("CustomControls", {
        Timeline: WinJS.Class.define(function (element, options) {
            this._element = element;

            this._start = 1990;
            this._end = 2030;
            this._interval = 1;

            if (options) {
                if (typeof options.start === "number") {
                    this._start = options.start;
                }
                if (typeof options.end === "number") {
                    this._end = options.end;
                }
                if (typeof options.interval === "number") {
                    this._interval = options.interval;
                }
            }

            this._element.className = "timeline";

            this._viewport = document.createElement("div");
            this._element.appendChild(this._viewport);

            this._canvas = document.createElement("div");
            this._viewport.appendChild(this._canvas);

            this._viewport.style.position = "absolute";
            this._viewport.style.overflowX = "auto";
            this._viewport.style.overflowY = "hidden";

            this._canvas.style.position = "relative";
            this._canvas.style.overflow = "hidden";

            var viewportWidth = this._element.offsetWidth;
            var viewportHeight = this._element.offsetHeight;

            this._viewport.style.width = viewportWidth + "px";
            this._viewport.style.height = viewportHeight + "px";

            var marginTop = 0.5 * viewportHeight - 20;

            this._itemWidth = 80;
            this._canvas.style.width = Math.max((this._itemWidth * ((this._end - this._start) / this._interval + 1)), viewportWidth) + "px";

            var that = this;
            for (var i = this._start; i <= this._end; i += this._interval) {
                var caption = document.createElement("div");
                caption.innerText = i.toString();
                caption.className = "timeline-caption";
                caption.style.marginTop = marginTop + "px";

                var line = document.createElement("div");
                line.className = "timeline-line";

                var container = document.createElement("div");
                container.appendChild(caption);
                container.appendChild(line);
                container.className = "timeline-item";

                container.addEventListener("click", function () {
                    if (that._isZoomedOut) {
                        that._triggerZoom();
                    }
                }, false);

                container.style.position = "relative";
                container.style.overflow = "hidden";

                this._canvas.appendChild(container);
            }

            this._element.winControl = this;
        }, {
            // Public properties
            zoomableView: {
                get: function () {
                    if (!this._zoomableView) {
                        this._zoomableView = new ZoomableView(this);
                    }

                    return this._zoomableView;
                }
            },

            // Private properties
            _getPanAxis: function () {
                return "horizontal";
            },

            _configureForZoom: function (isZoomedOut, isCurrentView, triggerZoom, prefectchedPages) {
                this._isZoomedOut = isZoomedOut;
                this._triggerZoom = triggerZoom;
            },

            _setCurrentItem: function (x, y) {
                // First, convert the position into canvas coordinates
                x += this._viewport.scrollLeft;

                // Now choose the closest element
                var leftPos = this._canvas.firstElementChild.offsetLeft;

                var i = Math.floor((x - leftPos) / this._itemWidth);
                i = Math.max(0, Math.min(Math.floor((this._end - this._start) / this._interval), i));

                // Set focus on the element
                var element = this._canvas.children[i];
                element.focus();
            },

            _beginZoom: function () {
                // Hide the scrollbar and extend the content beyond the viewport
                var scrollOffset = -this._viewport.scrollLeft;

                this._viewport.style.overflowX = "visible";
                this._canvas.style.left = scrollOffset + "px";

                this._viewport.style.overflowY = "visible";
            },

            _getCurrentItem: function () {
                // Get the element with focus
                var focusedElement = document.activeElement;
                if (focusedElement.className === "timeline-caption" || focusedElement.className === "timeline-line") {
                    focusedElement = focusedElement.parentNode;
                } else if (focusedElement.className !== "timeline-item") {
                    focusedElement = this._canvas.firstElementChild;
                }

                // Get the corresponding item for the element
                var /*@override*/item = parseInt(focusedElement.children[0].innerText, 10);

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
                // Get the corresponding item for the element
                var year = Math.max(this._start, Math.min(this._end, item)),
                    element = this._canvas.children[Math.floor((year - this._start) / this._interval)];

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
            },

            _handlePointer: function (pointerId) {
                // Let the viewport handle panning gestures               
                this._viewport.msSetPointerCapture(pointerId);
            }
        })
    });
})();
