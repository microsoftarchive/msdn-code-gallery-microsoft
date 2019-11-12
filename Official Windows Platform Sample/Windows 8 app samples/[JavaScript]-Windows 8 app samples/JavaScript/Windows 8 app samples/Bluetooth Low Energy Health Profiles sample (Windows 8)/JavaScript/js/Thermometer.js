//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var tempService = null;
    var uiVisible = false;
    var tempInitialized = false;

    var page = WinJS.UI.Pages.define("/html/Thermometer.html", {
        ready: function (element, options) {
            // Initialize the Thermometer device scenario

            uiVisible = true;

            if (!tempInitialized) {
                // initializeTemperatureDevices will set the value of tempInitialized depending
                // on whether devices were successfully initialized or not.
                initializeTemperatureDevicesAsync();
            }

            // Populate the UI with existing data
            UIHelper.refreshUI(Constants.thermometerDeviceShortName, Constants.thermometerDeviceIndex);
        },

        // This function gets called just before the current scenario gets unloaded
        // because the user chose a different scenario
        unload: function () {
            // Set the flag to not update the UI while this scenario is not visible
            uiVisible = false;
        }
    });

    function initializeTemperatureDevicesAsync() {

        // Setup event handlers for application lifecycle events
        Windows.UI.WebUI.WebUIApplication.addEventListener('suspending', applicationSuspended);
        Windows.UI.WebUI.WebUIApplication.addEventListener('resuming', applicationActivated);

        // Initialize Temperature Devices
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync("System.Devices.InterfaceClassGuid:=\"" + Constants.thermometerUUID + "\"", null).
            done(function (devices) {
                // If devices were found, proceed with initialization
                if (devices.length > 0) {
                    try {

                        // Use WPD Automation to initialize the device objects
                        var deviceFactory = new ActiveXObject("PortableDeviceAutomation.Factory");

                        var devServiceName = document.getElementById('deviceServiceName' + Constants.thermometerDeviceShortName);
                        devServiceName.innerText = devices[0].name;

                        // For the purpose of this sample we will initialize the first device
                        deviceFactory.getDeviceFromIdAsync(devices[0].id, function (device) {

                            // Initialize the temperature service
                            tempService = device.services[0];

                            // Set up ApplicationActivated to be called asynchronously (fire-and-forget mode)
                            tempService.onApplicationActivatedComplete = function() {};

                            // Set up ApplicationSuspended to be called asynchronously (fire-and-forget mode)
                            tempService.onApplicationSuspendedComplete = function() {};

                            var devs = DeviceData.getDevices();
                            var devId = Constants.thermometerDeviceIndex;
                            devs[devId] = {
                                devId: devId,
                                name: devices[0].name,
                                description: devices[0].id,
                                data: [],
                                handler: function (timestamp, thermometerMeasurementValue) {
                                    try {
                                        var measuredValue = {
                                            timestamp: new Date(timestamp * 1000),
                                            value: thermometerMeasurementValue,
                                            toString: function () {
                                                return this.value + " \u00B0C @ " + this.timestamp;
                                            },
                                        };

                                        DeviceData.addValue(
                                            Constants.thermometerDeviceIndex,
                                            measuredValue);

                                        if (uiVisible) {
                                            UIHelper.updateUI(Constants.thermometerDeviceShortName, Constants.thermometerDeviceIndex, measuredValue);
                                        }
                                    } catch (exception) {
                                        WinJS.log && WinJS.log(exception.toString(), "sample", "error");
                                    }
                                }
                            };

                            tempService.onTemperatureMeasurement = devs[devId].handler;
                            tempInitialized = true;
                            UIHelper.refreshUI(Constants.thermometerDeviceShortName, Constants.thermometerDeviceIndex);
                            tempService.ApplicationActivated();

                        }, function (errorCode) {
                            WinJS.log && WinJS.log("Getting the device failed with error: " + errorCode.toString(16), "sample", "error");
                        });

                    } catch (exception) {
                        WinJS.log && WinJS.log(exception.toString(), "sample", "error");
                    }
                } else {
                    // Update the UI to signal the scenario failure because of lack of connected devices
                    UIHelper.reportScenarioFailure(Constants.thermometerDeviceShortName, "Thermometer");
                }
            });
    }

    function applicationActivated() {
        try {
            // Invoking a Wpd Service Method to let the driver know that
            // the application has foreground and can receive notifications
            if (tempService !== null) {
                tempService.ApplicationActivated();
            }
        } catch (exception) {
            WinJS.log && WinJS.log(exception.toString(), "sample", "error");
        }
    }

    function applicationSuspended() {
        try {
            // Invoking a Wpd Service Method to let the driver know that
            // the application has been suspended and won't receive notifications
            if (tempService !== null) {
                tempService.ApplicationSuspended();
            }
        } catch (exception) {
            WinJS.log && WinJS.log(exception.toString(), "sample", "error");
        }
    }

    WinJS.Namespace.define('TemperatureDevices',
    {
        initializeTemperatureDevicesAsync: initializeTemperatureDevicesAsync,
        applicationActivated: applicationActivated,
        applicationSuspended: applicationSuspended
    });

})();
