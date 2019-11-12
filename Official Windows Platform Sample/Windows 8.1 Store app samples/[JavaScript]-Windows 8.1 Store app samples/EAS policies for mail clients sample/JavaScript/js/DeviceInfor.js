//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/DeviceInfor.html", {
        ready: function (element, options) {
            document.getElementById("DeviceInforLaunch").addEventListener("click", deviceInfor, false);
            document.getElementById("DeviceInforReset").addEventListener("click", deviceInforReset, false);
        }
    });


    function deviceInfor() {

        var currentDeviceInfor = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();

        document.getElementById("DeviceID").value = currentDeviceInfor.id;
        document.getElementById("OperatingSystem").value = currentDeviceInfor.operatingSystem;
        document.getElementById("FriendlyName").value = currentDeviceInfor.friendlyName;
        document.getElementById("Type").value = currentDeviceInfor.type;
        document.getElementById("SystemManufacturer").value = currentDeviceInfor.systemManufacturer;
        document.getElementById("SystemProductName").value = currentDeviceInfor.systemProductName;
        //  document.getElementById("SystemSku").value = CurrentDeviceInfor.systemSku;
    }

    function deviceInforReset() {
        document.getElementById("DeviceID").value = "";
        document.getElementById("OperatingSystem").value = "";
        document.getElementById("FriendlyName").value = "";
        document.getElementById("Type").value = "";
        document.getElementById("SystemManufacturer").value = "";
        document.getElementById("SystemProductName").value = "";
        //  document.getElementById("SystemSku").value = "";
    }
    function doSomething1() {
        WinJS.log && WinJS.log("Error message here", "sample", "error");
    }

    function doSomething2() {
        WinJS.log && WinJS.log("Show status here", "sample", "status");
    }
})();
