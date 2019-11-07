//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            document.getElementById("button4").addEventListener("click", launchFullScreenSample4, false);
        }
    });

    function launchFullScreenSample4() {
        WinJS.Navigation.navigate("/pages/semanticzoom.html");
    }
})();