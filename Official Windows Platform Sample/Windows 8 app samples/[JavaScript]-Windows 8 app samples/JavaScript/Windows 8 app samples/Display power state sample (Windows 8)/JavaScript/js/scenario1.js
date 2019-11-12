//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", activateRequest, false);
            document.getElementById("button2").addEventListener("click", releaseRequest, false);
        }
    });

    var g_dispRequest = null;
    var drCount = 0;

    function activateRequest() {

        if (g_dispRequest === null) {
            try {
                // This call creates an instance of the displayRequest object
                g_dispRequest = new Windows.System.Display.DisplayRequest;
            } catch (e) {
                WinJS.log && WinJS.log("Failed: displayRequest object creation, error: " + e.message, "sample", "error");
            }
        }

        if (g_dispRequest) {
            try {
                // This call activates a display-required request. If successful, 
                // the screen is guaranteed not to turn off automatically due to user inactivity.
                g_dispRequest.requestActive();
                drCount += 1;
                WinJS.log && WinJS.log("Display request activated (" + drCount +")", "sample", "status");
            } catch (e) {
                WinJS.log && WinJS.log("Failed: displayRequest.requestActive, error: " + e.message, "sample", "error");
            }
        }
    }

    function releaseRequest() {
        
        
        if (g_dispRequest) {
            try {
                // This call de-activates the display-required request. If successful, the screen
                // might be turned off automatically due to a user inactivity, depending on the
                // power policy settings of the system. The requestRelease method throws an exception 
                // if it is called before a successful requestActive call on this object.
                g_dispRequest.requestRelease();
                drCount -= 1;
                WinJS.log && WinJS.log("Display request released (" + drCount +")", "sample", "status");
            }  catch (e) {
                WinJS.log && WinJS.log("Failed: displayRequest.requestRelease, error: " + e.message, "sample", "error");
            }
        }
    }
})();
