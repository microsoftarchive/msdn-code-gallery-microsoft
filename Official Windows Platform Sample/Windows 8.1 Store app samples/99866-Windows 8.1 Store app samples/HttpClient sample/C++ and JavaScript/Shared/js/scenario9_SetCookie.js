//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario9_SetCookie.html", {
        ready: function (element, options) {
            document.getElementById("setCookieButton").addEventListener("click", setCookie, false);

            if (WinJS.UI.hasOwnProperty("DatePicker")) {
                // If DatePicker is available, we do not need the text input.
                document.getElementById("expiresDateAndTimeField").style.visibility = "hidden";
            } else {
                // If DatePicker is not available, set the text input value to the current time.
                document.getElementById("expiresDateAndTimeField").value = new Date();
            }
        }
    });

    function setCookie() {
        var outputField = document.getElementById("outputField");
        var cookie = null;

        try {
            cookie = new Windows.Web.Http.HttpCookie(
                document.getElementById("nameField").value,
                document.getElementById("domainField").value,
                document.getElementById("pathField").value);

            cookie.value = document.getElementById("valueField").value;
        }
        catch (ex) {
            // Catch argument exceptions.
            WinJS.log && WinJS.log("Invalid argument: " + ex, "sample", "error");
            return;
        }

        try {
            if (document.getElementById("nullCheckBox").checked) {
                cookie.expires = null;
            } else if (WinJS.UI.hasOwnProperty("DatePicker")) {
                var currentDate = expiresDatePicker.winControl.current;
                var currentTime = expiresTimePicker.winControl.current;
                currentDate.setHours(currentTime.getHours());
                currentDate.setMinutes(currentTime.getMinutes());
                cookie.expires = currentDate;
            } else {
                var dateString = document.getElementById("expiresDateAndTimeField").value;
                cookie.expires = new Date(Date.parse(dateString));
            }

            cookie.secure = document.getElementById("secureToggle").winControl.checked;
            cookie.httpOnly = document.getElementById("httpOnlyToggle").winControl.checked;

            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            var replaced = filter.cookieManager.setCookie(cookie, false);

            if (replaced) {
                WinJS.log && WinJS.log("Cookie replaced.", "sample", "status");
            } else {
                WinJS.log && WinJS.log("Cookie set.", "sample", "status");
            }
        }
        catch (error) {
            WinJS.log && WinJS.log(error, "sample", "error");
        }
    }
})();
