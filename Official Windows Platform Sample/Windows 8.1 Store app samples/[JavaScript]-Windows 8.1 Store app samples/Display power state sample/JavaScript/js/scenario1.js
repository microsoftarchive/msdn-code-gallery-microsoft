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
    
    var longMax = 2147483647;
    var appDisplayRequest = null;
    var displayRequestRefCount = 0;

    function activateRequest() {

        if (appDisplayRequest === null) {
        
            // This call creates an instance of the displayRequest object
            appDisplayRequest = new Windows.System.Display.DisplayRequest;
        }

        // This call activates a display-required request. If successful, 
        // the screen is guaranteed not to turn off automatically due to user inactivity.		
        if (displayRequestRefCount < longMax)
        {
            appDisplayRequest.requestActive();
            displayRequestRefCount++;
            WinJS.log && WinJS.log("Display request activated (" + displayRequestRefCount +")", "sample", "status");
        } else {
            WinJS.log && WinJS.log("Error: Exceeded maximum display request active instant count (" + displayRequestRefCount +")", "sample", "error");
        }
    }

    function releaseRequest() {     
        
        if (appDisplayRequest && displayRequestRefCount > 0) {
        
            // This call de-activates the display-required request. If successful, the screen
            // might be turned off automatically due to a user inactivity, depending on the
            // power policy settings of the system. The requestRelease method throws an exception 
            // if it is called before a successful requestActive call on this object.
            appDisplayRequest.requestRelease();
            displayRequestRefCount--;
            WinJS.log && WinJS.log("Display request released (" + displayRequestRefCount +")", "sample", "status");
        } else {
            WinJS.log && WinJS.log("No existing active display request instance to be released", "sample", "error");            
        }
    }
})();
