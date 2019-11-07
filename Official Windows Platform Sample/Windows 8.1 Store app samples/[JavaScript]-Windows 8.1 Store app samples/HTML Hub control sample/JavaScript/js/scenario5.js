//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        ready: function (element, options) {
            document.getElementById("button5").addEventListener("click", launchFullScreenSample5, false);
        }
    });

    function launchFullScreenSample5() {
        WinJS.Navigation.navigate("/pages/verticallayout.html");
    }
})();