//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    /// <summary>
    /// Create a public namespace to hold these values.
    /// </summary>
    WinJS.Namespace.define("UsbCdcControl", {
        /// <summary>
        /// Define a class to hold entries in the list of devices.
        /// </summary>
        deviceListEntry: WinJS.Class.define(
            function (deviceInterface) {
                this.deviceInterface = deviceInterface;
                this.matched = true;
            },
            {
                id: { get: function () { return this.deviceInterface.id; } },
                instanceId: { get: function () { return this.deviceInterface.properties["System.Devices.DeviceInstanceId"]; } },
                name: { get: function () { return this.deviceInterface.name; } },
                device: { get: function () { return this.deviceInterface; } }
            },
            {
                findInList: function (id, devices) {
                    var match = null;
                    var index;
                    devices.forEach(function (e, i) { if (e.id === id) { match = e; index = i; } });
                    return match ? { key: index, data: match } : null;
                }
            }
        ),

        usbDeviceInfo: WinJS.Class.define(
            // Constructor
            function (info) {
                this.id = info.id;
                this.name = info.name;
            },
            // InstanceMembers
            {
                id: null,
                name: null
            },
            // StaticMembers
            {

            }
        ),

        deviceList: WinJS.Class.define(
            // Constructor
            function (deviceSelector) {
                this._deviceSelector = deviceSelector;

                // Register global handlers for suspend/resume to stop and restart the device watcher.
                // Ideally, one should close the device on app suspend and reopen the device on app resume because the API will close the device
                // for you if you don't explicitly close the device. Please see CustomUsbDeviceAccess sample for an example of how that should 
                // be done.
                Windows.UI.WebUI.WebUIApplication.addEventListener("suspending", this.onSuspend.bind(this), false);
                Windows.UI.WebUI.WebUIApplication.addEventListener("resuming", this.onResume.bind(this), false);

                // Initialize the device watcher
                this.initDeviceWatcher();

                // Add this to the static list.
                UsbCdcControl.deviceList.Instances.push(this);
            },
            // InstanceMembers
            {
                _Watcher: null,
                _Devices: new WinJS.Binding.List(),
                _onAddedCallback: null,
                _onRemovedCallback: null,
                _deviceSelector: null,

                // Currently selected device
                _Device: null,

                _switchEventHandler: null,

                // The list of  devices found by the PNP watcher
                Devices: { get: function () { return this._Devices; } },

                // Whether the  watcher is currently active. Retains its value across suspend/resume.
                watcherStarted: false,

                events: new (WinJS.Class.mix(WinJS.Class.define(null), WinJS.Utilities.eventMixin)),

                initDeviceWatcher: function () {
                    // Create a device watcher to look for instances of the  device interface
                    this._Watcher = Windows.Devices.Enumeration.DeviceInformation.createWatcher(
                                this._deviceSelector,
                                ["System.Devices.DeviceInstanceId"]
                                );

                    // Register to know when devices come and go, and when an enumeration ends
                    this._Watcher.addEventListener("added", this.onAdded.bind(this));
                    this._Watcher.addEventListener("removed", this.onRemoved.bind(this));
                    this._Watcher.addEventListener("enumerationcompleted", this.onEnumerationComplete.bind(this));
                },

                // Method to start the PNP watcher
                startWatcher: function (onAddedCallback, onRemovedCallback) {
                    this._onAddedCallback = onAddedCallback;
                    this._onRemovedCallback = onRemovedCallback;

                    WinJS.log && WinJS.log("starting device watcher", "sample", "info");
                    // Clear the matched property on each entry that's already in the list
                    if (!this.watcherStarted) {
                        this.watcherStarted = true;
                        this.resumeWatcher();
                    }
                },

                suspendWatcher: function () {
                    if (this.watcherStarted) {
                        this._Watcher.stop();
                    }
                },

                resumeWatcher: function () {
                    if (this.watcherStarted) {
                        // Clear the matched property on each entry that's already in the list
                        this._Devices.forEach(function (e) { e.matched = false; });
                        this._Watcher.start();
                    }
                },

                // Method to stop the PNP watcher
                stopWatcher: function () {
                    WinJS.log && WinJS.log("stopping  watcher", "sample", "info");
                    this.suspendWatcher();
                    this.watcherStarted = false;
                    this._onAddedCallback = null;
                    this._onRemovedCallback = null;
                },

                // Event handler for arrival of  devices.  If the device isn't already in the
                // list then it's added.  If it is already in the list, then the verified property
                // is set to true so that endDeviceListUpdate() knows the device is still present.
                onAdded: function (devInterface) {
                    WinJS.log && WinJS.log("onAdded: " + devInterface.id, "sample", "info");

                    // Search the device list for a device with a matching interface ID
                    var match = UsbCdcControl.deviceListEntry.findInList(devInterface.id, this._Devices);

                    // If we found a match then mark it as verified and return
                    if (match !== null) {
                        if (!match.data.matched && this._onAddedCallback !== null) {
                            this._onAddedCallback(new UsbCdcControl.usbDeviceInfo(devInterface));
                        }
                        match.data.matched = true;
                        return;
                    }

                    // Create a new element for this device interface, and queue up the query of its
                    // device information.
                    match = new UsbCdcControl.deviceListEntry(devInterface);

                    // Add the new element to the end of the list of devices
                    this._Devices.push(match);

                    if (this._onAddedCallback !== null) {
                        this._onAddedCallback(new UsbCdcControl.usbDeviceInfo(devInterface));
                    }
                },

                // Event handler for removal of an  device.  If the device is in the list, it clears
                // the verified property and then purges it from the list by calling endDeviceListUpdate().
                onRemoved: function (devInformation) {
                    var deviceId = devInformation.id;
                    WinJS.log && WinJS.log("onRemoved: " + deviceId, "sample", "info");

                    // Search the list of devices for one with a matching ID.
                    var match = UsbCdcControl.deviceListEntry.findInList(devInformation.id, this._Devices);
                    if (match !== null) {
                        // Remove the matched item
                        WinJS.log && WinJS.log("onRemoved: " + deviceId, "sample", "info");
                        this._Devices.splice(match.key, 1);

                        if (this._onRemovedCallback !== null) {
                            this._onRemovedCallback(new UsbCdcControl.usbDeviceInfo(match.data));
                        }
                    }
                },

                // Event handler for the end of the enumeration triggered when starting the watcher
                // This calls endDeviceListUpdate() to purge the device list of any devices which
                // are no longer present (their verified property is false).
                onEnumerationComplete: function (devInformation) {
                    // Iterate through the list of devices and remove any that hasn't been matched.
                    // removing the selected entry will automatically trigger its close.
                    var that = this;
                    this._Devices.forEach(
                        function (entry, index) {
                            if (!entry.matched) {
                                that._Devices.splice(index, 1);
                            }
                        }
                    );
                },

                // Event callback for suspend events.  Stops the device watcher
                onSuspend: function () {
                    // Stop the device watcher when we get suspended.  We will restart it on resume,
                    // which will trigger a new enumeration.
                    this.suspendWatcher();
                },

                // Event callback for resume events. Clears the verified property on all devices
                // then restarts the device watcher.
                // Notify the user that they need to reopen the device.
                onResume: function () {
                    this.resumeWatcher();

                    // Notify the user that they have to go back to scenario 1 to reconnect to the device.
                    WinJS.log("If device was connected on app suspension, then it was closed. Please go to scenario 1 to reconnect to the device.", null, "status");
                }
            },
            // StaticMembers
            {
                Instances: {
                    get: function () {
                        if (!this._instances) {
                            this._instances = [];
                        }
                        return this._instances;
                    }
                },
                _instances: null
            }
        ),
    });
})();
