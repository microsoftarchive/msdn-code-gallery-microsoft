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
            document.getElementById("button1").addEventListener("click", launchFullScreenSample, false);
        }
    });

    function launchFullScreenSample() {
        var e = document.createElement("div");
        e.id = "full-screen";
        document.body.appendChild(e);
        
        var htmlControl = new WinJS.UI.HtmlControl(e, { uri: "/html/app.html" }, function (element) {
            WinJS.UI.Animation.enterContent(e, null).done();
        });
        
    }
})();
