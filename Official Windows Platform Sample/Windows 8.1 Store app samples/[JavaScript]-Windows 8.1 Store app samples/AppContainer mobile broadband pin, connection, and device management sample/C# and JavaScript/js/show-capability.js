//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/show-capability.html", {
        ready: function (element, options) {
            getDeviceCapability();
        }
    }); 

    function getDeviceCapability() {
        try {
            // Create the pin wrapper
            var pinWrapper = createPinWrapper();

            // Get the device capalilities
            pinWrapper.getDeviceCapability();
        } catch (e) {
            mbComApiUtil.parseExceptionCodeAndPrint(e);
        }
    }

    function createPinWrapper() {
        var pinWrapper;

        // Create the pin wrapper
        pinWrapper = new MbnComWrapper.PinWrapper(function (msg) {
            WinJS.log && WinJS.log(msg, "sample", "status");
        });

        return pinWrapper;
    }
})();
