//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Locations of the guitar and wood image resources.
    // Before you run this JavaScript project (BlockCompressedImages), you must do the following:
    // 1. Build the ImageContentPipeline project. This converts the JPEG and PNG image assets
    //    in ImageContentPipeline\OriginalAssets to block compressed (DDS) form in the
    //    BlockCompressedAssets folder.
    // 2. Add the resulting DDS images to this project by selecting Project > Add Existing Item
    //    and navigating to BlockCompressedAssets.
    var guitarPath = "BlockCompressedAssets\\guitar-transparent.dds";
    var woodPath = "BlockCompressedAssets\\oldWood4_nt.dds";

    // Canvas 2D context
    var context;

    // Image element
    var guitar;
    var wood;

    // Boolean
    var isGuitarReady;
    var isWoodReady;

    // Number
    var rotationAngle;
    var contextWidth;
    var contextHeight;
    var guitarOffsetX;
    var guitarOffsetY;

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            // Initialize canvas state while images are loading.
            context = document.getElementById("canvas").getContext("2d");
            contextWidth = document.getElementById("canvas").width;
            contextHeight = document.getElementById("canvas").height;
            context.fillStyle = "rgba(0,0,0,1)";
            context.fillRect(0, 0, contextWidth, contextHeight);
            rotationAngle = 0;
            window.requestAnimationFrame(drawCanvas);

            var packageLocation = Windows.ApplicationModel.Package.current.installedLocation;
            
            packageLocation.getFileAsync(guitarPath).then(function (file) {
                guitar = new Image();
                guitar.onload = function () {
                    // Determine offsets so we can draw the guitar centered in the canvas.
                    guitarOffsetX = guitar.width / -2;
                    guitarOffsetY = guitar.height / -2;

                    isGuitarReady = true;
                };

                guitar.src = window.URL.createObjectURL(file, { oneTimeOnly: true });

                return packageLocation.getFileAsync(woodPath);
            }).done(function (file) {
                wood = new Image();
                wood.onload = function () {
                    isWoodReady = true;
                };

                wood.src = window.URL.createObjectURL(file, { oneTimeOnly: true });
            }, function (err) {
                WinJS.log && WinJS.log("Could not find one or more image assets. Make sure to first " +
                    "build the ImageContentPipeline C++ project and add the DDS files in the " +
                    "BlockCompressedAssets folder to the BlockCompressedImages JS project.", "sample", "error");
            });
        }
    });

    // Callback for window.requestAnimationFrame.
    // Performs some simple imaging operations on the block compressed resources using a canvas.
    function drawCanvas() {
        if ((isGuitarReady === true) && (isWoodReady === true)) {
            context.drawImage(wood, 0, 0);

            // Draw a rotated guitar over the wood background. The guitar has transparency (BC3 format).
            context.save();
            context.translate(contextWidth / 2, contextHeight / 2);
            context.rotate(rotationAngle);
            rotationAngle += 0.01;
            context.drawImage(guitar, guitarOffsetX, guitarOffsetY);
            context.restore();
        }

        window.requestAnimationFrame(drawCanvas);
    }

})();
