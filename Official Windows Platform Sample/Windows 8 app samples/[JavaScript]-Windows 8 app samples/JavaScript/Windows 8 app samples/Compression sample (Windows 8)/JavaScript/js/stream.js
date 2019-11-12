//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/stream.html", {
        ready: function (element, options) {
            function scenario(algorithm) {
                return function () {
                    clearProgress();
                    if (typeof (algorithm) === "undefined") {
                        // Use default algorithm
                        doScenario();
                    } else {
                        doScenario(Windows.Storage.Compression.CompressAlgorithm[algorithm]);
                    }
                };
            }

            document.getElementById("DefaultButton").addEventListener("click", scenario(), false);
            document.getElementById("XpressButton").addEventListener("click", scenario("xpress"), false);
            document.getElementById("MszipButton").addEventListener("click", scenario("mszip"), false);
        }
    });

    function onError(e) {
        WinJS.log && WinJS.log(e, "sample", "error");
    }

    function clearProgress() {
        WinJS.log && WinJS.log("Testing...", "sample", "status");

        var progress = document.getElementById("scenarioProgress");
        progress.innerHTML = "";
    }

    function showProgress(message) {
        var progress = document.getElementById("scenarioProgress");
        progress.innerHTML += "<p>" + message + "</p>";
    }

    function getEnumerationValueName(namespace, value, defaultName) {
        for (var /*@override*/name in namespace) {
            if (namespace[name] === value) {
                return name;
            }
        }

        return defaultName;
    }

    //
    // Sample for bulk stream compression and decompression
    //
    function doScenario(compressAlgorithm) {
        // This scenario uses File Picker which doesn't work in snapped mode - try unsnap first
        // and fail gracefully if we can't
        var currentState = Windows.UI.ViewManagement.ApplicationView.value;
        if (currentState === Windows.UI.ViewManagement.ApplicationViewState.snapped &&
            !Windows.UI.ViewManagement.ApplicationView.tryUnsnap()) {

            onError("Sample doesn't work in snapped mode");
            return;
        }

        var filePicker = new Windows.Storage.Pickers.FileOpenPicker;
        filePicker.fileTypeFilter.append("*");

        filePicker.pickSingleFileAsync().then(function (file) {
            try {
                //
                // If user cancels file picker pickSingleFileAsync succeeds with result set to null
                //
                if (file === null) {
                    throw "No file has been selected";
                }

                var compressor;

                var algorithmName = getEnumerationValueName(Windows.Storage.Compression.CompressAlgorithm, compressAlgorithm, "default");

                //
                // Note, that file could be automatically renamed to prevent collision
                //
                var compressedFileName = file.name + "." + algorithmName + ".compressed";
                var decompressedFileName = file.name + "." + algorithmName + ".decompressed";

                //
                // It is safe to pass-through undefined value as compressAlgorithm in which case default algorithm (Xpress) will be used.
                // For the purposes of this sample both usages are shown.
                //
                if (typeof (compressAlgorithm) === "undefined") {
                    //
                    // If you don't have any specific algorithm requirements - this is recommended way to initialize Compressor object
                    //
                    compressor = new WinJS.Compressor(compressedFileName);
                } else {
                    compressor = new WinJS.Compressor(compressedFileName, compressAlgorithm);
                }

                compressor.compressAsync(file).then(function () {
                    showProgress("Compression done");
                    compressor.close();

                    try {
                        //
                        // We cannot use compressedFileName here because of automatic renaming
                        //
                        var decompressor = new WinJS.Decompressor(compressor.fileName);

                        return decompressor.copyAsync(decompressedFileName).then(function () {
                            showProgress("Decompression done");
                            decompressor.close();
                        }, onError);
                    } catch (e) {
                        onError(e);
                    }
                }, onError).done(function () {
                    WinJS.log && WinJS.log("Test finished", "sample", "status");
                }, onError);
            } catch (e) {
                onError(e);
            }
        }, onError);
    }
})();
