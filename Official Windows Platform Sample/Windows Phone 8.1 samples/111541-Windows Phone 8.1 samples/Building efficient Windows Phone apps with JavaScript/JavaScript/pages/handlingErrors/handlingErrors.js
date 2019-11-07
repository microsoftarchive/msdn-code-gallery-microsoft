//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/handlingErrors/handlingErrors.html", {
        ready: function (element, options) {

            // ERROR: This code raises a custom error.
            throwError.addEventListener("click", function () {
                var newError = new WinJS.ErrorFromName("Custom error", "I'm an error!");
                throw newError;
            });

        },

        error: function (err) {
            WinJS.Utilities.startLog({ type: "pageError", tags: "Page" });
            WinJS.log && WinJS.log(err.message, "Page", "pageError");
        }
    });
})();
