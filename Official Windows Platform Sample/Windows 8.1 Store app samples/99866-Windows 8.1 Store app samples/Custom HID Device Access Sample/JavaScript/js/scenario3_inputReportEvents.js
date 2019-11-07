//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var InterruptEventsClass = WinJS.Class.define(null, {
        // Device that we registered for events with
        _registeredDevice: null,
        isRegisteredForInputReportEvents: false,
        numInputReportEventsReceived: 0,
        totalNumberBytesReceived: 0,
        // Did we navigate away from this page?
        navigatedAway: false,
        inputReportEventHandler: null,
        /// <summary>
        /// Tells the SuperMutt device to start creating interrupts. The app won't get any interrupts until after we register for the event.
        /// 
        /// We are using an output report to start the interrupts, please go to Scenario4_InputOutputReports for more information on how to use
        /// output reports.
        /// </summary>
        startSuperMuttInputReports: function () {
            var outputReport = SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.createOutputReport(SdkSample.Constants.superMutt.deviceInputReportControlInformation.reportId);

            var writer = new Windows.Storage.Streams.DataWriter();

            writer.writeByte(outputReport.id);
            writer.writeByte(SdkSample.Constants.superMutt.deviceInputReportControlInformation.dataTurnOn);

            outputReport.data = writer.detachBuffer();

            SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.sendOutputReportAsync(outputReport);
        },
        /// <summary>
        /// Tells the SuperMutt device to stop creating interrupts. The app will stop getting event raises unless we start the interrupts again.
        /// 
        /// We are using an output report to start the interrupts, please go to Scenario4_InputOutputReports for more information on how to use
        /// output reports.
        /// </summary>
        stopSuperMuttInputReports: function () {
            var outputReport = SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.createOutputReport(SdkSample.Constants.superMutt.deviceInputReportControlInformation.reportId);

            var writer = new Windows.Storage.Streams.DataWriter();

            writer.writeByte(outputReport.id);
            writer.writeByte(SdkSample.Constants.superMutt.deviceInputReportControlInformation.dataTurnOff);

            outputReport.data = writer.detachBuffer();

            SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device.sendOutputReportAsync(outputReport);
        },
        /// <summary>
        /// This callback only increments the total number of events received and prints it
        ///
        /// This method is called whenever the device's state changes and sends a report. Since all input reports share the same event in 
        /// HidDevice, the app needs to get the HidInputReport from eventArgs.Report and compare report ids and usages with the desired
        /// report.
        /// </summary>
        /// <param name="eventArgs">Contains the HidInputReport that caused the event to raise</param> 
        onInputReportEvent: function (eventArgs) {
            // If we navigated away from this page, we don't need to process this event
            // This also prevents output from spilling into another page
            if (!inputReportEvents.navigatedAway) {
                inputReportEvents.numInputReportEventsReceived++;

                // The data from the input report
                var inputReport = eventArgs.report;
                var buffer = inputReport.data;

                inputReportEvents.totalNumberBytesReceived += buffer.length;

                WinJS.log && WinJS.log(
                    "Total number of events received: " + inputReportEvents.numInputReportEventsReceived
                    + "\nTotal number of bytes received: " + inputReportEvents.totalNumberBytesReceived,
                    "sample", "status");
            }
        },
        /// <summary>
        /// Register for the event that is triggered when the device sends a report to us.
        /// 
        /// All input reports share the same event in HidDevice, so once we register the event, all HidInputReports (regardless of the report id) will
        /// raise the event. Read the comment above onInputReportEvent for more information on how to distinguish input reports.
        ///
        /// The function also saves the event handler so that we can unregister from the event later on.
        /// </summary>
        registerForInputReportEvents: function () {
            if (!this.isRegisteredForInputReportEvents) {
                this.inputReportEventHandler = this.onInputReportEvent;

                // Remember which device we are registering the device with, in case there is a device disconnect and reconnect. We want to avoid unregistering
                // an event handler that is not registered with the device object.
                // Ideally, one should remove the event token (e.g. assign to null) upon the device removal to avoid using it again.
                this._registeredDevice = SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device;

                this._registeredDevice.addEventListener("inputreportreceived", this.inputReportEventHandler, false);

                this.isRegisteredForInputReportEvents = true;

                this.numInputReportEventsReceived = 0;
                this.totalNumberBytesReceived = 0;
            }
        },
        /// <summary>
        /// Unregisters from the HidDevice's InputReportReceived event that was registered for in the RegisterForInputReportEvents();
        /// </summary>
        unregisterFromInputReportEvents: function () {
            if (this.isRegisteredForInputReportEvents) {
                // Don't unregister event if the device was removed and reconnected because the endpoint event no longer contains our event handler
                if (this._registeredDevice === SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.device) {
                    this._registeredDevice.removeEventListener("inputreportreceived", this.inputReportEventHandler, false);
                }

                this._registeredDevice = null;
                this.isRegisteredForInputReportEvents = false;
            }
        },
        _onDeviceClosing: function (deviceInformation) {
            inputReportEvents.unregisterFromInputReportEvents();

            updateRegisterEventButtons();
        },
    },
    null);

    var inputReportEvents = new InterruptEventsClass();

    var page = WinJS.UI.Pages.define("/html/scenario3_inputReportEvents.html", {
        ready: function (element, options) {
            inputReportEvents.navigatedAway = false;

            // Set up Button listeners before hiding scenarios in case the button is removed when hiding scenario
            document.getElementById("buttonRegisterForInputReportEvents").addEventListener("click", registerForInputReportEventsClick, false);
            document.getElementById("buttonUnregisterFromInputReportEvents").addEventListener("click", unregisterFromInputReportEventsClick, false);

            // We will disable the scenario that is not supported by the device.
            // If no devices are connected, none of the scenarios will be shown and an error will be displayed
            var deviceScenarios = {};
            deviceScenarios[SdkSample.Constants.deviceType.superMutt] = document.querySelector(".superMuttScenario");

            SdkSample.CustomHidDeviceAccess.utilities.setUpDeviceScenarios(deviceScenarios, document.querySelector(".deviceScenarioContainer"));

            // Get notified when the device is about to be closed so we can unregister events
            SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.onDeviceCloseCallback = inputReportEvents._onDeviceClosing;

            updateRegisterEventButtons();
        },
        unload: function () {
            inputReportEvents.navigatedAway = true;

            if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
                // Unregister events
                inputReportEvents.unregisterFromInputReportEvents();

                // Stop the interrupts on the SuperMutt device
                inputReportEvents.stopSuperMuttInputReports();
            }

            // We are leaving and no longer care about the device closing
            SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.onDeviceCloseCallback = null;
        }
    });

    function registerForInputReportEventsClick(eventArgs) {
        if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
            // Start the interrupts on the SuperMutt device (by default the device doesn't generate interrupts)
            inputReportEvents.startSuperMuttInputReports();

            inputReportEvents.registerForInputReportEvents();

            updateRegisterEventButtons();
        } else {
            SdkSample.CustomHidDeviceAccess.utilities.notifyDeviceNotConnected();
        }
    }

    function unregisterFromInputReportEventsClick(eventArgs) {
        if (SdkSample.CustomHidDeviceAccess.eventHandlerForDevice.current.isDeviceConnected) {
            inputReportEvents.unregisterFromInputReportEvents();

            inputReportEvents.stopSuperMuttInputReports();

            updateRegisterEventButtons();
        } else {
            SdkSample.CustomHidDeviceAccess.utilities.notifyDeviceNotConnected();
        }
    }

    function updateRegisterEventButtons() {
        if (window.buttonRegisterForInputReportEvents) {
            window.buttonRegisterForInputReportEvents.disabled = inputReportEvents.isRegisteredForInputReportEvents;
        }
        if (window.buttonUnregisterFromInputReportEvents) {
            window.buttonUnregisterFromInputReportEvents.disabled = !inputReportEvents.isRegisteredForInputReportEvents;
        }

    }
})();
