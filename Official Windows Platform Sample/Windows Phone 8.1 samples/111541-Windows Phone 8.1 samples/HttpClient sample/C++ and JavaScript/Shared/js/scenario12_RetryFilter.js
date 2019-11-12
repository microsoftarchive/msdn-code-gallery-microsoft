//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/scenario12_RetryFilter.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", start, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);
            document.getElementById("retryAfterSwitch").addEventListener("change", updateAddressField, false);

            var baseProtocolFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            var retryFilter = new HttpFilters.HttpRetryFilter(baseProtocolFilter);
            httpClient = new Windows.Web.Http.HttpClient(retryFilter);
            updateAddressField();
        }
    });

    function updateAddressField() {
        // Tell the server we want a transfer-encoding chunked response.
        var queryString = "";
        var x = document.getElementById("chunkedResponseToggle");
        if (document.getElementById("retryAfterSwitch").winControl.checked) {
            queryString = "?retryAfter=deltaSeconds";
        } else {
            queryString = "?retryAfter=httpDate";
        }

        Helpers.replaceQueryString(queryString);
    }

    function start() {
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
