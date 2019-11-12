//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("button2").addEventListener("click", launchFullScreenSample2, false);
        }
    });

    function launchFullScreenSample2() {
        WinJS.Navigation.navigate("/pages/heroimage.html");
    }
})();