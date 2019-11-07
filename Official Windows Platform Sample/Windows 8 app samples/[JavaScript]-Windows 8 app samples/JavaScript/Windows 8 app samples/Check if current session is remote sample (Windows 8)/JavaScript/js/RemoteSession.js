//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/RemoteSession.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", checkIsRemote, false);
        }
    });

    function checkIsRemote() {
        document.getElementById("IsRemoteOutput").innerText = "The current session is : " + (Windows.System.RemoteDesktop.InteractiveSession.isRemote ? "Remote" : "Local");
    }

})();
