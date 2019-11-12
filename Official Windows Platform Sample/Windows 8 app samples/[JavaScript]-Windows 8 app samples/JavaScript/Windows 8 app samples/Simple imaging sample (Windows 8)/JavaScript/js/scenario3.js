//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Parameters that control the behavior of the Canvas and artistic effect.
    var CANVAS_WIDTH_FACTOR = 0.4;
    var STROKES_PER_BATCH = 1000;
    // STROKES_PER_CLICK must be a multiple of STROKES_PER_BATCH.
    var STROKES_PER_CLICK = 25000;

    // Keep objects in-scope across the lifetime of the scenario.
    var Context;
    var OriginalPixelData;
    var NumStrokes;
    var CanvasDimensions;

    // Namespace and API aliases
    var Pickers = Windows.Storage.Pickers;
    var Imaging = Windows.Graphics.Imaging;

    function id(elementId) {
        return document.getElementById(elementId);
    }

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            id("buttonOpen").addEventListener("click", openHandler, false);
            id("buttonRender").addEventListener("click", renderHandler, false);
            id("buttonSave").addEventListener("click", saveHandler, false);

            // State must be reset before using the scenario.
            resetState();
        }
    });

    function resetState() {
        id("buttonOpen").disabled = false;
        id("buttonSave").disabled = true;
        id("buttonRender").disabled = true;

        // Triggers a reset of the canvas contents.
        id("outputCanvas").width = id("outputCanvas").width;

        Context = null;
        OriginalPixelData = null;
        NumStrokes = 0;
        CanvasDimensions = {};
    }

    function openHandler() {
        resetState();

        Helpers.getFileFromOpenPickerAsync().done(function (file) {
            var objectUrl = window.URL.createObjectURL(file, { oneTimeOnly: true });
            var canvasImage = new Image();
            canvasImage.src = objectUrl;
            canvasImage.alt = file.name;
            canvasImage.addEventListener("load", function () {
                var canvas = id("outputCanvas");
                var context = canvas.getContext("2d");

                // Determine what height and width are needed to preserve the aspect ratio.
                canvas.width = parseInt(
                    document.documentElement.clientWidth * CANVAS_WIDTH_FACTOR);

                CanvasDimensions = Helpers.getScaledDimensions(
                    this.width,
                    this.height,
                    canvas.width
                    );

                canvas.width = CanvasDimensions.width;
                canvas.height = CanvasDimensions.height;

                context.drawImage(this, 0, 0, canvas.width, canvas.height);
                var originalPixelData = context.getImageData(0, 0, canvas.width, canvas.height);
                OriginalPixelData = originalPixelData;
                Context = context;

                WinJS.log && WinJS.log("Loaded image file: " + file.name, "sample", "status");
                id("buttonRender").disabled = false;
                id("buttonSave").disabled = false;
            });
        }, function (error) {
            WinJS.log && WinJS.log("Failed to load file: " + error.message, "sample", "error");
            resetState();
        });
    }

    function saveHandler() {
        id("buttonSave").disabled = true;
        id("buttonRender").disabled = true;
        WinJS.log && WinJS.log("Saving to a new file...", "sample", "status");

        // Keep data in-scope across multiple asynchronous methods.
        var encoderId;
        var filename;
        var stream;

        Helpers.getFileFromSavePickerAsync().then(function (file) {
            filename = file.name;

            switch (file.fileType) {
                case ".jpg":
                    encoderId = Imaging.BitmapEncoder.jpegEncoderId;
                    break;
                case ".bmp":
                    encoderId = Imaging.BitmapEncoder.bmpEncoderId;
                    break;
                case ".png":
                default:
                    encoderId = Imaging.BitmapEncoder.pngEncoderId;
                    break;
            }

            return file.openAsync(Windows.Storage.FileAccessMode.readWrite);
        }).then(function (_stream) {
            stream = _stream;
            
            // BitmapEncoder expects an empty output stream; the user may have selected a
            // pre-existing file.
            stream.size = 0;
            return Imaging.BitmapEncoder.createAsync(encoderId, stream);
        }).then(function (encoder) {
            var width = id("outputCanvas").width;
            var height = id("outputCanvas").height;
            var outputPixelData = Context.getImageData(0, 0, width, height);

            encoder.setPixelData(
                Imaging.BitmapPixelFormat.rgba8,
                Imaging.BitmapAlphaMode.straight,
                width,
                height,
                96, // Horizontal DPI
                96, // Vertical DPI
                outputPixelData.data
                );

            return encoder.flushAsync();
        }).then(function () {
            WinJS.log && WinJS.log("Saved new file: " + filename, "sample", "status");
            id("buttonSave").disabled = false;
            id("buttonRender").disabled = false;
        }).then(null, function (error) {
            WinJS.log && WinJS.log("Failed to save file: " + error.message, "sample", "error");
        }).done(function () {
            stream && stream.close();
        });
    }

    // Each time this function is run, it applies STROKES_PER_CLICK strokes.
    // It batches the rendering into sets of STROKES_PER_BATCH strokes. After each
    // batch completes, the UI is updated.
    function renderHandler() {
        id("buttonRender").disabled = true;
        id("buttonSave").disabled = true;

        var renderCallback = function () {
            for (var i = 0; i < STROKES_PER_BATCH; i++) {
                // Get the pixel coordinates.
                var x = Math.floor(Math.random() * CanvasDimensions.width);
                var y = Math.floor(Math.random() * CanvasDimensions.height);

                // Get the original pixel color data.
                var pixelOffset = (y * CanvasDimensions.width + x) * 4;
                var r = OriginalPixelData.data[pixelOffset];
                var g = OriginalPixelData.data[++pixelOffset];
                var b = OriginalPixelData.data[++pixelOffset];
                var a = OriginalPixelData.data[++pixelOffset];
                Context.fillStyle = "rgba(" + r + ", " + g + ", " + b + ", " + a + ")";

                Context.strokeStyle = "rgba(" + r + ", " + g + ", " + b + ", " + a + ")";

                // Calculate the stroke parameters.
                var minLength = 5;
                var maxLength = 15;
                var angle = Math.random() * Math.PI;
                var radius = (minLength + Math.random() * (maxLength - minLength)) / 2;
                var xOffset = radius * Math.cos(angle);
                var yOffset = radius * Math.sin(angle);

                // Draw the stroke.
                Context.beginPath();
                Context.moveTo(x - xOffset, y - yOffset);
                Context.lineTo(x + xOffset, y + yOffset);
                Context.stroke();

                NumStrokes++;
            }

            WinJS.log && WinJS.log("Applied " + NumStrokes + " strokes", "sample", "status");

            if ((NumStrokes % STROKES_PER_CLICK) === 0) {
                id("buttonRender").disabled = false;
                id("buttonSave").disabled = false;
            } else {
                window.setImmediate(renderCallback);
            }
        };

        window.setImmediate(renderCallback);
    }
})();
