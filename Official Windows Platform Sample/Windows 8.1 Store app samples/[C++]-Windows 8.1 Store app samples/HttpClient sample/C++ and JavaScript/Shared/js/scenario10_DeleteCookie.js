//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario10_DeleteCookie.html", {
        ready: function (element, options) {
            document.getElementById("deleteCookieButton").addEventListener("click", deleteCookie, false);
        }
    });

    function deleteCookie() {
        var outputField = document.getElementById("outputField");
        var cookie = null;

        try {
            cookie = new Windows.Web.Http.HttpCookie(
                document.getElementById("nameField").value,
                document.getElementById("domainField").value,
                document.getElementById("pathField").value);
        }
        catch (ex) {
            // Catch argument exceptions.
            WinJS.log && WinJS.log("Invalid argument: " + ex, "sample", "error");
            return;
        }

        try {
            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            filter.cookieManager.deleteCookie(cookie);

            WinJS.log && WinJS.log("Cookie deleted.", "sample", "status");
        }
        catch (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
        }
    }
})();
