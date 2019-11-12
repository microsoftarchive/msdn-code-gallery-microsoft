//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var Notifications = Windows.UI.Notifications;
    var templateSizes = {
        TileSquare150x150Image: { heights: [150], widths: [150], index: 0 },
        TileSquare150x150PeekImageAndText01: { heights: [150], widths: [150], index: 6 },
        TileSquare150x150PeekImageAndText02: { heights: [150], widths: [150], index: 7 },
        TileSquare150x150PeekImageAndText03: { heights: [150], widths: [150], index: 8 },
        TileSquare150x150PeekImageAndText04: { heights: [150], widths: [150], index: 9 },
        TileWide310x150Image: { heights: [150], widths: [310], index: 10 },
        TileWide310x150ImageCollection: { heights: [150, 75, 75, 75, 75], widths: [160, 75, 75, 75, 75], index: 11 },
        TileWide310x150ImageAndText01: { heights: [100], widths: [310], index: 12 },
        TileWide310x150ImageAndText02: { heights: [100], widths: [310], index: 13 },
        TileWide310x150PeekImageCollection01: { heights: [150, 75, 75, 75, 75], widths: [160, 75, 75, 75, 75], index: 16 },
        TileWide310x150PeekImageCollection02: { heights: [150, 75, 75, 75, 75], widths: [160, 75, 75, 75, 75], index: 17 },
        TileWide310x150PeekImageCollection03: { heights: [150, 75, 75, 75, 75], widths: [160, 75, 75, 75, 75], index: 18 },
        TileWide310x150PeekImageCollection04: { heights: [150, 75, 75, 75, 75], widths: [160, 75, 75, 75, 75], index: 19 },
        TileWide310x150PeekImageCollection05: { heights: [150, 75, 75, 75, 75, 80], widths: [160, 75, 75, 75, 75, 80], index: 20 },
        TileWide310x150PeekImageCollection06: { heights: [150, 75, 75, 75, 75, 80], widths: [160, 75, 75, 75, 75, 80], index: 21 },
        TileWide310x150PeekImageAndText01: { heights: [100], widths: [310], index: 22 },
        TileWide310x150PeekImageAndText02: { heights: [100], widths: [310], index: 23 },
        TileWide310x150PeekImage01: { heights: [150], widths: [310], index: 24 },
        TileWide310x150PeekImage02: { heights: [150], widths: [310], index: 25 },
        TileWide310x150PeekImage03: { heights: [150], widths: [310], index: 26 },
        TileWide310x150PeekImage04: { heights: [150], widths: [310], index: 27 },
        TileWide310x150PeekImage05: { heights: [150, 80], widths: [310, 80], index: 28 },
        TileWide310x150PeekImage06: { heights: [150, 80], widths: [310, 80], index: 29 },
        TileWide310x150SmallImageAndText01: { heights: [80], widths: [80], index: 30 },
        TileWide310x150SmallImageAndText02: { heights: [80], widths: [80], index: 31 },
        TileWide310x150SmallImageAndText03: { heights: [80], widths: [80], index: 32 },
        TileWide310x150SmallImageAndText04: { heights: [80], widths: [80], index: 33 },
        TileWide310x150SmallImageAndText05: { heights: [80], widths: [60], index: 34 },
        TileSquare310x310Image: { heights: [310], widths: [310], index: 48 },
        TileSquare310x310ImageAndText01: { heights: [260], widths: [310], index: 49 },
        TileSquare310x310ImageAndText02: { heights: [260], widths: [310], index: 50 },
        TileSquare310x310ImageAndTextOverlay01: { heights: [310], widths: [310], index: 51 },
        TileSquare310x310ImageAndTextOverlay02: { heights: [310], widths: [310], index: 52 },
        TileSquare310x310ImageAndTextOverlay03: { heights: [310], widths: [310], index: 53 },
        TileSquare310x310ImageCollectionAndText01: { heights: [185, 75, 75, 75, 75], widths: [310, 80, 75, 75, 80], index: 54 },
        TileSquare310x310ImageCollectionAndText02: { heights: [185, 75, 75, 75, 75], widths: [310, 80, 75, 75, 80], index: 55 },
        TileSquare310x310ImageCollection: { heights: [235, 75, 75, 75, 75], widths: [310, 80, 75, 75, 80], index: 56 },
        TileSquare310x310SmallImagesAndTextList01: { heights: [60, 60, 60], widths: [60, 60, 60], index: 57 },
        TileSquare310x310SmallImagesAndTextList02: { heights: [60, 60, 60], widths: [60, 60, 60], index: 58 },
        TileSquare310x310SmallImagesAndTextList03: { heights: [60, 60, 60], widths: [60, 60, 60], index: 59 },
        TileSquare310x310SmallImagesAndTextList04: { heights: [65, 65, 65], widths: [45, 45, 45], index: 60 },
        TileSquare310x310SmallImagesAndTextList05: { heights: [50, 50, 50], widths: [50, 50, 50], index: 73 },
        TileSquare310x310SmallImageAndText01: { heights: [100], widths: [100], index: 72 },
    };
    var height = 0;
    var width = 0;
    var origHeight = 0;
    var origWidth = 0;
    var croppingMethod = null;
    var aspectRatio = null;
    var imagePreviewElement;
    var cropPreviewElement;
    var cropPreviewDetailsElement;
    var displayName = "";

    var page = WinJS.UI.Pages.define("/html/scenario12_imageManipulation.html", {
        ready: function (element, options) {
            imagePreviewElement = document.getElementById("imagePreview");
            cropPreviewElement = document.getElementById("cropPreview");
            cropPreviewDetailsElement = document.getElementById("cropPreviewDetails");

            document.getElementById("imageManipulationSelector").addEventListener("change", imageManipulationSelectorChanged, false);
            document.getElementById("openPicker").addEventListener("click", openPicker, false);
            document.getElementById("tileSelector").addEventListener("change", tileSelectorChanged, false);
            document.getElementById("tileImageSelector").addEventListener("change", tileImageSelectorChanged, false);
            document.getElementById("cropScaleSelector").addEventListener("change", tileImageSelectorChanged, false);
            document.getElementById("croppingMethodTallSelector").addEventListener("change", croppingMethodSelectorChanged, false);
            document.getElementById("croppingMethodWideSelector").addEventListener("change", croppingMethodSelectorChanged, false);
            document.getElementById("scaleSelector").addEventListener("change", scaleSelectorChanged, false);
            document.getElementById("saveButton").addEventListener("click", saveButtonClicked, false);

            document.getElementById("upperLeftX").addEventListener("input", customCropChanged, false);
            document.getElementById("upperLeftY").addEventListener("input", customCropChanged, false);
            document.getElementById("cropWidth").addEventListener("input", customCropChanged, false);
            document.getElementById("cropHeight").addEventListener("input", customCropChanged, false);

            document.getElementById("upArrow").addEventListener("click", function () { changeCrop(false); }, false);
            document.getElementById("downArrow").addEventListener("click", function () { changeCrop(true); }, false);
            document.getElementById("cropPx").addEventListener("input", customScaledCropChanged, false);
        }
    });

    function scaleImageMaxSize(startWidth, maxWidth, startHeight, maxHeight) {
        // Scales the image such that width and height both fall underneath the max values
        if ((startHeight / maxHeight) > (startWidth / maxWidth)) {
            startWidth = maxWidth / startHeight * startWidth;
            startHeight = maxWidth;
        } else {
            startHeight = maxWidth / startWidth * startHeight;
            startWidth = maxWidth;
        }
        return { "width": startWidth, "height": startHeight };
    }

    function scaleImageMeetSize(startWidth, wantedWidth, startHeight, wantedHeight) {
        // Scales the image such that the smallest dimension meets the wanted dimension
        if ((startHeight / wantedHeight) < (startWidth / wantedWidth)) {
            startWidth = wantedHeight / startHeight * startWidth;
            startHeight = wantedHeight;
        } else {
            startHeight = wantedWidth / startWidth * startHeight;
            startWidth = wantedWidth;
        }
        return { "width": startWidth, "height": startHeight };
    }

    function restrict(value, max, min) {
        // Restricts the value between max and min
        value = parseInt(value) || 0;
        value = Math.max(min, value);
        value = Math.min(max, value);
        return value;
    }

    function scaleSelectorChanged() {
        // Scaling of an image by percentage
        var scaleSelector = document.getElementById("scaleSelector");
        var scale = scaleSelector.options[scaleSelector.selectedIndex].value;

        if (scale === "max") {
            // If the image has dimensions greater than 1024x1024, scale it to fit within that size
            if (1024 < height || 1024 < width) {
                var scaled = scaleImageMaxSize(width, 1024, height, 1024);
                cropPreviewElement.width = scaled.width;
                cropPreviewElement.height = scaled.height;
            } else {
                cropPreviewElement.width = origWidth;
                cropPreviewElement.height = origHeight;
            }
        } else {
            cropPreviewElement.width = origWidth * scale;
            cropPreviewElement.height = origHeight * scale;
        }

        cropPreviewElement.getContext("2d").drawImage(imagePreviewElement, 0, 0, origWidth, origHeight, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
        setPreviewDetails();
    }

    function customCropChanged() {
        // Crops the image according to an upper left hand corner and the height and width of the crop
        var upperLeftXElement = document.getElementById("upperLeftX");
        var upperLeftYElement = document.getElementById("upperLeftY");
        var cropWidth = document.getElementById("cropWidth");
        var cropHeight = document.getElementById("cropHeight");

        width = origWidth;
        height = origHeight;
        var upperLeftX = restrict(parseInt(upperLeftXElement.value), width, 0);
        var upperLeftY = restrict(parseInt(upperLeftYElement.value), height, 0);
        var cropWidthValue = restrict(cropWidth.value, width - upperLeftX, 1);
        var cropHeightValue = restrict(cropHeight.value, height - upperLeftY, 1);

        // Default to a height and width of 1 px
        width = restrict(cropWidthValue, width, 1);
        height = restrict(cropHeightValue, height, 1);
        cropPreviewElement.width = width;
        cropPreviewElement.height = height;
        cropPreviewElement.getContext("2d").drawImage(imagePreviewElement, upperLeftX, upperLeftY, width, height, 0, 0, width, height);
        setPreviewDetails();

        // Set values to restricted ones
        upperLeftXElement.value = upperLeftX;
        upperLeftYElement.value = upperLeftY;
        cropWidth.value = width;
        cropHeight.value = height;
    }

    function customScaledCropChanged() {
        // Resize image
        var tempCanvas = document.createElement("canvas");
        tempCanvas.width = width;
        tempCanvas.height = height;
        tempCanvas.getContext("2d").drawImage(imagePreviewElement, 0, 0, width, height);
        var ctx = cropPreviewElement.getContext("2d");
        cropPreviewElement.width = aspectRatio.width;
        cropPreviewElement.height = aspectRatio.height;
        var starting = parseInt(document.getElementById("cropPx").value);

        if (croppingMethod) {
            var crop = croppingMethod.options[croppingMethod.selectedIndex].value;
            if (crop === "CustomTall") {
                starting = Math.floor(restrict(starting, height - cropPreviewElement.height, 0));
                ctx.drawImage(tempCanvas, 0, starting, cropPreviewElement.width, cropPreviewElement.height, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
            }
            else if (crop === "CustomWide") {
                starting = Math.floor(restrict(starting, width - cropPreviewElement.width, 0));
                ctx.drawImage(tempCanvas, starting, 0, cropPreviewElement.width, cropPreviewElement.height, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
            }
        }
        document.getElementById("cropPx").value = starting;
    }

    function croppingMethodSelectorChanged() {
        var cropDiv = document.getElementById("cropDiv");
        // Resize image
        var tempCanvas = document.createElement("canvas");
        tempCanvas.width = width;
        tempCanvas.height = height;
        tempCanvas.getContext("2d").drawImage(imagePreviewElement, 0, 0, width, height);

        var ctx = cropPreviewElement.getContext("2d");
        cropPreviewElement.width = aspectRatio.width;
        cropPreviewElement.height = aspectRatio.height;

        // Crop image
        if (croppingMethod) {
            WinJS.Utilities.addClass(cropDiv, "hidden");
            var crop = croppingMethod.options[croppingMethod.selectedIndex].value;
            switch (crop) {
                case "Profile":
                    ctx.drawImage(tempCanvas,                       // Draw source
                        0,                                          // Source upper left X
                        (height - cropPreviewElement.height) / 3,   // Source upper left Y
                        cropPreviewElement.width,                   // Width to copy from source
                        cropPreviewElement.height,                  // Height to copy from source
                        0,                                          // Destination upper left X
                        0,                                          // Destination upper left Y
                        cropPreviewElement.width,                   // Destination width
                        cropPreviewElement.height);                 // Destination Height
                    break;
                case "Top":
                    ctx.drawImage(tempCanvas, 0, 0, cropPreviewElement.width, cropPreviewElement.height, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
                    break;
                case "CenterTall":
                    ctx.drawImage(tempCanvas, 0, height / 2 - cropPreviewElement.height / 2, cropPreviewElement.width, cropPreviewElement.height, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
                    break;
                case "CenterWide":
                    ctx.drawImage(tempCanvas, width / 2 - cropPreviewElement.width / 2, 0, cropPreviewElement.width, cropPreviewElement.height, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
                    break;
                case "Bottom":
                    ctx.drawImage(tempCanvas, 0, height - cropPreviewElement.height, cropPreviewElement.width, cropPreviewElement.height, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
                    break;
                case "CustomTall":
                    // Crop by Y axis
                    WinJS.Utilities.removeClass(cropDiv, "hidden");
                    customScaledCropChanged();
                    break;
                case "CustomWide":
                    // Crop by X axis
                    WinJS.Utilities.removeClass(cropDiv, "hidden");
                    customScaledCropChanged();
                    break;
                case "Left":
                    ctx.drawImage(tempCanvas, 0, 0, cropPreviewElement.width, cropPreviewElement.height, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
                    break;
                case "Right":
                    ctx.drawImage(tempCanvas, width - cropPreviewElement.width, 0, cropPreviewElement.width, cropPreviewElement.height, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
                    break;
            }
            setPreviewDetails();
        } else {
            // Display image without cropping
            ctx.drawImage(tempCanvas, 0, 0, cropPreviewElement.width, cropPreviewElement.height);
        }
    }

    function saveButtonClicked() {
        // Save canvas to an image file
        var blob = cropPreviewElement.msToBlob();
        var input = blob.msDetachStream();
        var accessStream = null;
        var file = null;
        var previewWidth = cropPreviewElement.width;
        var previewHeight = cropPreviewElement.height;

        var fileName = displayName + "_" + previewWidth + "x" + previewHeight + ".jpg";
        Windows.Storage.ApplicationData.current.localFolder.createFileAsync(fileName, Windows.Storage.NameCollisionOption.generateUniqueName).then(function (result) {
            file = result;
            return file.openAsync(Windows.Storage.FileAccessMode.readWrite);
        }).then(function (result) {
            accessStream = result;
            // Copy the stream from the blob to the File stream
            return Windows.Storage.Streams.RandomAccessStream.copyAsync(input, accessStream);
        }).then(function (result) {
            return accessStream.flushAsync();
        }).then(function (result) {
            input.close();
            accessStream.close();
            blob.msClose();
        }).done(function () {
            WinJS.log && WinJS.log("Cropped image saved to " + file.path, "sample", "status");
        }, function (e) {
            WinJS.log && WinJS.log(e, "sample", "error");
        });
    }

    function openPicker() {
        // Load an image
        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
        picker.fileTypeFilter.replaceAll([".jpg", ".jpeg", ".png", ".gif"]);
        picker.commitButtonText = "Copy";
        var fileSize = "";
        var pickedFile = null;


        var errorFunction = function (e) {
            WinJS.log && WinJS.log(e, "sample", "error");
        };

        picker.pickSingleFileAsync().then(function (file) {
            if (file) {
                pickedFile = file;
                displayName = file.displayName;
                pickedFile.getBasicPropertiesAsync().then(function (basicProperties) {
                    if (basicProperties) {
                        fileSize = basicProperties.size / 1000; // Convert to kb
                        pickedFile.properties.getImagePropertiesAsync().then(function (imageProperties) {
                            if (imageProperties) {
                                origHeight = imageProperties.height;
                                origWidth = imageProperties.width;
                                pickedFile.copyAsync(Windows.Storage.ApplicationData.current.localFolder, pickedFile.name, Windows.Storage.NameCollisionOption.generateUniqueName).done(function (newFile) {
                                    if (newFile) {
                                        var imageDetails = document.getElementById("imageDetails");
                                        var imageSelectedDiv = document.getElementById("imageSelectedDiv");
                                        var fileName = newFile.name;
                                        imagePreviewElement.src = "ms-appdata:///Local/" + fileName; // Change image to relative path
                                        WinJS.Utilities.removeClass(imageSelectedDiv, "hidden");
                                        width = origWidth;
                                        height = origHeight;
                                        imageDetails.innerHTML = "Image copied to: ms-appdata:///Local/" + fileName + "<br/>Dimensions: " + width + " x " + height + "px <br/>" + "Size: " + fileSize + "kb";
                                        imagePreviewElement.onload = function () {
                                            imageManipulationSelectorChanged();
                                        };

                                        WinJS.log && WinJS.log("Image copied to application data local storage: " + newFile.path, "sample", "status");
                                    }
                                }, errorFunction);
                            }
                        }, errorFunction);
                    }
                }, errorFunction);
            }
        }, errorFunction);
    }

    function changeCrop(isSubtract) {
        // UI code: add or subtract the cropping parameter by 1
        var cropPx = document.getElementById("cropPx");
        var starting = parseInt(cropPx.value) || 0;
        var crop = croppingMethod.options[croppingMethod.selectedIndex].value;

        if (isSubtract) {
            starting--;
        } else {
            starting++;
        }
        cropPx.value = starting;
        customScaledCropChanged();
    }

    function tileSelectorChanged() {
        // UI code: shows the list of available images based on the template selected
        var tileSelector = document.getElementById("tileSelector");
        var tileImageSelector = document.getElementById("tileImageSelector");
        var tilePreview = document.getElementById("tilePreview");
        var tilePreviewName = tileSelector.options[tileSelector.selectedIndex].value;

        // Set image selector options
        tilePreview.setAttribute("src", "images/tiles/" + tilePreviewName + ".png");
        var template = templateSizes[tileSelector.options[tileSelector.selectedIndex].value].index;
        var tileXml = Notifications.TileUpdateManager.getTemplateContent(template);
        var tileImageAttributes = tileXml.getElementsByTagName("image");
        tileImageSelector.innerHTML = "";
        for (var imageAttribute = 1, max = tileImageAttributes.length + 1; imageAttribute < max; imageAttribute++) {
            tileImageSelector.add(new Option("Image " + imageAttribute, null));
        }
        tileImageSelectorChanged();
    }

    function tileImageSelectorChanged() {
        // UI code: shows the proper cropping options based on the image loaded
        var index = document.getElementById("tileImageSelector").selectedIndex;
        var tileSelector = document.getElementById("tileSelector");
        var cropScaleSelector = document.getElementById("cropScaleSelector");
        var imagePosition = document.getElementById("imagePositionDiv");
        var croppingMethodTall = document.getElementById("croppingMethodTallSelector");
        var croppingMethodWide = document.getElementById("croppingMethodWideSelector");
        var croppingMethodDiv = document.getElementById("croppingMethodDiv");
        var heights = templateSizes[tileSelector.options[tileSelector.selectedIndex].value].heights;
        var widths = templateSizes[tileSelector.options[tileSelector.selectedIndex].value].widths;
        var scale = cropScaleSelector.options[cropScaleSelector.selectedIndex].value;

        // Adjust image according to the scale
        aspectRatio = { height: heights[index] * scale, width: widths[index] * scale };
        var scaled = scaleImageMeetSize(width, aspectRatio.width, height, aspectRatio.height);
        width = scaled.width;
        height = scaled.height;

        if (widths.length <= 1) {
            WinJS.Utilities.addClass(imagePosition, "hidden");
        } else {
            WinJS.Utilities.removeClass(imagePosition, "hidden");
        }

        // If the image is too big, show cropping options
        if ((aspectRatio.height <= height) || (aspectRatio.width <= width)) {
            if ((height / aspectRatio.height) < (width / aspectRatio.width)) {
                // Image is too wide
                WinJS.Utilities.addClass(croppingMethodTall, "hidden");
                WinJS.Utilities.removeClass(croppingMethodWide, "hidden");
                croppingMethod = croppingMethodWide;
                document.getElementById("cropText").innerHTML = "Starting point for cropping in the X axis (0 px is at the left)";
            } else {
                // Image is too tall
                WinJS.Utilities.addClass(croppingMethodWide, "hidden");
                WinJS.Utilities.removeClass(croppingMethodTall, "hidden");
                croppingMethod = croppingMethodTall;
                document.getElementById("cropText").innerHTML = "Starting point for cropping in the Y axis (0 px is at the top)";
            }
        }

        // Image is perfect, don't show any cropping options
        if (height === aspectRatio.height && width === aspectRatio.width) {
            WinJS.Utilities.addClass(croppingMethodDiv, "hidden");
            croppingMethod = null;
        } else {
            WinJS.Utilities.removeClass(croppingMethodDiv, "hidden");
        }
        croppingMethodSelectorChanged();
    }

    function imageManipulationSelectorChanged() {
        // UI code: toggles the visibility of autoscale & crop/scale/crop divs
        var manipulationProtocol = document.getElementById("imageManipulationSelector").selectedIndex;
        var cropScaleSelector = document.getElementById("cropScaleSelector");
        var scaleValue = cropScaleSelector.options[cropScaleSelector.selectedIndex].value;
        var scaleAndCrop = document.getElementById("scaleAndCrop");
        var scale = document.getElementById("scale");
        var crop = document.getElementById("crop");
        var tileSelector = document.getElementById("tileSelector");

        width = origWidth * scaleValue;
        height = origHeight * scaleValue;
        cropPreviewElement.getContext("2d").clearRect(0, 0, cropPreviewElement.width, cropPreviewElement.height);

        cropPreviewDetailsElement.innerHTML = "";
        WinJS.Utilities.addClass(scale, "hidden");
        WinJS.Utilities.addClass(crop, "hidden");
        WinJS.Utilities.addClass(scaleAndCrop, "hidden");

        if (manipulationProtocol === 0) {
            WinJS.Utilities.removeClass(scaleAndCrop, "hidden");
            tileSelectorChanged();
        } else if (manipulationProtocol === 1) {
            WinJS.Utilities.removeClass(scale, "hidden");
            scaleSelectorChanged();
        } else if (manipulationProtocol === 2) {
            WinJS.Utilities.removeClass(crop, "hidden");
        }
    }

    function setPreviewDetails() {
        // UI code: displays details about the preview image
        var previewWidth = cropPreviewElement.width;
        var previewHeight = cropPreviewElement.height;
        var size = cropPreviewElement.msToBlob().size / 1000;

        cropPreviewDetailsElement.innerHTML = "Dimensions: " + previewWidth + " x " + previewHeight + "px"
            + "<br/>Size: " + size + "kb";

        // Display error message if image is > 1024x1024 or has a size > 200kb
        if (previewWidth > 1024 || previewHeight > 1024 || size > 200) {
            WinJS.Utilities.removeClass(document.getElementById('error'), "hidden");
        } else {
            WinJS.Utilities.addClass(document.getElementById('error'), "hidden");
        }
    }
})();