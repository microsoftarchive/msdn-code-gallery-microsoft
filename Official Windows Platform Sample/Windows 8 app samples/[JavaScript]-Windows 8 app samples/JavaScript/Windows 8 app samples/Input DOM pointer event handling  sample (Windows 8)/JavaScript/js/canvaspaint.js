//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/canvaspaint.html", {
        ready: function (element, options) {
            onLoad();
        }
    });

    var context;
    var brushList;
    var canvas;
    var output;
    var animationActive = false;

    function onLoad() {
        var element;
        canvas = document.getElementById("paintCanvas");
        output = document.getElementById("output");

        // account for margins
        canvas.width = output.clientWidth - 25;
        canvas.height = output.clientHeight - 125;

        context = canvas.getContext("2d");
        canvas.addEventListener("MSPointerDown", canvasHandler, false);
        canvas.addEventListener("MSPointerMove", canvasHandler, false);
        canvas.addEventListener("MSPointerUp", canvasHandler, false);
        canvas.addEventListener("MSPointerOver", canvasHandler, false);
        canvas.addEventListener("MSPointerOut", canvasHandler, false);
        canvas.addEventListener("MSPointerCancel", canvasHandler, false);
        context.lineWidth = 1;
        context.lineCap = "round";
        context.lineJoin = "round";

        initColorPalette();
        initToolbar();

        brushList = new Array();
    }

    function initColorPalette() {
        var element = document.getElementById("palette");
        var divs = element.getElementsByTagName("div");
        for (var idx = 0; idx < divs.length; idx++) {
            divs[idx].addEventListener("MSPointerUp", colorSelector, false);
        }
    }

    // Clear canvas for new drawing
    function initToolbar() {
        var element = document.getElementById("newFile");
        element.addEventListener("MSPointerUp", clearCanvas, false);
    }

    function colorSelector(evt) {
        context.strokeStyle = evt.srcElement.id;
        var element = document.getElementById("selectedColor");
        element.style.backgroundColor = evt.srcElement.id;
        evt.preventDefault();
    }

    function brushTool() {
        var brush = this;
        brush.started = false;
        brush.over = false;
        brush.prevX = 0;
        brush.prevY = 0;
        brush.currentX = 0;
        brush.currentY = 0;
        brush.lineWidth = 1;

        // Even though the choice of raw coordinates over predicted coordinates has performance
        // overhead we will use raw coordinates because predicted coordinates don't give
        // accurate results for our purpose.
        this.MSPointerDown = function (evt) {
            canvas.msSetPointerCapture(evt.pointerId);
            brush.currentX = evt.currentPoint.rawPosition.x;
            brush.currentY = evt.currentPoint.rawPosition.y;
            brush.prevX = brush.currentX;
            brush.prevY = brush.currentY;
            brush.started = true;
            brush.over = true;
            if (!animationActive) {
                window.requestAnimationFrame(animationHandler);
                animationActive = true;
            }
        };

        this.MSPointerOver = function (evt) {
            brush.over = true;
            if (brush.started) {
                brush.currentX = evt.currentPoint.rawPosition.x;
                brush.currentY = evt.currentPoint.rawPosition.y;
            } else if (evt.currentPoint.isInContact) {
                // If the Down occurred outside of the canvas element but the pointer is in contact,
                // simulate the Down behavior when the pointer enters the canvas
                brush.MSPointerDown(evt);
            }
        };

        this.MSPointerMove = function (evt) {
            if (brush.started) {
                // Adjust the line width by reading the contact width from
                // the event parameter. Use a width of 1 for pen and mouse.
                if (evt.pointerType === evt.MSPOINTER_TYPE_TOUCH) {
                    brush.lineWidth = evt.width / 2;
                } else {
                    brush.lineWidth = 1;
                }
                brush.currentX = evt.currentPoint.rawPosition.x;
                brush.currentY = evt.currentPoint.rawPosition.y;
            }
        };

        this.MSPointerUp = function (evt) {
            canvas.msReleasePointerCapture(evt.pointerId);
            brush.started = false;
        };

        this.MSPointerOut = function (evt) {
            brush.over = false;
        };

        this.MSPointerCancel = function (evt) {
            brush.over = false;
            brush.started = false;
        };
    }

    function canvasHandler(evt) {
        var brush;
        var func;

        if (brushList[evt.pointerId] === null ||
            brushList[evt.pointerId] === undefined) {
            brushList[evt.pointerId] = new brushTool();
        }

        brush = brushList[evt.pointerId];

        func = brush[evt.type];
        func(evt);

        if (!brush.started && !brush.over) {
            // clean up when the brush is finished
            delete brushList[evt.pointerId];
        }
    }

    function clearCanvas(evt) {
        context.clearRect(0, 0, canvas.width, canvas.height);
        brushList.length = 0;
    }

    function animationHandler() {
        animationActive = false;
        for (var idx in brushList) {
            var currentBrush = brushList[idx];
            if (currentBrush.started) {
                context.beginPath();
                context.lineWidth = currentBrush.lineWidth;
                context.moveTo(currentBrush.prevX, currentBrush.prevY);
                context.lineTo(currentBrush.currentX, currentBrush.currentY);
                context.stroke();
                currentBrush.prevX = currentBrush.currentX;
                currentBrush.prevY = currentBrush.currentY;
                animationActive = true;
            }
        }
        if (animationActive) {
            // Request for another callback until all pointers are gone.
            window.requestAnimationFrame(animationHandler);
        }
    }
})();
