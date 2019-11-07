//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/Scenario2_SetRotationPreference.html", {
        ready: function (element, options) {
            var textOut = document.getElementById("deviceRotation");
            textOut.innerText = Math.floor(m_rotationAngle).toString();

            document.getElementById("orientationPref").addEventListener("click", lockUnLock, false);
            addAccelerometerListener();
            setButtonText();
        },
        unload: function () {
            removeAccelerometerListener();
        }
    });

    var m_rotationAngle = 0;
    var m_accelerometer;

    function getAccelerometer() {
        if (!m_accelerometer) {
            try {
                m_accelerometer = Windows.Devices.Sensors.Accelerometer.getDefault();
                if (!m_accelerometer) {
                    window.console.log("failed to get accelerometer");
                }
            } catch (e) {
                window.console.log(e);
            }
        }
    }

    function addAccelerometerListener() {
        try {
            getAccelerometer();
            if (!m_accelerometer) {
                return;
            }

            m_accelerometer.addEventListener("readingchanged", calculateDeviceRotationScenario);
        } catch (e) {
            window.console.log(e);
        }
    }

    function removeAccelerometerListener() {
        try {
            getAccelerometer();
            if (!m_accelerometer) {
                return;
            }

            m_accelerometer.removeEventListener("readingchanged", calculateDeviceRotationScenario);
        } catch (e) {
            window.console.log(e);
        }
    }

    // Compute the difference, in degrees, between the device's orientation and the up direction.
    // We only take into account the X and Y dimensions, i.e. device screen is perpendicular to the ground.
    function calculateDeviceRotationScenario(eventArgs) {
        try {
            m_rotationAngle = Constants.UIAngleOffset -
                Math.atan2(eventArgs.reading.accelerationY, eventArgs.reading.accelerationX) * 180 / Math.PI;

            // Ensure that the range of the value is within [0, 360).
            if (m_rotationAngle >= 360) {
                m_rotationAngle -= 360;
            }

            var textOut = document.getElementById("deviceRotation");
            textOut.innerText = Math.floor(m_rotationAngle).toString();
            updateArrowForRotation();
        } catch (e) {
            window.console.log(e);
        }
    }

    // Rotate the UI arrow image to point up, adjusting for the accelerometer and screen rotation.
    function updateArrowForRotation() {
        var screenRotation = 0;

        // Adjust the UI steering angle to account for screen rotation.
        switch (Windows.Graphics.Display.DisplayInformation.getForCurrentView().currentOrientation) {
            case Windows.Graphics.Display.DisplayOrientations.landscape:
                screenRotation = 0;
                break;
            case Windows.Graphics.Display.DisplayOrientations.portrait:
                screenRotation = 270;
                break;
            case Windows.Graphics.Display.DisplayOrientations.landscapeFlipped:
                screenRotation = 180;
                break;
            case Windows.Graphics.Display.DisplayOrientations.portraitFlipped:
                screenRotation = 90;
                break;
            default:
                screenRotation = 0;
                break;
        }

        var steeringAngle = m_rotationAngle - screenRotation;

        // Ensure the steering angle is positive.
        if (steeringAngle < 0) {
            steeringAngle += 360;
        }

        // Update the UI based on steering action.
        var arrow = document.getElementById("arrow");
        arrow.style.transform = "rotate(" + steeringAngle + "deg)";
        m_rotationAngle = steeringAngle;
    }

    function setButtonText() {
        var textOut = document.getElementById("orientationPref");
        if (Windows.Graphics.Display.DisplayInformation.autoRotationPreferences === Windows.Graphics.Display.DisplayOrientations.none) {
            textOut.innerText = "Lock";
        } else {
            textOut.innerText = "Unlock";
        }
    }


    function lockUnLock() {
        var currentOrientation = Windows.Graphics.Display.DisplayInformation.getForCurrentView().currentOrientation;
        var dispInfo = Windows.Graphics.Display.DisplayInformation;

        if (dispInfo.autoRotationPreferences === Windows.Graphics.Display.DisplayOrientations.none) {
            // Get the current screen orientation and set it as the preference.
            dispInfo.autoRotationPreferences = currentOrientation;
        } else {
            // Reset to no preference.
            dispInfo.autoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.none;
        }

        // Update the button to reflect the current state.
        setButtonText();
    }
})();
