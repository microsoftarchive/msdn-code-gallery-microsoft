//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/scenario11_MeteredConnectionFilter.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", start, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);
            document.getElementById("optInSwitch").addEventListener("change", optInSwitchToggled, false);

            var baseProtocolFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            Helpers.meteredConnectionFilter = new HttpFilters.HttpMeteredConnectionFilter(baseProtocolFilter);
            httpClient = new Windows.Web.Http.HttpClient(Helpers.meteredConnectionFilter);

            document.getElementById("optInSwitch").winControl.checked = Helpers.meteredConnectionFilter.optIn;
        }
    });

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

        var request = new Windows.Web.Http.HttpRequestMessage(Windows.Web.Http.HttpMethod.get, resourceAddress);

        var priority = HttpFilters.MeteredConnectionPriority.low;
        if (document.getElementById("mediumRadio").checked) {
            priority = HttpFilters.MeteredConnectionPriority.medium;
        } else if (document.getElementById("highRadio").checked) {
            priority = HttpFilters.MeteredConnectionPriority.high;
        }
        request.properties["meteredConnectionPriority"] = priority;

        httpPromise = httpClient.sendRequestAsync(request).then(function (response) {
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

    function optInSwitchToggled() {
        Helpers.meteredConnectionFilter.optIn = document.getElementById("optInSwitch").winControl.checked;
    }
})();
