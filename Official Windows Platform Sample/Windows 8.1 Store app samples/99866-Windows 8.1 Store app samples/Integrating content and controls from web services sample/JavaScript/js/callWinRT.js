//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    function initialize() {
        var isWebContext = (document.location.protocol === "ms-appx-web:");
        if (isWebContext) {
            document.getElementById("description").innerText ="Page loaded in the Web Context (\"ms-appx-web:///callWinRT.html\"), it will fail to call any WinRT api";
        } else {
            document.getElementById("description").innerText ="Page loaded in the Local Context (\"ms-appx:///callWinRT.html\"), it will be able to call any WinRT api";
        }

        document.getElementById("showPackageInfoButton").addEventListener("click", showPackageInfo, false);
    }

    function showPackageInfo() {
        var info = "";
        try {
            info += "Windows.ApplicationModel.Package.current.id.name = ";
            info += Windows.ApplicationModel.Package.current.id.name;
            info += "<br/><br/>";

            info += "Windows.ApplicationModel.Package.current.id.publisher = ";
            info += Windows.ApplicationModel.Package.current.id.publisher;
        } catch (e) {
            // Expected in the web context
            info = "Js error thrown = " + e;
        }
        document.getElementById("status").innerHTML = info;
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
})();
