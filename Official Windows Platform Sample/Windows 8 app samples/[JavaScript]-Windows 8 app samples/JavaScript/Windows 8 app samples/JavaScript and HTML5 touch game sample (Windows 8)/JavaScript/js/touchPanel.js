//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var TouchPanel = WinJS.Class.define(
    null,
{
    boundCanvas: null,
    touchHandler: null,
    enabled: function () { return window.navigator.msPointerEnabled; },

    initialize: function (canvas, touchHandler) {
        this.boundCanvas = canvas;
        if (touchHandler) {
            canvas.addEventListener("MSPointerDown", this.start, false);
            canvas.addEventListener("MSPointerUp", this.end, false);
            canvas.addEventListener("MSPointerMove", this.move, false);
            canvas.addEventListener("MSPointerOut", this.cancel, false);
            canvas.addEventListener("MSPointerCancel", this.cancel, false);
            this.touchHandler = touchHandler;
        }
    },

    start: function (e) {
        e.preventDefault();

        if ((e.pointerType === e.MSPOINTER_TYPE_MOUSE) && (e.button === 0)) {
            // We don't need to track mouse unless buttons are pressed
            return;
        }

        // Call registered handler in game logic
        GameManager.touchPanel.touchHandler("start", e);
    },

    move: function (e) {
        e.preventDefault();

        // Call registered handler in game logic
        GameManager.touchPanel.touchHandler("move", e);
    },

    end: function (e) {
        // Call registered handler in game logic
        GameManager.touchPanel.touchHandler("end", e);
    },

    cancel: function (e) {
        // Call registered handler in game logic
        GameManager.touchPanel.touchHandler("cancel", e);
    },

});
