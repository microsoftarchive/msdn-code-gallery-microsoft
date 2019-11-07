//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var c_NumberOfBars = 8;

    var page = WinJS.UI.Pages.define("/html/scenario4_deviceAsync.html", {
        ready: function (element, options) {
            document.getElementById("device-async-set").addEventListener("click", onSetBarGraph, false);
            document.getElementById("device-async-get").addEventListener("click", onGetBarGraph, false);
        }
    });


    // Set the bar graph on the device based on user input
    function onSetBarGraph() {
        if (!DeviceList.fx2Device) {
            WinJS.log && WinJS.log("Fx2 device not connected or accessible", "sample", "error");
            return;
        }
        // Get the selector element
        var barGraphSelector = document.getElementById("barGraphInput");
        // Get the selected value
        var val = barGraphSelector.options[barGraphSelector.selectedIndex].value;
        var barGraphArray = new Array();
        for (var i = 0; i < c_NumberOfBars; i++) {
            barGraphArray[i] = i < val;
        }

        try {
            DeviceList.fx2Device.setBarGraphDisplay(barGraphArray);
        } catch (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
            return;
        }
        document.getElementById("device-async-bars").innerHTML = "";
        WinJS.log && WinJS.log("Bar Graph Set to " + val, "sample", "status");
    }

    // Get the bar graph from the device
    function onGetBarGraph() {

        if (!DeviceList.fx2Device) {
            WinJS.log && WinJS.log("Fx2 device not connected or accessible", "sample", "error");
            return;
        }

        try {
            WinJS.log && WinJS.log("Getting Fx2 bars", "sample", "status");
            DeviceList.fx2Device.getBarGraphDisplayAsync().
            done(
                function (result) {
                    WinJS.log && WinJS.log("Got bars value", "sample", "status");
                    var barGraphArray = result.barGraphDisplay;
                    updateBarGraphTable(barGraphArray);
                },
                function (error) {
                    WinJS.log && WinJS.log("Error getting bars value: " + error, "sample", "error");
                }
            );
        } catch (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
        }
    }

    function updateBarGraphTable(barGraphArray) {
        var output = document.getElementById("device-async-bars");

        if (!output) {
            // the control isn't currently visible.  Need to check
            // this because the bar retrieval is asynchronous
            return;
        }

        output.innerHTML = "";

        var table = DeviceList.createBooleanTable(
                                                   barGraphArray,
                                                   null,
                                                   {
                                                       indexTitle: "Bar Number",
                                                       valueTitle: "Bar State",
                                                       trueValue: "Lit",
                                                       falseValue: "Not Lit"
                                                   });

        output.insertBefore(table);
    }

})();
