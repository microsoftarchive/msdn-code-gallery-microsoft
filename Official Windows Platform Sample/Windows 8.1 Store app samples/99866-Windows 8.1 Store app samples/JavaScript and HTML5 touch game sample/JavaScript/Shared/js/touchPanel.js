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
    enabled: function () { return window.navigator.pointerEnabled; },

    initialize: function (canvas, touchHandler) {
        this.boundCanvas = canvas;
        if (touchHandler) {
            canvas.addEventListener("pointerdown", this.start, false);
            canvas.addEventListener("pointerup", this.end, false);
            canvas.addEventListener("pointermove", this.move, false);
            canvas.addEventListener("pointerout", this.cancel, false);
            canvas.addEventListener("pointercancel", this.cancel, false);
            this.touchHandler = touchHandler;
        }
    },

    start: function (e) {
        e.preventDefault();

        if ((e.pointerType === "mouse") && (e.button === 0)) {
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
