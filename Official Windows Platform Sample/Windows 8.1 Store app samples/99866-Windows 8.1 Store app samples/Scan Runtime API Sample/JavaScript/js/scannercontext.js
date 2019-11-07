//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var scannerWatcher;
    var scannerList = new WinJS.Binding.List();
    var currentScannerId = null;
    
    //
    // Define a class to hold entries in the list of scanner device information.
    //
    var ScannerListEntry = WinJS.Class.define(
        function (deviceinformation) {
            this.deviceinformation = deviceinformation;
            this.matched = true;
        },
        {
            // The scanner device id
            id: { get: function () { return this.deviceinformation.id; } },
            // The scanner name 
            name: { get: function () { return this.deviceinformation.name; } },
        },
        {
            findInList: function (id) {
                var match = null;
                var index;
                scannerList.forEach(function (e, i) { if (e.id === id) { match = e; index = i; } });
                return match ? { key: index, data: match } : null;
            }
        }
    );

    //
    // Create a public namespace to hold scanner context specific values.
    //
    WinJS.Namespace.define("ScannerContext", {
        // Class for entries held in the list of scanner devices
        ScannerListEntry: ScannerListEntry,

        // The list of Scanner devices found by the scanner watcher
        scannerList: {
            get: function (){
                return scannerList;
            }
        },

        // The projection for the currently selected Scanner device
        currentScannerId: {
            get: function () {
                return currentScannerId;
            },
            set: function (value) {
                currentScannerId = value;                
                return value;
            }
        },

        // Whether the scanner watcher is currently active.  Retains its 
        // value across suspend/resume
        watcherStarted: false,

        // Method to start the scanner watcher
        startScannerWatcher: startScannerWatcher,

        // Method to stop the scanner watcher
        stopScannerWatcher: stopScannerWatcher,

        //  Whether the scanner watcher is currently active or suspended
        watcherSuspended: false,

        events: new (WinJS.Class.mix(WinJS.Class.define(null), WinJS.Utilities.eventMixin))
    });

    /// <summary>
    /// Initializes the watcher which is used for enumerating scanners
    /// </summary>
    function initDeviceWatcher() {       

        // Create a device watcher to enumerate available image scanner devices
        scannerWatcher = Windows.Devices.Enumeration.DeviceInformation.createWatcher(Windows.Devices.Enumeration.DeviceClass.imageScanner);

        // Register to know when devices are added or removed, and when the enumeration ends
        scannerWatcher.addEventListener("added", onScannerAdded);
        scannerWatcher.addEventListener("removed", onScannerRemoved);
        scannerWatcher.addEventListener("enumerationcompleted", onScannerEnumerationComplete);
    }

    /// <summary>
    /// Start Watcher for scanner devices
    /// </summary>
    function startScannerWatcher() {
        WinJS.log && WinJS.log("starting scanner watcher", "sample", "info");
        if (!ScannerContext.watcherStarted) {
            ScannerContext.watcherStarted = true;
            // Clear the matched property on each entry that's already in the list
            scannerList.forEach(function (e) { e.matched = false; });
            scannerWatcher.start();
        }
    }

    /// <summary>
    /// Stop Watcher for scanner devices
    /// </summary>
    function stopScannerWatcher() {
        WinJS.log && WinJS.log("stopping scanner watcher", "sample", "info");        
        ScannerContext.watcherStarted = false;
        // Remove all elements from the scanner device information list
        scannerList.splice(0, scannerList.length);
        scannerWatcher.stop();
    }

    /// <summary>
    /// Event handler for addition of scanner devices.  If the scanner isn't already in the
    /// list then it's added.  If it is already in the list, then the verified property
    /// is set to true so that onScannerEnumerationComplete() knows the device is still present.
    /// </summary>
    /// <param name="deviceInfo">The device info for the device which was added</param>
    function onScannerAdded(deviceInfo) {
        WinJS.log && WinJS.log("Scanner with device id " + deviceInfo.Id + " has been added", "sample", "info");

        // Search the scanner list for a scanner with a matching device id
        var matchEntry = ScannerListEntry.findInList(deviceInfo.id);

        // If we found a match then mark it as verified and return
        if (matchEntry !== null) {
            matchEntry.data.matched = true;
            return;
        }
        addToList(deviceInfo);
    }

    /// <summary>
    /// Event handler for removal of an scanner device.  If the device is in the list, it is removed from the list.
    /// </summary>
    /// <param name="deviceInfo">The device info for the device which was added</param>
    function onScannerRemoved(devInformation) {
        var deviceId = devInformation.id;
        WinJS.log && WinJS.log("Scanner with device id " + deviceId + " has been removed", "sample", "info");

        // Search the scanner list for a scanner with a matching device id
        var matchEntry = ScannerListEntry.findInList(devInformation.id);
        if (matchEntry !== null) {
            // remove the matched item
            WinJS.log && WinJS.log("Scanner with device id " + deviceId + " has been removed from list", "sample", "info");
            
            removeFromList(matchEntry.key);
        }
    }

    /// <summary>
    /// Event handler for the end of the enumeration triggered when starting the watcher
    /// This function purges the scanner list of any scanners which
    /// are no longer present 
    /// </summary>
    /// <param name="devInformation">The device info for the device which was added</param>
    function onScannerEnumerationComplete(devInformation) {
        WinJS.log && WinJS.log("Enumeration of scanners is complete", "sample", "info");
        // Iterate through the list of devices and remove any that hasn't been matched.
        scannerList.forEach(
            function (entry, index) {
                if (!entry.matched) {
                    removeFromList(index);
                }
            }
        );
    }

    /// <summary>
    /// Appends a ScannerListEntry to the list with the given device information
    /// </summary>
    /// <param name="scannerDeviceInfo">device information of the scanner that is to be added</param>
    function addToList(scannerDeviceInfo) {
        // Add the new element to the end of the list of devices
        var entry = new ScannerListEntry(scannerDeviceInfo);
        scannerList.push(entry);
        if (!ScannerContext.currentScannerId) {
            updateCurrentScannerId(scannerDeviceInfo.id);
        }
    }

    /// <summary>
    /// Removes a ScannerListEntry at the index of the scannerList, and updated the current scanner id is if
    /// the scanner device that is removed the current selected scanner.
    /// </summary>
    function removeFromList(index) {
        var id = scannerList.getAt(0).id;
        scannerList.splice(index, 1);
        // Remove the entry from the list
        if (id === ScannerContext.currentScannerId) {
            var scannerId = (scannerList.length === 0) ? null : scannerList.getAt(0);
            updateCurrentScannerId(scannerId);
        }
    }

    /// <summary>
    /// Updated the current scanner id and sends scanner id changed event
    /// </summary>
    function updateCurrentScannerId(scannerId) {
        ScannerContext.currentScannerId = scannerId;
        ScannerContext.events.dispatchEvent("currentScannerId");
    }

    /// <summary>
    /// Event handler to suspend watcher when application is suspended
    /// </summary>
    function onSuspend(){
        if (ScannerContext.watcherStarted) {
            stopScannerWatcher();
            ScannerContext.watcherSuspended = true;
        }
       
    }

    /// <summary>
    /// Event handler to resume watcher when application is started
    /// </summary>
    function onResume() {
        if (ScannerContext.watcherSuspended) {
            startScannerWatcher();
            ScannerContext.watcherSuspended = false;
        }
    }

    // Register global handlers for suspend/resume to stop and restart the device
    // watcher.
    Windows.UI.WebUI.WebUIApplication.addEventListener("suspending", onSuspend, false);
    Windows.UI.WebUI.WebUIApplication.addEventListener("resuming", onResume, false);

    // Initialize the device watcher
    initDeviceWatcher();
})();

