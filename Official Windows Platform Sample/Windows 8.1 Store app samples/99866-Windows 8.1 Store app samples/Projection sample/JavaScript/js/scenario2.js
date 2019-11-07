//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ViewManagement = Windows.UI.ViewManagement;

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            var output = document.getElementById("outputBox");
                if (ViewManagement.ProjectionManager.projectionDisplayAvailable) {
                    output.innerText = "A second screen is available";
                } else {
                    output.innerText = "No second screen is available";
                }
        }
    });

})();
