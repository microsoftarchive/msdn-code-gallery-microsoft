//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", launchFullScreenSample, false);
        }
    });

    function launchFullScreenSample() {
        WinJS.Navigation.navigate("/pages/basichub.html");
    }
})();