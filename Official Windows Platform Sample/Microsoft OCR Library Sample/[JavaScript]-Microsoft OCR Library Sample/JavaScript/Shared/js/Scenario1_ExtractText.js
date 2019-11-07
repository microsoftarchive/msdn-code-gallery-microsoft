//// Copyright (c) Microsoft Corporation. All rights reserved



(function () {
    "use strict";

    // Keep objects in-scope across the lifetime of the scenario.
    var FileToken = "";

    // Define namespace and API aliases.
    var FutureAccess = Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList;
    var OCR = WindowsPreview.Media.Ocr;
    var ocrEngine = new OCR.OcrEngine(OCR.OcrLanguage.english);

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function query(element, elementId) {
        return element.querySelector("#" + elementId);
    }

    Object.prototype.getKeyByValue = function (value) {
        for (var prop in this) {
            if (this.hasOwnProperty(prop)) {
                if (this[prop] === value)
                    return prop;
            }
        }
    }

    var page = WinJS.UI.Pages.define("/html/Scenario1_ExtractText.html", {
        ready: function (element, options) {
            query(element, "buttonLoad").addEventListener("click", loadHandler, false);
            query(element, "buttonSample").addEventListener("click", sampleHandler, false);
            query(element, "buttonExtract").addEventListener("click", extractHandler, false);
            query(element, "languageList").addEventListener("change", languageChanged, false);
            query(element, "overlay").addEventListener("click", overlayHandler, false);

            // Populate language list based on OcrLanguage enum.
            var languagesList = query(element, "languageList");
            if (languagesList.options.length == 0) {
                var index = 0;
                for (var lang in OCR.OcrLanguage) {

                    var option = document.createElement("option");
                    option.text = lang;
                    option.value = lang;
                    languagesList.add(option);
                }
            }

            // Check Object.prototype.getKeyByValue above.
            languagesList.value = OCR.OcrLanguage.getKeyByValue(ocrEngine.language);

            // Continuation handlers are specific to Windows Phone.
            // For more information about continuable file pickers, see:
            // http://go.microsoft.com/fwlink/?LinkId=393345
            if (options && options.activationKind ===
                Windows.ApplicationModel.Activation.ActivationKind.pickFileContinuation) {
                var file = options.activatedEventArgs[0].files[0];

                // The returned file is null if the user cancelled the picker.
                // Do not wait for the load handler to complete.
                file && loadImage(file);
            }
        }
    });

    // Invoked when user select new language from language list.
    // Tries to change language for Optical Character Recognition.
    // If language resources are not present reverts selected language.
    // Check MSDN docs or readme.txt in NuGet Package to learn how to produce 
    // resource file that contains language specific resources.
    function languageChanged() {

        var selectedLanguage = id("languageList").value;

        try {

            clearResults();

            // Try to set new language for OCR.
            ocrEngine.language = OCR.OcrLanguage[selectedLanguage];

            WinJS.log && WinJS.log("OCR engine set to extract text in " + selectedLanguage + " language.",
                                    "sample",
                                    "status");
        } catch (e) {
            // Check Object.prototype.getKeyByValue above.
            id("languageList").value = OCR.OcrLanguage.getKeyByValue(ocrEngine.language);

            WinJS.log && WinJS.log(
                "Resource file 'MsOcrRes.opr' does not contain required resources for " + selectedLanguage + " language.\n" +
                    "Check MSDN docs or readme.txt in NuGet Package to learn how to produce resource file " +
                    "that contains " + selectedLanguage + " language specific resources.\n" +
                    "OCR language is now reverted to " + id("languageList").value + " language.",
                "sample",
                "error");
        }
    }

    // Invoked when the user clicks on the Load button.
    function loadHandler() {

        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
        picker.fileTypeFilter.append(".jpg");
        picker.fileTypeFilter.append(".jpeg");
        picker.fileTypeFilter.append(".png");

        // Windows Phone-specific codepath. After the picker is launched the app is closed.
        if ("pickSingleFileAndContinue" in picker) {
            picker.pickSingleFileAndContinue();

            // Windows-specific codepath. Do not wait for the load handler to complete.
            // Note that both Windows and Windows Phone define the pickSingleFileAsync() method
            // although it is only usable on Windows.
        } else {
            picker.pickSingleFileAsync().done(function (file) {
                // The returned file is null if the user cancelled the picker.
                file && loadImage(file);
            });
        }
    }

    // Invoked when the user clicks on the Sample button.
    function sampleHandler() {

        // Load sample "Hello World" image.
        Windows.ApplicationModel.Package.current.installedLocation.getFileAsync("sample\\sample.png")
            .then(function (file) {
                loadImage(file);
            });
    }

    // Load an image from a file and display for preview before extracting text.
    // On Windows this method is called once the file picker returns.
    // On Windows Phone this method is called after the app is reactivated
    // following the completion of the continuable file picker process.
    function loadImage(file) {
        // Request persisted access permissions to the file the user selected.
        // This allows the app to directly load the file in the future without relying on a
        // broker such as the file picker.
        FileToken = FutureAccess.add(file);

        // Display image for preview.
        id("imagePreview").src = window.URL.createObjectURL(file, { oneTimeOnly: true });

        // Clear results from previous OCR recognize.
        clearResults();

        // Update status message.
        file.properties.getImagePropertiesAsync().then(function (imageProps) {

            WinJS.log && WinJS.log("Loaded image from file " + file.displayName +
                                        " (" + imageProps.width + "x" + imageProps.height + ").",
                                    "sample",
                                    "status");
        });
    }

    // Clear results from previous OCR recognize.
    function clearResults() {
        id("imagePreview").style.transform = "";
        id("imagePreview").hidden = false;
        id("textOverlay").innerHTML = "";
        id("languageList").disabled = false;
        id("buttonExtract").disabled = false;
        id("imageText").textContent = "Text not extracted.";
        id("imageText").className = "imageText yellow";
        id("buttonExtract").disabled = false;
    }

    // Invoked when the user clicks on the Extract button.
    function extractHandler() {

        // Prevent another OCR request, since only image can be processed at the time at same OCR engine instance.
        id("buttonExtract").disabled = true;

        var imgWidth = 0;
        var imgHeight = 0;

        FutureAccess.getFileAsync(FileToken).then(function (file) {
            return file.openAsync(Windows.Storage.FileAccessMode.read);

        }).then(function (stream) {
            var bitmapDecoder = Windows.Graphics.Imaging.BitmapDecoder;
            return bitmapDecoder.createAsync(stream);

        }).then(function (decoder) {
            imgWidth = decoder.pixelWidth;
            imgHeight = decoder.pixelHeight;

            // Check whether is loaded image supported for processing.
            // Supported image dimensions are between 40 and 2600 pixels.
            if (imgHeight < 40 ||
                imgHeight > 2600 ||
                imgWidth < 40 ||
                imgWidth > 2600) {

                throw "Image size is not supported.\n" +
                        "Loaded image size is " + imgWidth + "x" + imgHeight + ".\n" +
                        "Supported image dimensions are between 40 and 2600 pixels."
            }

            return decoder.getPixelDataAsync();

        }).then(function (pixelProvider) {

            var buffer = pixelProvider.detachPixelData();
            return ocrEngine.recognizeAsync(imgHeight, imgWidth, buffer);

        }).then(function (result) {
            var extractedText = "";

            if (result.textAngle) {
                id("imagePreview").style.transform = "rotate(" + result.textAngle + "deg)";
            }

            var ratio = id("imagePreview").width / imgWidth;

            var ocrText = id("textOverlay");

            if (result.lines != null) {
                for (var l = 0; l < result.lines.length; l++) {
                    var line = result.lines[l];
                    for (var w = 0; w < line.words.length; w++) {
                        var word = line.words[w];

                        // Define "div" tag to overlay recognized word.
                        var wordbox = document.createElement("div");

                        wordbox.textContent = word.text;
                        wordbox.className = "result";
                        wordbox.style.width = Math.round(ratio * word.width) + "px";
                        wordbox.style.height = Math.round(ratio * word.height) + "px";
                        wordbox.style.top = Math.round(ratio * word.top) + "px";
                        wordbox.style.left = Math.round(ratio * word.left) + "px";
                        wordbox.style.fontSize = Math.round(ratio * word.height * 0.8) + "px";

                        // Put the filled textblock in the results grid.
                        ocrText.appendChild(wordbox);

                        extractedText += word.text + " ";
                    }
                    extractedText += "\n";
                }

                id("imageText").textContent = extractedText;
                id("imageText").rows = result.lines.length + 2;
                id("imageText").className = "imageText green";
            }
            else {
                id("imageText").textContent = "No text.";
                id("imageText").rows = 1;
                id("imageText").className = "imageText red";
            }

            WinJS.log && WinJS.log(
                                    "Image successfully processed in " + id("languageList").value + " language.",
                                    "sample",
                                    "status");

        }).then(null, function (e) {

            id("imageText").textContent = e.toString();
            id("imageText").rows = 3;
            id("imageText").className = "imageText red";

            WinJS.log && WinJS.log(e.toString(), "sample", "error");
        });
    }

    // Invoked when the user clicks on the Overlay button.
    // Check state of this this control determines whether extracted text will be overlaid over image. 
    function overlayHandler() {
        id("textOverlay").hidden = !id("overlay").checked;
    }
})();
