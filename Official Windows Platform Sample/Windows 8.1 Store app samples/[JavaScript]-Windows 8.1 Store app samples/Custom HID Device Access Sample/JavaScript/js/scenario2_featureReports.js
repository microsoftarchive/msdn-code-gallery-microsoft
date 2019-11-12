//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var FeatureReportsClass = WinJS.Class.define(null, {
        /// <summary>
        /// Uses feature report to get the LED blink pattern. 
        /// 
        /// Any errors in async function will be passed down the task chain and will not be caught here because errors should 
        /// be handled at the end of the task chain.
        /// 
        /// The simplest way to obtain a byte from the buffer is by using a DataReader. DataReader provides a simple way
        /// to read from buffers (e.g. can return bytes, strings, ints).
        /// </summary>
        /// <returns>A promise that can be used to chain more methods after completing the scenario</returns>
        getLedBlinkPatternAsync: function () {
            return SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.getFeatureReportAsync(SdkSample.Constants.superMutt.ledPattern.reportId).then(function (featureReport) {
                if (featureReport.data.length === 2) {
                    var reader = Windows.Storage.Streams.DataReader.fromBuffer(featureReport.data);

                    // First byte is always report id
                    var reportId = reader.readByte();
                    var pattern = reader.readByte();

                    WinJS.log && WinJS.log("The Led blink pattern is " + pattern, "sample", "status");
                } else {
                    WinJS.log && WinJS.log("Expecting 2 bytes, but received " + featureReport.data.length, "sample", "error");
                }
            });
        },
        /// <summary>
        /// Uses a feature report to set the blink pattern on the SuperMutt's LED. 
        ///
        /// Please note that when we create an FeatureReport, all data is nulled out in the report. Since we will only be modifying 
        /// data we care about, the other bits that we don't care about, will be zeroed out. Controls will effectively do the same thing (
        /// all bits are zeroed out except for the bits we care about).
        ///
        /// Any errors in async function will be passed down the task chain and will not be caught here because errors should be handled 
        /// at the end of the task chain.
        ///
        /// The SuperMutt has the following patterns:
        /// 0 - LED always on
        /// 1 - LED flash 2 seconds on, 2 off, repeat
        /// 2 - LED flash 2 seconds on, 1 off, 2 on, 4 off, repeat
        /// ...
        /// 7 - 7 iterations of 2 on, 1 off, followed by 4 off, repeat
        /// </summary>
        /// <param name="pattern">A number from 0-7. Each number represents a different blinking pattern</param>
        /// <returns>A promise that can be used to chain more methods after completing the scenario</returns>
        setLedBlinkPatternAsync: function (pattern) {
            var featureReport = SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.createFeatureReport(SdkSample.Constants.superMutt.ledPattern.reportId);

            var writer = new Windows.Storage.Streams.DataWriter();

            writer.writeByte(featureReport.id);
            writer.writeByte(pattern);

            featureReport.data = writer.detachBuffer();

            return SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.sendFeatureReportAsync(featureReport).then(function (bytesSend) {
                WinJS.log && WinJS.log("The Led blink pattern is set to " + pattern, "sample", "status");
            });
        }
    }, null);

    var featureReportsForSuperMutt = new FeatureReportsClass();

    var page = WinJS.UI.Pages.define("/html/scenario2_featureReports.html", {
        ready: function (element, options) {
            // Set up Button listeners before hiding scenarios in case the button is removed when hiding scenario
            document.getElementById("buttonGetLedBlinkPattern").addEventListener("click", getLedBlinkPatternClick, false);
            document.getElementById("buttonSetLedBlinkPattern").addEventListener("click", setLedBlinkPatternClick, false);

            // We will disable the scenario that is not supported by the device.
            // If no devices are connected, none of the scenarios will be shown and an error will be displayed
            var deviceScenarios = {};
            deviceScenarios[SdkSample.Constants.deviceType.superMutt] = document.querySelector(".superMuttScenario");

            SdkSample.CustomHidDeviceAccess.utilities.setUpDeviceScenarios(deviceScenarios, document.querySelector(".deviceScenarioContainer"));
        }
    });

    function getLedBlinkPatternClick() {
        if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
            buttonGetLedBlinkPattern.disabled = true;

            featureReportsForSuperMutt.getLedBlinkPatternAsync().done(function () {
                buttonGetLedBlinkPattern.disabled = false;
            });
        } else {
            SdkSample.CustomHidDeviceAccess.utilities.notifyDeviceNotConnected();
        }
    }

    function setLedBlinkPatternClick() {
        if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
            buttonSetLedBlinkPattern.disabled = true;

            var pattern = ledBlinkPatternInput.selectedIndex;

            featureReportsForSuperMutt.setLedBlinkPatternAsync(pattern).done(function () {
                buttonSetLedBlinkPattern.disabled = false;
            });
        } else {
            SdkSample.CustomHidDeviceAccess.utilities.notifyDeviceNotConnected();
        }
    }



})();
