//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/inlineWebContent.html", {
        ready: function (element, options) {
            document.getElementById("saveBingAppId").addEventListener("click", saveBingAppId, false);
            document.getElementById("inlineWebContentIframe").addEventListener("load", loadBingAppId);
        }
    });

    function saveBingAppId() {
        var bingAppId = document.getElementById("bingAppId").value;
        Windows.Storage.ApplicationData.current.localSettings.values["bingAppId"] = bingAppId;
        sendAppIdToFrame(bingAppId);
    }

    function sendAppIdToFrame(bingAppId) {
        document.frames['inlineWebContentIframe'].postMessage(bingAppId, "ms-appx-web://" + document.location.host);
    }

    function loadBingAppId() {
        var bingAppId = Windows.Storage.ApplicationData.current.localSettings.values["bingAppId"];
        document.getElementById("bingAppId").value = bingAppId || "";
        sendAppIdToFrame(bingAppId);
    }

})();
