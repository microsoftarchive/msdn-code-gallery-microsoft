//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/custom-exception.html", {
        ready: function (element, options) {
            document.getElementById("customExceptionStartButton").addEventListener("click", customExceptionRun, false);
        }
    });

    function customExceptionRun() {
        var SampleNamespace = Microsoft.SDKSamples.Kitchen;;
        var myOven = new SampleNamespace.Oven();

        try {
            // Intentionally pass an invalid value
            myOven.configurePreheatTemperature(5);
        }
        catch (e) {
            printLn("Exception caught. Please attach a debugger and enable first chance native exceptions to view exception details.");
        }
    }

    function printLn(str) {
        document.getElementById("customExceptionOutput").innerHTML += str + "<br>";
    }
})();
