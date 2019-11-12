//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_deviceProperties.html", {
        ready: function (element, options) {
            document.getElementById("device-properties-get").addEventListener("click", onGetSegmentValue, false);
            document.getElementById("device-properties-set").addEventListener("click", onSetSegmentValue, false);
        }
    });

    function onGetSegmentValue() {

        if (!DeviceList.fx2Device) {
            WinJS.log && WinJS.log("Fx2 device not connected or accessible", "sample", "error");
            return;
        }
        try {
            var segment = DeviceList.fx2Device.sevenSegmentDisplay;
        }
        catch (error) {
            var invalidParameterError = -2147024809; // 0x80070057
            if (error.number !== invalidParameterError) {
                WinJS.log && WinJS.log(error, "sample", "error");
                return;
            } else {
                segment = 255;
            }
        }

        if (segment === 255) {
            WinJS.log && WinJS.log("The segment display value is not yet initialized.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("The segment display value is " + segment, "sample", "status");
        }
    }

    // Sets the segment display value on the Fx2 Device
    function onSetSegmentValue() {

        if (!DeviceList.fx2Device) {
            WinJS.log && WinJS.log("Fx2 device not connected or accessible", "sample", "error");
            return;
        }
        var segmentSelector = document.getElementById("device-properties-segmentInput");
        // Get the selected value to be set on the device
        var val = segmentSelector.options[segmentSelector.selectedIndex].value;
        try {
            DeviceList.fx2Device.sevenSegmentDisplay = val;
            WinJS.log && WinJS.log("The segment display is set to " + val, "sample", "status");
        }
        catch (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
            return;
        }
    }

})();