(function () {
    // A custom WinJS control for selection of scanner from the list
    var ScannerPicker = WinJS.Class.define(function (element, options) {
        this.element = element;
        element.winControl = this;

        WinJS.UI.setOptions(this, options);

        var that = this;
        
        var list = ScannerContext.scannerList;
        
        // Populate the options the items are already present in the scanner list 
        list.forEach(function (scannerListEntry) {
            that.addScannerToList(scannerListEntry);
        });
        // Selecting the current scanner ID present in scanner context
        that.element.value = ScannerContext.currentScannerId;
        // Registering event handler of selection changed by the user
        that.element.addEventListener("change", that.selectionChange, false);
        // Registering event handler for current scanner id change event.
        ScannerContext.events.addEventListener("currentScannerId", that.currentScannerChange.bind(that), false);
        // Registering event handlers for events of interstation and removal of items from the scanner list
        list.addEventListener("iteminserted", that.listItemAdded.bind(that));
        list.addEventListener("itemremoved", that.listItemRemoved.bind(that));
        // Disable combo-box if there are not scanner present.
        if (list.length === 0) {
            that.element.disabled = true;
        }
    }, {
        // Adds a new scanner to the selection combo-box
        // watcher.
        addScannerToList: function (scannerListEntry) {
            var that = this;
            var value = scannerListEntry.id;
            var text = scannerListEntry.name;
            that.addOption(text, value, false, false);
        },
        // Adds a new option to the selection combo-box
        // watcher.
        addOption: function (text, value, disabled, selected) {
            var that = this;
            var option = document.createElement('option');
            option.value = value;
            option.selected = selected || false;
            option.disabled = disabled || false;
            option.textContent = text;
            that.element.appendChild(option);
        },
        // Event handler for selection change event
        // watcher.
        selectionChange: function () {
            ScannerContext.currentScannerId = this.value;
        },
        // Event handler for current scanner being changed by the scanner context
        // watcher.
        currentScannerChange: function () {
            var that = this;
            that.element.value = ScannerContext.currentScannerId;
        },
        // Event handler for Scanner item being added to the list
        listItemAdded: function (eventInfo) {
            var that = this;
            var scannerListEntry = eventInfo.detail.value;
            that.addScannerToList(scannerListEntry);
            // Always enabled combo-box when scanner is added
            this.element.disabled = false;
        },
        // Event handler for Scanner item being removed from the list
        listItemRemoved: function (eventInfo) {
            var that = this;
            var scannerListEntry = eventInfo.detail.value;
            var id = scannerListEntry.id;            
            var match = null;

            var options = this.element.options;
            for (var i = 0 ; i < options.length ; i++) {
                if (options[i].value === id) {
                    match = options[i];
                    break;
                }
            }
            if (match) {
                that.element.removeChild(match);
            }
            // Disable combo-box if there are not scanner present.
            if (options.length === 0) {
                that.element.disabled  = true;
            }
        },
        unLoad: function () {
            var that = this;
            var list = ScannerContext.scannerList;            
            // Unregistering event handlers for events of interstation and removal of items from the scanner list
            list.removeEventListener("iteminserted", that.listItemAdded.bind(that));
            list.removeEventListener("itemremoved", that.listItemRemoved.bind(that));
            // Unregistering event handler for change current scanner id
            ScannerContext.events.removeEventListener("currentScannerId", that.currentScannerChange.bind(that), false);
            // Registering event handler of selection changed by the user
            that.element.removeEventListener("change", that.selectionChange, false);

            that.element.winControl = null;
            that.element = null;
        }
    });

    WinJS.Namespace.define("SDKSample", {
        ScannerPicker: ScannerPicker
    });
})();