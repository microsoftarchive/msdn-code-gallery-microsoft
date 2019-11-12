//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved



(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/rotation.html", {
        ready: function (element, options) {
            // add the listener for accelerometer  
            addAccelerometerListener();
        },
        unload: function () {
            removeAccelerometerListener();
        }
    });

    var rotationAngle = 0;
    var accelerometer;
    var toDegrees = 180 / Math.PI;
   
    function getAccelerometer() {
        if (!accelerometer) {
            try {
                accelerometer = Windows.Devices.Sensors.Accelerometer.getDefault();
                if (!accelerometer) {
                    window.console.log("failed to get accelerometer");
                }
            } catch (e) {
                window.console.log(e);
            }
        }

    }

    function addAccelerometerListener() {
        // Register for Accelerometer change event
        try {
            getAccelerometer();
            if (!accelerometer) {
                return;
            }

            accelerometer.addEventListener("readingchanged", calculateDeviceRotationScenario);
        } catch (e) {
            window.console.log(e);
        }
    }

    function removeAccelerometerListener() {
        // Un-Register for Accelerometer change event because that scenario is no longer active
        try {
            getAccelerometer();
            if (!accelerometer) {
                return;
            }

            accelerometer.removeEventListener("readingchanged", calculateDeviceRotationScenario);
        } catch (e) {
            window.console.log(e);
        }
    }

    function calculateDeviceRotationScenario(eventArgs) {
        try {
            // Compute the rotation angle based on the accelerometer's position
            var angle = Math.atan2(eventArgs.reading.accelerationY, eventArgs.reading.accelerationX) * toDegrees;

            // Since our arrow points upwards insted of the right, we rotate the coordinate system by 90 degrees
            angle += 90;

            // Ensure that the range of the value is between [0, 360)
            if (angle < 0) {
                angle += 360;
            }
            
            rotationAngle = angle;

            var textOut;
            textOut = document.getElementById("deviceRotation");
            textOut.innerText = rotationAngle.toString();
            updateArrowForRotation();
        } catch (e) {
            window.console.log(e);
        }
    }
    
    function updateArrowForRotation() {
        // Obtain current rotation taking into account a Landscape first or a Portrait first device
        var screenRotation = 0;

        // Native orientation can only be Landscape or Portrait
        if (Windows.Graphics.Display.DisplayProperties.nativeOrientation === Windows.Graphics.Display.DisplayOrientations.landscape) {
            switch (Windows.Graphics.Display.DisplayProperties.currentOrientation) {
                case Windows.Graphics.Display.DisplayOrientations.landscape:
                    screenRotation = 0;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.portrait:
                    screenRotation = 90;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
                    screenRotation = 180;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
                    screenRotation = 270;
                    break;
                default:
                    screenRotation = 0;
                    break;
            }
        } else {
            switch (Windows.Graphics.Display.DisplayProperties.currentOrientation) {
                case Windows.Graphics.Display.DisplayOrientations.landscape:
                    screenRotation = 270;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.portrait:
                    screenRotation = 0;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
                    screenRotation = 90;
                    break;
                case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
                    screenRotation = 180;
                    break;
                default:
                    screenRotation = 270;
                    break;
            }
        }

        var steeringAngle = rotationAngle - screenRotation;

        // Keep the steering angle positive             
        if (steeringAngle < 0) {
            steeringAngle = 360 + steeringAngle;
        }

        // Update the UI based on steering action
        var arrow;
        arrow = document.getElementById("arrow");
        arrow.style.transform = "rotate(-" + steeringAngle + "deg)";
        rotationAngle = steeringAngle;

    }
     

})();
