//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var simpleKeyService = null;
    var controllerInitialized = false;

    var page = WinJS.UI.Pages.define("/html/SimpleBLEController.html", {
        ready: function (element, options) {
            // Initialization code

            // Only initialize once
            if (!controllerInitialized) {
                initializeControllerDevices();
                controllerInitialized = true;
            }
        }
    });

    function applicationActivated() {
        try {
            if (simpleKeyService !== null) {
                simpleKeyService.ApplicationActivated();
            }
        } catch (exception) {
            WinJs.log && WinJS.log(exception.toString(), "sample", "error");
        }
    }

    function applicationSuspended() {
        try {
            if (simpleKeyService !== null) {
                simpleKeyService.ApplicationSuspended();
            }
        } catch (exception) {
            WinJs.log && WinJS.log(exception.toString(), "sample", "error");
        }
    }

    function initializeControllerDevices() {
        // Setup event handlers for application lifecycle events
        Windows.UI.WebUI.WebUIApplication.addEventListener('suspending', applicationSuspended);
        Windows.UI.WebUI.WebUIApplication.addEventListener('resuming', applicationActivated);

        WinJS.log && WinJS.log("Looking for bluetooth controllers...", "sample", "status");

        // Use Wpd Automation to query for devices that implement the Simple Key Service
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync("System.Devices.InterfaceClassGuid:=\"" + Constants.simpleKeyDeviceUUID + "\"", null).
            done(function (devices) {
                if (devices.length > 0) {
                    WinJS.log && WinJS.log("Found a bluetooth controller...", "sample", "status");
                    try {
                        var deviceFactory = new ActiveXObject("PortableDeviceAutomation.Factory");

                        deviceFactory.getDeviceFromIdAsync(devices[0].id, function (device) {
                            // Store the device's first service
                            simpleKeyService = device.services[0];

                            // Set up ApplicationActivated to be called asynchronously (fire-and-forget mode)
                            simpleKeyService.onApplicationActivatedComplete = function() {};

                            // Set up ApplicationSuspended to be called asynchronously (fire-and-forget mode)
                            simpleKeyService.onApplicationSuspendedComplete = function() {};

                            // Register for custom onKeyPressed event
                            simpleKeyService.onKeyPressed = function (keyPressValue) {

                                var leftButton = document.getElementById("leftButton");
                                var rightButton = document.getElementById("rightButton");

                                WinJS.log && WinJS.log("Received a value of : " + keyPressValue.toString());

                                // Handle the value received from the device
                                switch (keyPressValue) {
                                    // No Button is pressed
                                    case 0:
                                        leftButton.style.background = "red";
                                        rightButton.style.background = "red";
                                        break;
                                        // Left Button is pressed
                                    case 1:
                                        leftButton.style.background = "green";
                                        rightButton.style.background = "red";
                                        break;
                                        // Right Button is pressed
                                    case 2:
                                        leftButton.style.background = "red";
                                        rightButton.style.background = "green";
                                        break;
                                        // Both Buttons are pressed
                                    case 3:
                                        leftButton.style.background = "green";
                                        rightButton.style.background = "green";
                                        break;
                                    default:
                                        break;
                                };
                            };
                            simpleKeyService.ApplicationActivated();

                        }, function (errorCode) {
                            WinJS.log && WinJS.log("Getting the device failed with error: " + errorCode.toString(16), "sample", "error");
                        });

                    } catch (e) {
                        reportFailure();
                    }

                } else {
                    reportFailure();
                }
            });
    };


    function reportFailure() {
        // Remove the UI elements for normal device operation
        var hrmOutput = document.getElementById(Constants.sampleContent);
        while (hrmOutput.childNodes.length > 0) {
            hrmOutput.removeChild(hrmOutput.childNodes[0]);
        }

        // Report device status to the user
        WinJS.log && WinJS.log("No bluetooth controller found !", "sample", "error");
    }

})();
