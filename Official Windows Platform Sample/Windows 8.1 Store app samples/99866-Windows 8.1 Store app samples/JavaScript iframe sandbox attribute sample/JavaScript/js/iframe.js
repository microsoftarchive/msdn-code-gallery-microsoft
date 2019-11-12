//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function iframeButton() {
        document.getElementById("output").innerText = "Random Number: " + Math.random();
        document.getElementById("outputdiv").style.visibility = "visible";
    }

    WinJS.Namespace.define("SdkSample", {
        iframeButton: iframeButton
    });
})();
