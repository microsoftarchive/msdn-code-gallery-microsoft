//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//// PARTICULAR PURPOSE. 
//// 
//// Copyright (c) Microsoft Corporation. All rights reserved 

(function () {
    "use strict";

    var context;
    var controller;
    var x;
    var y;
    var app = WinJS.Application;

    function renderLoop() {
        var state = controller.getState();

        if (state.connected) {
            controllerPresent.style.visibility = "hidden";

            context.beginPath();

            // Gamepad rightTrigger values are between 0 and 255. Line Width reduced to one tenth of rightTrigger.
            context.lineWidth = state.rightTrigger / 10 + 10;

            // Gamepad thumbstick values are between -32768 and 32767. Drawing position is moved incrementally if
            // the thumbstick value exceeds a deadzone value of 6500. 
            var x2 = Math.abs(state.leftThumbX) > 6500 ? x + (state.leftThumbX / 32767) * 5 : x;
            var y2 = Math.abs(state.rightThumbY) > 6500 ? y - (state.rightThumbY / 32767) * 5 : y;

            // Clip to canvas space
            if (x2 < 0) {
                x2 = 0;
            }
            if (y2 < 0) {
                y2 = 0;
            }
            if (x2 > sketchSurface.width) {
                x2 = sketchSurface.width;
            }
            if (y2 > sketchSurface.height) {
                y2 = sketchSurface.height;
            }

            context.moveTo(x, y);
            context.lineTo(x2, y2);
            context.stroke();

            x = x2;
            y = y2;
        }
        else {
            controllerPresent.style.visibility = "";
        };

        window.requestAnimationFrame(renderLoop);
    };
   

    app.onactivated = function (eventObj) {
        if (eventObj.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // The canvas is sized to fit the window, minus the fixed columns
            sketchSurface.width = window.innerWidth - 100;
            sketchSurface.height = window.innerHeight - 150;

            // The initial position is centered in the canvas
            x = sketchSurface.width / 2;
            y = sketchSurface.height / 2;

            // Set up the drawing context
            context = sketchSurface.getContext("2d");
            context.strokeStyle = "black";
            
            // Although the API supports up to 4 controllers per machine,
            // this sample only works with a single controller.
            controller = new GameController.Controller(0);

            // Start rendering loop
            window.requestAnimationFrame(renderLoop);
        };
    };

    app.start();
})();
