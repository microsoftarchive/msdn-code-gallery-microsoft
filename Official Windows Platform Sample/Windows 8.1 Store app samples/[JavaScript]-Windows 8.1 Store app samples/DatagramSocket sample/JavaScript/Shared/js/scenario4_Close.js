//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario4_Close.html", {
        ready: function (element, options) {
            document.getElementById("buttonClose").addEventListener("click", closeListenerAndSockets, false);
        }
    });

    function closeListenerAndSockets() {
        socketsSample.close();
        socketsSample.hostNameConnect = null;
        WinJS.log && WinJS.log("Client and server closed.", "sample", "status");
    }
})();
