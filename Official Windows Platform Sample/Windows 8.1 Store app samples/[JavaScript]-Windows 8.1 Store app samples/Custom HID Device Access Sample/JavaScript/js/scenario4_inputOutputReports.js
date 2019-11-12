//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var InputOutputReportsClass = WinJS.Class.define(null, {
        /// <summary>
        /// Uses an input report to get the value that was previously written to the device with output report.
        /// 
        /// We will be using controls to modify the data buffer. See SendNumericOutputReportAsync for how to modify the data buffer directly.
        ///
        /// Any errors in async function will be passed down the task chain and will not be caught here because errors should be handled 
        /// at the end of the task chain.
        /// </summary>
        /// <returns>A promise that can be used to chain more methods after completing the scenario</returns>
        getNumericInputReportAsync: function () {
            return SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.getInputReportAsync(SdkSample.Constants.superMutt.readWriteBufferControlInformation.reportId)
                .then(function (inputReport) {
                    var inputReportControl = inputReport.getNumericControl(
                        SdkSample.Constants.superMutt.readWriteBufferControlInformation.volumeUsagePage,
                        SdkSample.Constants.superMutt.readWriteBufferControlInformation.volumeUsageId);

                    var data = inputReportControl.value;
                    var scaledData = inputReportControl.scaledValue;

                    WinJS.log && WinJS.log("Value read: " + data, "sample", "status");
                });
        },
        /// <summary>
        /// Uses an output report to write data to the device so that it can be read back using input report. 
        /// 
        /// Please note that when we create an OutputReport, all data is nulled out in the report. Since we will only be modifying 
        /// data we care about, the other bits that we don't care about, will be zeroed out. Controls will effectively do the same thing (
        /// all bits are zeroed out except for the bits we care about).
        ///
        /// We will modify the data buffer directly. See GetNumericInputReportAsync for how to use controls.
        ///
        /// Any errors in async function will be passed down the task chain and will not be caught here because errors should be handled 
        /// at the end of the task chain.
        /// </summary>
        /// <param name="valueToWrite"></param>
        /// <returns>A promise that can be used to chain more methods after completing the scenario</returns>
        sendNumericOutputReportAsync: function (valueToWrite) {
            var outputReport = SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.createOutputReport(SdkSample.Constants.superMutt.readWriteBufferControlInformation.reportId);

            // Our data is supposed to be stored in the upper 4 bits of the byte
            var byteToWrite = valueToWrite << SdkSample.Constants.superMutt.readWriteBufferControlInformation.volumeDataStartBit;

            var dataWriter = new Windows.Storage.Streams.DataWriter();
            dataWriter.byteOrder = Windows.Storage.Streams.ByteOrder.littleEndian;

            dataWriter.writeByte(outputReport.id);
            dataWriter.writeByte(byteToWrite);

            outputReport.data = dataWriter.detachBuffer();

            return SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.sendOutputReportAsync(outputReport).then(function (bytesWritten) {
                WinJS.log && WinJS.log("Value of Byte written: " + valueToWrite, "sample", "status");
            });
        },
        /// <summary>
        /// Uses an input report to get the value that was previously written to the device with output report. 
        /// 
        /// We will be using controls to modify the data buffer. See SendBooleanOutputReportAsync for how to modify the data buffer directly.
        ///
        /// Any errors in async function will be passed down the task chain and will not be caught here because errors should be handled 
        /// at the end of the task chain.
        /// </summary>
        /// <param name="buttonNumber">Button to get the value of (1-4)</param>
        /// <returns>A promise that can be used to chain more methods after completing the scenario</returns>
        getBooleanInputReportAsync: function (buttonNumber) {
            return SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.getInputReportAsync(SdkSample.Constants.superMutt.readWriteBufferControlInformation.reportId)
                .then(function (inputReport) {
                    var inputReportControl = inputReport.getBooleanControl(
                        SdkSample.Constants.superMutt.readWriteBufferControlInformation.buttonUsagePage,
                        SdkSample.Constants.superMutt.readWriteBufferControlInformation.buttonUsageId[buttonNumber - 1]);

                    var data = inputReportControl.isActive ? "1" : "0";

                    WinJS.log && WinJS.log("Bit value: " + data, "sample", "status");
                });
        },
        /// <summary>
        /// Uses an output report to write data to the device so that it can be read back using input report. 
        /// 
        /// Please note that when we create an OutputReport, all data is nulled out in the report. Since we will only be modifying 
        /// data we care about, the other bits that we don't care about, will be zeroed out. Controls will effectively do the same thing (
        /// all bits are zeroed out except for the bits we care about).
        ///
        /// We will modify the data buffer directly. See GetBooleanInputReportAsync for how to use controls.
        ///
        /// Any errors in async function will be passed down the task chain and will not be caught here because errors should be handled 
        /// at the end of the task chain.
        /// </summary>
        /// <param name="bitValue"></param>
        /// <param name="buttonNumber">Button to change value of (1-4)</param>
        /// <returns>A promise that can be used to chain more methods after completing the scenario</returns>
        sendBooleanOutputReportAsync: function (bitValue, buttonNumber) {
            var buttonIndex = buttonNumber - 1;

            var outputReport = SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.createOutputReport(SdkSample.Constants.superMutt.readWriteBufferControlInformation.reportId);

            var bitNumericValue = bitValue ? 0xFF : 0x00; // Used to do bit wise operations with

            var valueAfterSettingBit = bitNumericValue & SdkSample.Constants.superMutt.readWriteBufferControlInformation.buttonDataMask[buttonIndex];

            var dataWriter = new Windows.Storage.Streams.DataWriter();
            dataWriter.byteOrder = Windows.Storage.Streams.ByteOrder.littleEndian;

            dataWriter.writeByte(outputReport.id);
            dataWriter.writeByte(valueAfterSettingBit);

            outputReport.data = dataWriter.detachBuffer();

            return SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.sendOutputReportAsync(outputReport).then(function (bytesWritten) {
                var valueWritten = bitValue ? "1" : "0";
                WinJS.log && WinJS.log("Bit value written: " + valueWritten, "sample", "status");
            });
        }
    },
    null);

    var inputOutputReports = new InputOutputReportsClass();

    var page = WinJS.UI.Pages.define("/html/scenario4_inputOutputReports.html", {
        ready: function (element, options) {
            // Set up Button listeners before hiding scenarios in case the button is removed when hiding scenario
            document.getElementById("buttonSendNumericOutputReport").addEventListener("click", sendNumericInputReportClick, false);
            document.getElementById("buttonGetNumericInputReport").addEventListener("click", getNumericInputReportClick, false);
            document.getElementById("buttonSendBooleanOutputReport").addEventListener("click", sendBooleanInputReportClick, false);
            document.getElementById("buttonGetBooleanInputReport").addEventListener("click", getBooleanInputReportClick, false);

            // We will disable the scenario that is not supported by the device.
            // If no devices are connected, none of the scenarios will be shown and an error will be displayed
            var deviceScenarios = {};
            deviceScenarios[SdkSample.Constants.deviceType.superMutt] = document.querySelector(".superMuttScenario");

            SdkSample.CustomHidDeviceAccess.utilities.setUpDeviceScenarios(deviceScenarios, document.querySelector(".deviceScenarioContainer"));
        }
    });

    function getNumericInputReportClick(eventArgs) {
        if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
            // Don't allow multiple reads at the same time
            window.buttonGetNumericInputReport.disabled = true;

            inputOutputReports.getNumericInputReportAsync().done(function () {
                window.buttonGetNumericInputReport.disabled = false;
            });
        } else {
            SdkSample.CustomHidDeviceAccess.utilities.notifyDeviceNotConnected();
        }
    } 

    function sendNumericInputReportClick(eventArgs) {
        if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
            window.buttonSendNumericOutputReport.disabled = true;

            var valueToWrite = window.numericValueToWrite.selectedIndex;

            inputOutputReports.sendNumericOutputReportAsync(valueToWrite).done(function () {
                window.buttonSendNumericOutputReport.disabled = false;
            });
        } else {
            SdkSample.CustomHidDeviceAccess.utilities.notifyDeviceNotConnected();
        }
    }

    function getBooleanInputReportClick(eventArgs) {
        if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
            // Don't allow multiple reads at the same time
            window.buttonGetBooleanInputReport.disabled = true;

            // The sample will only demonstrate how to use the first button because it is the same for other buttons
            inputOutputReports.getBooleanInputReportAsync(1).done(function () {
                window.buttonGetBooleanInputReport.disabled = false;
            });
        } else {
            SdkSample.CustomHidDeviceAccess.utilities.notifyDeviceNotConnected();
        }
    }

    function sendBooleanInputReportClick(eventArgs) {
        if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
            window.buttonSendBooleanOutputReport.disabled = true;

            var valueToWrite = window.booleanValueToWrite.selectedIndex === 1 ? true : false;

            // The sample will only demonstrate how to use the first button because it is the same for other buttons
            inputOutputReports.sendBooleanOutputReportAsync(valueToWrite, 1).done(function () {
                window.buttonSendBooleanOutputReport.disabled = false;
            });
        } else {
            SdkSample.CustomHidDeviceAccess.utilities.notifyDeviceNotConnected();
        }
    }
})();
