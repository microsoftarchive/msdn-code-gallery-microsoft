//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/home/home.html", {
        processed: function (element, option) {
            WinJS.log && WinJS.log("", "Sample", "status"); /* Clear the log */
            return WinJS.Binding.processAll(element);
        }
    });
})();