//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario11_TryToGetAFileWithoutGettingAnError.html", {
        ready: function (element, options) {
            WinJS.log && WinJS.log("Windows Phone doesn’t currently support this function.", "sample", "error");
        }
    });
})();
