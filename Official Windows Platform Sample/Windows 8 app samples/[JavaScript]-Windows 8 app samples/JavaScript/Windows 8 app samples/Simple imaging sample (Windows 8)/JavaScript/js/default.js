//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Simple imaging JS sample";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Image editor (Windows.Storage.FileProperties)" },
        { url: "/html/scenario2.html", title: "Image editor (Windows.Graphics.Imaging)" },
        { url: "/html/scenario3.html", title: "Drawing app (HTML Canvas)" },
        { url: "/html/scenario4.html", title: "Photo pins (Bing Maps AJAX API)" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
       
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();

// Provides shared imaging functionality to all scenarios, focusing on
// EXIF orientation, GPS, and rating metadata.
(function () {
    "use strict";

    WinJS.Namespace.define("Helpers", {
        // Converts a failure HRESULT (Windows error code) to a number which can be compared to the
        // WinRTError.number parameter. This allows us to define error codes in terms of well-known
        // Windows HRESULTs, found in winerror.h.
        "convertHResultToNumber": function (hresult) {
            if ((hresult > 0xFFFFFFFF) || (hresult < 0x80000000)) {
                throw new Error("Value is not a failure HRESULT.");
            }

            return hresult - 0xFFFFFFFF - 1;
        },
    
        // Opens a file picker with appropriate settings and asynchronously returns the selected file
        // Throws an exception if the user clicks cancel (there is no file).
        "getFileFromOpenPickerAsync": function () {
            // Attempt to ensure that the view is not snapped, otherwise the picker will not display.
            var viewState = Windows.UI.ViewManagement.ApplicationView.value;
            if (viewState === Windows.UI.ViewManagement.ApplicationViewState.snapped &&
                !Windows.UI.ViewManagement.ApplicationView.tryUnsnap())
            {
                throw new Error("File picker cannot display in snapped view.");
            }

            WinJS.log && WinJS.log("Loading image from picker...", "sample", "status");
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
            Helpers.fillDecoderExtensions(picker.fileTypeFilter);
            return picker.pickSingleFileAsync().then(function (file) {
                if (!file) {
                    throw new Error("User did not select a file.");
                }

                return file;
            });
        },

        // Opens a file picker with appropriate settings and asynchronously returns all of the
        // selected files. Throws an exception if the user clicks cancel (there is no file).
        "getFilesFromOpenPickerAsync": function () {
            // Attempt to ensure that the view is not snapped, otherwise the picker will not display.
            var viewState = Windows.UI.ViewManagement.ApplicationView.value;
            if (viewState === Windows.UI.ViewManagement.ApplicationViewState.snapped &&
                !Windows.UI.ViewManagement.ApplicationView.tryUnsnap())
            {
                throw new Error("File picker cannot display in snapped view.");
            }

            WinJS.log && WinJS.log("Loading images from picker...", "sample", "status");
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
            Helpers.fillDecoderExtensions(picker.fileTypeFilter);
            return picker.pickMultipleFilesAsync().then(function (files) {
                if (files.size === 0) {
                    throw new Error("User did not select a file.");
                }

                return files;
            });
        },

        // Opens a file picker with appropriate settings and asynchronously returns a file
        // that the user has selected as the encode destination. Selects a few common
        // encoding formats.
        "getFileFromSavePickerAsync": function () {
            // Attempt to ensure that the view is not snapped, otherwise the picker will not display.
            var viewState = Windows.UI.ViewManagement.ApplicationView.value;
            if (viewState === Windows.UI.ViewManagement.ApplicationViewState.snapped &&
                !Windows.UI.ViewManagement.ApplicationView.tryUnsnap())
            {
                throw new Error("File picker cannot display in snapped view.");
            }

            var picker = new Windows.Storage.Pickers.FileSavePicker();

            // Restrict the user to a fixed list of file formats that support encoding.
            picker.fileTypeChoices.insert("PNG file", [".png"]);
            picker.fileTypeChoices.insert("BMP file", [".bmp"]);
            picker.fileTypeChoices.insert("JPEG file", [".jpg"]);
            picker.defaultFileExtension = ".png";
            picker.suggestedFileName = "Output file";
            picker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;

            return picker.pickSaveFileAsync().then(function (file) {
                if (!file) {
                    throw new Error("User did not select a file.");
                }

                return file;
            });
        },

        // Gets the file extensions supported by all of the bitmap codecs installed on the system.
        // The "collection" argument is of type IVector and implements the append method. The
        // function does not return a value; instead, it populates the collection argument with
        // the file extensions.
        "fillDecoderExtensions": function (collection) {
            var enumerator = Windows.Graphics.Imaging.BitmapDecoder.getDecoderInformationEnumerator();

            enumerator.forEach(function (decoderInfo) {
                // Each bitmap codec contains a list of file extensions it supports; get this list
                // and append every element in the list to "collection".
                decoderInfo.fileExtensions.forEach(function (fileExtension) {
                    collection.append(fileExtension);
                });
            });
        },

        // Calculates the dimensions necessary to fit an HTML element within a square of a
        // specified side length, while preserving aspect ratio. For example, a 500x250 element
        // must be scaled to 300x150 in order to fit within a square with side length 300.
        // Does not rescale dimensions if the element fits entirely within the square.
        "getScaledDimensions": function (width, height, maxSideLength) {
            var scaleFactor = maxSideLength / Math.max(width, height);
            var dimensions = {};

            if (scaleFactor >= 1) {
                dimensions.width = width;
                dimensions.height = height;
            } else {
                dimensions.width = Math.floor(width * scaleFactor);
                dimensions.height = Math.floor(height * scaleFactor);
            }

            return dimensions;
        },

        //
        //  ***** Rating helpers (System.Rating property and WinJS rating control) *****
        //

        // convertSystemRatingToStars and convertStarsToSystemRating convert between the
        // "System.Rating" value (1-99) and WinJS rating control value (0-5 stars).
        // The behavior is consistent with how Windows handles "System.Rating", and is
        // documented at:
        // http://msdn.microsoft.com/en-us/library/bb787554(v=VS.85).aspx
        // A value of 0 is equivalent to 0 stars/no rating.
        // This function only works for rating controls with 5 values.
        "convertSystemRatingToStars": function (systemRating) {
            if (systemRating <= 0) {
                return 0;
            } else if (systemRating <= 12) {
                return 1;
            } else if (systemRating <= 37) {
                return 2;
            } else if (systemRating <= 62) {
                return 3;
            } else if (systemRating <= 87) {
                return 4;
            } else {
                return 5;
            }
        },

        "convertStarsToSystemRating": function (numStars) {
            if (numStars <= 0) {
                return 0;
            } else if (numStars === 1) {
                return 1;
            } else if (numStars === 2) {
                return 25;
            } else if (numStars === 3) {
                return 50;
            } else if (numStars === 4) {
                return 75;
            } else {
                return 99;
            }
        },

        //
        //  ***** GPS/Geolocation property helpers *****
        //

        // ImageProperties.Latitude and ImageProperties.Longitude are returned
        // as double precision numbers. This function converts them to a 
        // degrees/minutes/seconds/reference ("N/E/W/S") format.
        // latLong is either a signed latitude or longitude value.
        // isLatitude is a boolean indicating whether latLong is latitude or longitude.
        "convertLatLongToString": function (latLong, isLatitude) {
            var reference;
            if (isLatitude) {
                reference = (latLong >= 0) ? "N" : "S";
            } else {
                reference = (latLong >= 0) ? "E" : "W";
            }

            latLong = Math.abs(latLong);
            var degrees = Math.floor(latLong);
            var minutes = Math.floor((latLong - degrees) * 60);
            var seconds = ((latLong - degrees - minutes / 60) * 3600).toFixed(2);

            return degrees + "°" + minutes + "\'" + seconds + "\"" + reference;
        },

        //
        //  ***** EXIF orientation helpers *****
        //

        // Converts a PhotoOrientation value into a human readable string.
        // The text is adapted from the EXIF specification.
        // Note that PhotoOrientation uses a counterclockwise convention,
        // while the EXIF spec uses a clockwise convention.
        "getOrientationString": function (photoOrientation) {
            switch (photoOrientation) {
                case Windows.Storage.FileProperties.PhotoOrientation.normal:
                    return "No rotation";
                case Windows.Storage.FileProperties.PhotoOrientation.flipHorizontal:
                    return "Flip horizontally";
                case Windows.Storage.FileProperties.PhotoOrientation.rotate180:
                    return "Rotate 180° clockwise";
                case Windows.Storage.FileProperties.PhotoOrientation.flipVertical:
                    return "Flip vertically";
                case Windows.Storage.FileProperties.PhotoOrientation.transpose:
                    return "Rotate 270° clockwise, then flip horizontally";
                case Windows.Storage.FileProperties.PhotoOrientation.rotate270:
                    return "Rotate 90° clockwise";
                case Windows.Storage.FileProperties.PhotoOrientation.transverse:
                    return "Rotate 90° clockwise, then flip horizontally";
                case Windows.Storage.FileProperties.PhotoOrientation.rotate90:
                    return "Rotate 270° clockwise";
                case Windows.Storage.FileProperties.PhotoOrientation.unspecified:
                default:
                    return "Unspecified";
            }
        },

        // Converts a Windows.Storage.FileProperties.PhotoOrientation value into a
        // Windows.Graphics.Imaging.BitmapRotation value.
        // For PhotoOrientation values reflecting a flip/mirroring operation, returns "None";
        // therefore this is a potentially lossy transformation.
        // Note that PhotoOrientation uses a counterclockwise convention,
        // while BitmapRotation uses a clockwise convention.
        "convertToBitmapRotation": function (photoOrientation) {
            switch (photoOrientation) {
                case Windows.Storage.FileProperties.PhotoOrientation.normal:
                    return Windows.Graphics.Imaging.BitmapRotation.none;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate270:
                    return Windows.Graphics.Imaging.BitmapRotation.clockwise90Degrees;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate180:
                    return Windows.Graphics.Imaging.BitmapRotation.clockwise180Degrees;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate90:
                    return Windows.Graphics.Imaging.BitmapRotation.clockwise270Degrees;
                default:
                    // Ignore any flip/mirrored values.
                    return BitmapRotation.none;
            }
        },

        // Converts a Windows.Graphics.Imaging.BitmapRotation value into a
        // Windows.Storage.FileProperties.PhotoOrientation value.
        // Note that PhotoOrientation uses a counterclockwise convention,
        // while BitmapRotation uses a clockwise convention.
        "convertToPhotoOrientation": function (bitmapRotation) {
            switch (bitmapRotation) {
                case Windows.Graphics.Imaging.BitmapRotation.none:
                    return Windows.Storage.FileProperties.PhotoOrientation.normal;
                case Windows.Graphics.Imaging.BitmapRotation.clockwise90Degrees:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate270;
                case Windows.Graphics.Imaging.BitmapRotation.clockwise180Degrees:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate180;
                case Windows.Graphics.Imaging.BitmapRotation.clockwise270Degrees:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate90;
                default:
                    return PhotoOrientation.normal;
            }
        },

        // "Adds" two PhotoOrientation values. For simplicity, does not handle any values with
        // flip/mirroring; therefore this is a potentially lossy transformation.
        // Note that PhotoOrientation uses a counterclockwise convention.
        "addPhotoOrientation": function (photoOrientation1, photoOrientation2) {
            switch (photoOrientation2) {
                case Windows.Storage.FileProperties.PhotoOrientation.rotate90:
                    return Helpers.add90DegreesCCW(photoOrientation1);
                case Windows.Storage.FileProperties.PhotoOrientation.rotate180:
                    return Helpers.add90DegreesCCW(Helpers.add90DegreesCCW(photoOrientation1));
                case Windows.Storage.FileProperties.PhotoOrientation.rotate270:
                    return Helpers.add90DegreesCW(photoOrientation1);
                case Windows.Storage.FileProperties.PhotoOrientation.normal:
                default:
                    // Ignore any values with flip/mirroring.
                    return photoOrientation1;
            }
        },

        // "Add" 90 degrees clockwise rotation to a PhotoOrientation value.
        // For simplicity, does not handle any values with flip/mirroring; therefore this is a potentially
        // lossy transformation.
        // Note that PhotoOrientation uses a counterclockwise convention.
        "add90DegreesCW": function (photoOrientation) {
            switch (photoOrientation) {
                case Windows.Storage.FileProperties.PhotoOrientation.normal:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate270;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate90:
                    return Windows.Storage.FileProperties.PhotoOrientation.normal;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate180:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate90;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate270:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate180;
                default:
                    // Ignore any values with flip/mirroring.
                    return Windows.Storage.FileProperties.PhotoOrientation.unspecified;
            }
        },

        // "Add" 90 degrees counter-clockwise rotation to a PhotoOrientation value.
        // For simplicity, does not handle any values with flip/mirroring; therefore this is a potentially
        // lossy transformation.
        // Note that PhotoOrientation uses a counterclockwise convention.
        "add90DegreesCCW": function (photoOrientation) {
            switch (photoOrientation) {
                case Windows.Storage.FileProperties.PhotoOrientation.normal:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate90;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate90:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate180;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate180:
                    return Windows.Storage.FileProperties.PhotoOrientation.rotate270;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate270:
                    return Windows.Storage.FileProperties.PhotoOrientation.normal;
                default:
                    // Ignore any values with flip/mirroring.
                    return Windows.Storage.FileProperties.PhotoOrientation.unspecified;
            }
        },

        // Modifies the style of an HTML element to reflect the specified PhotoOrientation.
        // Ignores any values with flip/mirroring.
        // Note that PhotoOrientation uses a counterclockwise convention.
        "applyRotationStyle": function (photoOrientation, htmlElement) {
            var style = htmlElement.style;
            switch (photoOrientation) {
                case Windows.Storage.FileProperties.PhotoOrientation.rotate270:
                    style.transform = "rotate(90deg)";
                    break;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate180:
                    style.transform = "rotate(180deg)";
                    break;
                case Windows.Storage.FileProperties.PhotoOrientation.rotate90:
                    style.transform = "rotate(270deg)";
                    break;
                case Windows.Storage.FileProperties.PhotoOrientation.normal:
                default:
                    // Ignore any values with flip/mirroring.
                    style.transform = "";
                    break;
            }
        }
    });
})();
