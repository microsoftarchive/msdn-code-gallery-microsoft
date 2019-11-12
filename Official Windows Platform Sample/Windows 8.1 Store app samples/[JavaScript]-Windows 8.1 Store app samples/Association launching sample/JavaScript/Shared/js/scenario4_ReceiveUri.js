//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4_ReceiveUri.html", {
        processed: function (element, uri) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Uri is the parameter passed to navigation in the activated event handler.
            if (uri) {
                WinJS.log && WinJS.log("Protocol activation received. The received URI is " + uri.rawUri + ".", "sample", "status");
            }

            // Detect whether the platform is Windows Phone and show the appropriate scenario description.
            // The platform detection is done by checking the desiredRemainingView property in an object of Windows.System.LauncherOptions. 
            // It is not available on Windows Phone platform.
            var options = new Windows.System.LauncherOptions();
            if ('desiredRemainingView' in options) {
                phoneScenarioDescription.hidden = true;
            }
            else {
                windowsScenarioDescription.hidden = true;
            }
        }
    });
})();
