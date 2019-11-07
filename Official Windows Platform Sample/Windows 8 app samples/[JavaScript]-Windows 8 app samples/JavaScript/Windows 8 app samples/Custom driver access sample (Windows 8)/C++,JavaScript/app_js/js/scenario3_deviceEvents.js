//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Has the user enabled switch change events?
    var switchChangedEventsRegistered = false;

    // The previous switch values - saved to allow the switch changes to be
    // bolded.
    var previousSwitchValues = null;

    var page = WinJS.UI.Pages.define("/html/scenario3_deviceEvents.html", {
        ready: function (element, options) {
            document.getElementById("device-events-get").addEventListener("click", onGetSwitchState, false);
            document.getElementById("device-events-register").addEventListener("click", onChangeEventRegistration, false);
        },
        processed: function () {
            clearSwitchStateTable();
            updateRegisterButton();
            DeviceList.events.addEventListener("deviceclosing", onDeviceClose);
        },
        unload: function () {
            DeviceList.events.removeEventListener("deviceclosing", onDeviceClose);
            if (switchChangedEventsRegistered) {
                registerForSwitchStateChangedEvent(false);
            }
        }
    });

    function updateRegisterButton() {
        if (switchChangedEventsRegistered) {
            document.getElementById("device-events-register").innerText = "Unregister From Switch State Change Event";
        } else {
            document.getElementById("device-events-register").innerText = "Register For Switch State Change Event";
        }
    }

    function onDeviceClose() {
        if (switchChangedEventsRegistered) {
            registerForSwitchStateChangedEvent(false);
        }
    }

    // Gets the current switch state of the Fx2 Device dip switches
    function onGetSwitchState() {
        var switchStateArray;

        if (!DeviceList.fx2Device) {
            WinJS.log && WinJS.log("Fx2 device not connected or accessible", "sample", "error");
            return;
        }

        try {
            switchStateArray = DeviceList.fx2Device.switchState;
        } catch (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
            return;
        }

        updateSwitchStateTable(switchStateArray);
    }

    // Event handler for the register/unregister for switch change events UI button
    function onChangeEventRegistration() {
        if (!DeviceList.fx2Device) {
            WinJS.log && WinJS.log("Fx2 device not connected or accessible", "sample", "error");
            return;
        }

        registerForSwitchStateChangedEvent(!switchChangedEventsRegistered);
    }

    // registers or unregisters for switch change events.  Updates the
    // switchChangedEventsRegistered global if the event state was changed
    function registerForSwitchStateChangedEvent(register) {
        try {
            if (switchChangedEventsRegistered === false) {
                DeviceList.fx2Device.addEventListener("switchstatechanged", onSwitchStateChangedEvent);
            } else {
                DeviceList.fx2Device.removeEventListener("switchstatechanged", onSwitchStateChangedEvent);
            }

        } catch (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
            return;
        }

        switchChangedEventsRegistered = register;
        updateRegisterButton();
        clearSwitchStateTable();
    }

    // Event handler invoked when the switch state is changed on the FX2 device.
    function onSwitchStateChangedEvent(eventArgs) {
        var switchStateArray;

        try {
            switchStateArray = eventArgs.switchState;
        } catch (error) {
            WinJS.log && WinJS.log("Error accessing Fx2 device:\n" + error, "sample", "error");
            return;
        }

        if (document.getElementById("device-events-switches")) {
            updateSwitchStateTable(switchStateArray);
        }
    }

    function clearSwitchStateTable() {
        document.getElementById("device-events-switches").innerHTML = "";
    }

    function updateSwitchStateTable(switchStateArray) {

        var table = DeviceList.createBooleanTable(
                                           switchStateArray,
                                           previousSwitchValues,
                                           {
                                               indexTitle: "Switch Number",
                                               valueTitle: "Switch State",
                                               trueValue: "off",
                                               falseValue: "on"
                                           }
                                        );
        var output = document.getElementById("device-events-switches");
        if (output.children.length > 0) {
            output.removeChild(output.children[0]);
        }
        output.insertBefore(table);

        previousSwitchValues = switchStateArray;
    }
})();
