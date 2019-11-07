//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Color Globals
    var fillStyleOuterColor = "rgb(9, 126, 196)";
    var fillStyleInnerColor = "rgb(255, 255, 255)";

    // Draw grahic on canvas element
    function draw() {
        var canvas = document.getElementById("s3-hc-control");
        if (canvas) {
            var ctx = canvas.getContext('2d');
            ctx.beginPath();
            ctx.moveTo(194, 104);
            ctx.quadraticCurveTo(54, 104, 54, 246);
            ctx.quadraticCurveTo(54, 371, 194, 371);
            ctx.quadraticCurveTo(324, 371, 338, 272);
            ctx.lineTo(240, 272);
            ctx.arc(197, 272, 47, 0, Math.PI, false);
            ctx.lineTo(150, 256);
            ctx.lineTo(338, 256);
            ctx.quadraticCurveTo(338, 104, 194, 104);
            ctx.moveTo(154, 207);
            ctx.fillStyle = fillStyleOuterColor;
            ctx.fill();
            ctx.closePath();

            // Inner arc of e
            ctx.beginPath();
            ctx.fillStyle = fillStyleInnerColor;

            ctx.lineTo(240, 211);
            ctx.arc(197, 211, 47, 0, Math.PI, true);
            ctx.fill();
            ctx.closePath();
            WinJS.log && WinJS.log("Completed Drawing", "sample", "status");
        }
    }

    // Media Query List Listener that updates the color values if there is a High Contrast change
    function updateColorValues(listener) {
        if (listener.matches) {
            fillStyleOuterColor = "ButtonText";
            fillStyleInnerColor = "ButtonFace";
            draw();
        }
        else {
            fillStyleOuterColor = "rgb(9, 126, 196)";
            fillStyleInnerColor = "rgb(255, 255, 255)";
            draw();
        }
    }

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("scenario3Redraw").addEventListener("click", draw, false);

            // Check to see if we've started the sample in High Contrast, update colors accordingly
            if (matchMedia("(-ms-high-contrast)").matches) {
                fillStyleOuterColor = "ButtonText";
                fillStyleInnerColor = "ButtonFace";
            }

            // Register for Media Query List changes for High Contrast
            var mql = matchMedia("(-ms-high-contrast)");
            mql.addListener(updateColorValues);

            draw();
        }
    });
})();
