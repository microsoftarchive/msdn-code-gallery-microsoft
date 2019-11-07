//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/scenario8_GetCookies.html", {
        ready: function (element, options) {
            document.getElementById("getCookiesButton").addEventListener("click", getCookies, false);
            document.getElementById("startButton").addEventListener("click", sendHttpGet, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);
            httpClient = new Windows.Web.Http.HttpClient();
        }
    });

    function getCookies() {
        var outputField = document.getElementById("outputField");

        // The value of 'AddressField' is set by the user and is therefore untrusted input. If we can't create a
        // valid, absolute URI, we'll notify the user about the incorrect input.
        var resourceAddress = Helpers.tryGetUri(document.getElementById("addressField").value.trim());
        if (!resourceAddress) {
            return;
        }

        try {
            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            var cookieCollection = filter.cookieManager.getCookies(resourceAddress);

            outputField.value = cookieCollection.size + " cookies found.\r\n";
            cookieCollection.forEach(function (cookie, index) {
                outputField.value += "--------------------\r\n";
                outputField.value += "Name: " + cookie.name + "\r\n";
                outputField.value += "Domain: " + cookie.domain + "\r\n";
                outputField.value += "Path: " + cookie.path + "\r\n";
                outputField.value += "Value: " + cookie.value + "\r\n";
                if (cookie.expires) {
                    outputField.value += "Expires: " + cookie.expires + "\r\n";
                } else {
                    outputField.value += "Expires:\r\n";
                }
                outputField.value += "Secure: " + cookie.secure + "\r\n";
                outputField.value += "HttpOnly: " + cookie.httpOnly + "\r\n";
            });
        }
        catch (ex) {
            WinJS.log && WinJS.log(ex, "sample", "error");
        }
    }

    function sendHttpGet() {
        // The value of 'AddressField' is set by the user and is therefore untrusted input. If we can't create a
        // valid, absolute URI, we'll notify the user about the incorrect input.
        var resourceAddress = Helpers.tryGetUri(document.getElementById("addressField").value.trim());
        if (!resourceAddress) {
            return;
        }

        Helpers.scenarioStarted();
        WinJS.log && WinJS.log("In progress", "sample", "status");

        var outputField = document.getElementById("outputField");

        httpPromise = httpClient.getAsync(resourceAddress).then(function (response) {
            return Helpers.displayTextResultAsync(response, outputField);
        });
        httpPromise.done(function () {
            WinJS.log && WinJS.log("Completed", "sample", "status");
            Helpers.scenarioCompleted();
        }, Helpers.onError);
    }

    function cancel() {
        if (httpPromise) {
            httpPromise.cancel();
        }
    }
})();
