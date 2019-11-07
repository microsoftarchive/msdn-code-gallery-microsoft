//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;

    var page = WinJS.UI.Pages.define("/html/scenario3_GetList.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", start, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);
            httpClient = new Windows.Web.Http.HttpClient();
        }
    });

    function start() {
        // The value of 'AddressField' is set by the user and is therefore untrusted input. If we can't create a
        // valid, absolute URI, we'll notify the user about the incorrect input.
        var resourceUri = Helpers.tryGetUri(document.getElementById("addressField").value.trim());
        if (!resourceUri) {
            return;
        }

        Helpers.scenarioStarted();
        WinJS.log && WinJS.log("In progress", "sample", "status");

        var outputField = document.getElementById("outputField");

        httpPromise = httpClient.getAsync(resourceUri).then(function (response) {
            return Helpers.displayTextResultAsync(response, outputField).then(function () {
                response.ensureSuccessStatusCode();
                return response.content.readAsStringAsync();
            });
        }).then(function (responseBodyAsText) {
            var outputList = document.getElementById("outputList");

            var parser = new window.DOMParser();
            var xml = parser.parseFromString(responseBodyAsText, "text/xml");
            var items = xml.firstChild;
            for (var i = 0; i < items.childNodes.length; i++) {
                if (items.childNodes[i].nodeType === Node.ELEMENT_NODE) {
                    var item = items.childNodes[i].getAttribute("name");
                    var option = document.createElement("option");
                    option.text = item;
                    option.value = item;
                    outputList.options.add(option);
                }
            }
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
