//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("writeBytes").addEventListener("click", writeBytes, false);
            document.getElementById("readBytes").addEventListener("click", readBytes, false);
            // To test if "sample.dat" is created.
            SdkSample.validateFileExistence();
        }
    });

    // Writes some text to 'sample.dat' using bytes
    function writeBytes() {
        if (SdkSample.sampleFile !== null) {
            var textArea = document.getElementById("textarea");
            var userContent = textArea.innerText;
            var outputDiv = document.getElementById("output");
            if (userContent !== "") {
                var buffer = getBufferFromString(userContent);
                Windows.Storage.FileIO.writeBufferAsync(SdkSample.sampleFile, buffer).done(function () {
                    outputDiv.innerHTML = "The following " + buffer.length + " bytes of text were written to '" + SdkSample.sampleFile.name + "':<br /><br />" + userContent;
                },
                function (error) {
                    WinJS.log && WinJS.log(error, "sample", "error");
                });
            } else {
                outputDiv.innerHTML = "The text box is empty, please write something and then click 'Write' again.";
            }
        }
    }

    function getBufferFromString(str) {
        var memoryStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();
        var dataWriter = new Windows.Storage.Streams.DataWriter(memoryStream);
        dataWriter.writeString(str);
        var buffer = dataWriter.detachBuffer();
        dataWriter.close();
        return buffer;
    }

    // Reads text from 'sample.dat' using bytes
    function readBytes() {
        if (SdkSample.sampleFile !== null) {
            Windows.Storage.FileIO.readBufferAsync(SdkSample.sampleFile).done(function (buffer) {
                var dataReader = Windows.Storage.Streams.DataReader.fromBuffer(buffer);
                var fileContent = dataReader.readString(buffer.length);
                dataReader.close();
                var outputDiv = document.getElementById("output");
                outputDiv.innerHTML = "The following " + buffer.length + " bytes of text were read from '" + SdkSample.sampleFile.name + "':<br /><br />" + fileContent;
            },
            function (error) {
                WinJS.log && WinJS.log(error, "sample", "error");
            });
        }
    }
})();
