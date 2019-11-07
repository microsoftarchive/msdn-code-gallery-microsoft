//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Scenario3_touch.html", {
        ready: function (element, options) {
            id("button1").addEventListener("click", getTouchCapabilities, false);
        }
    });

    function id(elementId) {  
        return document.getElementById(elementId);  
    }

    function getTouchCapabilities() {
        var touchCapabilities = new Windows.Devices.Input.TouchCapabilities();
        id("touchPresent").innerHTML = /*@static_cast(String)*/touchCapabilities.touchPresent;
        id("contacts").innerHTML = /*@static_cast(String)*/touchCapabilities.contacts; 
    }
})();
