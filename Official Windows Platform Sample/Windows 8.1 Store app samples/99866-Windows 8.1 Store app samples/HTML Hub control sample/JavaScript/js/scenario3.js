//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("button3").addEventListener("click", launchFullScreenSample3, false);
        }
    });

    function launchFullScreenSample3() {
        WinJS.Navigation.navigate("/pages/interactiveheaders.html");
    }
})();