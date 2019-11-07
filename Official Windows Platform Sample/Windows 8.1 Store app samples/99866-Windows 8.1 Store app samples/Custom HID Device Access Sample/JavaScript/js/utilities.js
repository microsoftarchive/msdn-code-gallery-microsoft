//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var UtilitiesClass = WinJS.Class.define(null, null,
    {
        /// <summary>
        /// Displays the compatible scenarios and hides the non-compatible ones.
        /// If there are no supported devices, the scenarioContainer will be hidden and an error message
        /// will be displayed.
        /// </summary>
        /// <param name="scenarios">The key is the device type that the value, scenario, supports.</param>
        /// <param name="scenarioContainer">The container that encompasses all the scenarios that are specific to devices</param>
        setUpDeviceScenarios: function (scenarios, scenarioContainer) {
            var supportedScenario = null;

            if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
                for (var deviceTypeSupported in scenarios) {
                    // Enable the scenario if it's generic or the device type matches
                    if (parseInt(deviceTypeSupported) === SdkSample.Constants.deviceType.all
                        || parseInt(deviceTypeSupported) === this.getDeviceType(SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device)) {
                        // Make the scenario visible in case other devices use the same scenario and collapsed it.
                        scenarios[deviceTypeSupported].style.visibility = "visible";

                        supportedScenario = scenarios[deviceTypeSupported];

                        break;
                    }
                }
            }

            if (!supportedScenario) {
                scenarioContainer.parentNode.removeChild(scenarioContainer);    // Remove the container holding all scenarios so that it doesn't tak eup UI space

                WinJS.log && WinJS.log("No supported devices are currently selected", "sample", "error");
            } else {
                // Remove all scenarios that are not going to be displayed so that they don't take up UI space
                for (var deviceTypeToRemove in scenarios) {
                    if (scenarios[deviceTypeToRemove] !== supportedScenario) {
                        scenarioContainer.removeChild(scenarios[deviceTypeToRemove]);
                    }
                }
            }
        },
        notifyDeviceNotConnected: function () {
            WinJS.log && WinJS.log("Device is not connected, please select a plugged in device to try the scenario again", "sample", "error");
        },
        /// <summary>
        /// Device type of the device provided device.
        /// </summary>
        /// <param name="device"></param>
        /// <returns>The DeviceType of the device or DeviceType.None if there are no devices connected or is not recognized</returns>
        getDeviceType: function (device) {
            if (device) {
                if (device.vendorId === SdkSample.Constants.superMutt.device.vid
                    && device.productId === SdkSample.Constants.superMutt.device.pid) {
                    return SdkSample.Constants.deviceType.superMutt;
                }
            }

            return SdkSample.Constants.deviceType.none;
        },
        isSuperMuttDevice: function (hidDevice) {
            return (UtilitiesClass.getDeviceType(hidDevice) === SdkSample.Constants.deviceType.superMutt);
        }
    });

    WinJS.Namespace.define(SdkSample.Constants.sampleNamespace, {
        utilities: UtilitiesClass
    });
})();