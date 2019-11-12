//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var httpClient;
    var httpPromise;
    var readBuffer;

    var page = WinJS.UI.Pages.define("/html/scenario2_GetStream.html", {
        ready: function (element, options) {
            document.getElementById("startButton").addEventListener("click", start, false);
            document.getElementById("cancelButton").addEventListener("click", cancel, false);
            httpClient = new Windows.Web.Http.HttpClient();
            readBuffer = Windows.Security.Cryptography.CryptographicBuffer.createFromByteArray(new Array(1000));
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

        httpPromise = httpClient.sendRequestAsync(
            request,
            Windows.Web.Http.HttpCompletionOption.responseHeadersRead).then(function (response) {
            outputField.value += Helpers.serializeHeaders(response);
            return response.content.readAsInputStreamAsync();
        }).then(function (responseStream) {
            return readDataAsync(responseStream);
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

    function readDataAsync(stream) {
        return stream.readAsync(readBuffer, readBuffer.capacity, Windows.Storage.Streams.InputStreamOptions.none).then(function (buffer) {
            var outputField = document.getElementById("outputField");
            outputField.value += "Bytes read from stream: " + buffer.length + "\n";

            // Use the buffer contents for something.  We can't safely display it as a string though, since encodings
            // like UTF-8 and UTF-16 have a variable number of bytes per character and so the last bytes in the buffer
            // may not contain a whole character.   Instead, we'll convert the bytes to hex and display the result.
            var responseBodyAsText = Windows.Security.Cryptography.CryptographicBuffer.encodeToHexString(buffer);
            outputField.value += responseBodyAsText + "\n";

            // Continue reading until the response is complete.
            return ((buffer.length > 0) ? readDataAsync(stream) : null);
        });
    }
})();
