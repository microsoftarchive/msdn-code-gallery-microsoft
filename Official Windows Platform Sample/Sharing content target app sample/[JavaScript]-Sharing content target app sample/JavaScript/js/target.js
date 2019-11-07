//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Variable to store the ShareOperation object
    var shareOperation = null;

    // Variable to store the visibility of the Extended Sharing section
    var extendedSharingCollapsed = true;

    // Variable to store the custom format string
    var customFormatName = "http://schema.org/Book";

    /// <summary>
    /// Helper function to display received sharing content
    /// </summary>
    /// <param name="type">
    /// The type of content received
    /// </param>
    /// <param name="value">
    /// The value of the content
    /// </param>
    function displayContent(label, content, preformatted) {
        var labelNode = document.createElement("strong");
        labelNode.innerText = label;

        document.getElementById("contentValue").appendChild(labelNode);

        if (preformatted) {
            var pre = document.createElement("pre");
            pre.innerHTML = toStaticHTML(content);
            document.getElementById("contentValue").appendChild(pre);
        }
        else {
            document.getElementById("contentValue").appendChild(document.createTextNode(content));
        }
        document.getElementById("contentValue").appendChild(document.createElement("br"));
    }

    function displayError(label, errorString) {
        return displayContent(label, errorString);
    }

    /// <summary>
    /// Handler executed on activation of the target
    /// </summary>
    /// <param name="eventArgs">
    /// Arguments of the event. In the case of the Share contract, it has the ShareOperation
    /// </param>
    function activatedHandler(eventObject) {
        // In this sample we only do something if it was activated with the Share contract
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.shareTarget) {
            eventObject.setPromise(WinJS.UI.processAll());

            // We receive the ShareOperation object as part of the eventArgs
            shareOperation = eventObject.detail.shareOperation;

            // We queue an asychronous event so that working with the ShareOperation object does not
            // block or delay the return of the activation handler.
            WinJS.Application.queueEvent({ type: "shareready" });
        }
    }

    /// <summary>
    /// Handler executed when ready to share; handling the share operation should be performed
    /// outside the activation handler
    /// </summary>
    function shareReady(eventArgs) {
        document.getElementById("title").innerText = shareOperation.data.properties.title;
        document.getElementById("description").innerText = shareOperation.data.properties.description;
        document.getElementById("packageFamilyName").innerText = shareOperation.data.properties.packageFamilyName;
        if (shareOperation.data.properties.contentSourceWebLink) {
            document.getElementById("contentSourceWebLink").innerText = shareOperation.data.properties.contentSourceWebLink.rawUri;
        }
        if (shareOperation.data.properties.contentSourceApplicationLink) {
            document.getElementById("contentSourceApplicationLink").innerText = shareOperation.data.properties.contentSourceApplicationLink.rawUri;
        }
        if (shareOperation.data.properties.square30x30Logo) {
            var backgroundColor = shareOperation.data.properties.logoBackgroundColor;
            document.getElementById("logoBackground").style.backgroundColor = "rgba(" + backgroundColor.r + "," + backgroundColor.g + "," + backgroundColor.b + "," + backgroundColor.a + ")";

            shareOperation.data.properties.square30x30Logo.openReadAsync().done(function (logoStream) {
                document.getElementById("logoHolder").src = URL.createObjectURL(logoStream, { oneTimeOnly: true });
                document.getElementById("logoArea").className = "unhidden";
            }, function (e) {
                displayError("Logo: ", "Error reading image stream:  " + e);
            });
        }
        // If this app was activated via a QuickLink, display the QuickLinkId
        if (shareOperation.quickLinkId !== "") {
            document.getElementById("selectedQuickLinkId").innerText = shareOperation.quickLinkId;
            document.getElementById("quickLinkArea").className = "hidden";
        }
        // Display a thumbnail if available
        if (shareOperation.data.properties.thumbnail) {
            shareOperation.data.properties.thumbnail.openReadAsync().done(function (thumbnailStream) {
                document.getElementById("thumbnailImage").src = URL.createObjectURL(thumbnailStream, { oneTimeOnly: true });
                document.getElementById("thumbnailArea").className = "unhidden";
            }, function (e) {
                displayError("Thumbnail: ", "Error reading image stream:  " + e);
            });
        }
        // Display the data received based on data type
        if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.text)) {
            shareOperation.data.getTextAsync().done(function (text) {
                displayContent("Text: ", text, false);
            }, function (e) {
                displayError("Text: ", "Error retrieving data: " + e);
            });
        }
        if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.webLink)) {
            shareOperation.data.getWebLinkAsync().done(function (webLink) {
                displayContent("WebLink: ", webLink.rawUri, false);
            }, function (e) {
                displayError("WebLink: ", "Error retrieving data: " + e);
            });
        }
        if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.applicationLink)) {
            shareOperation.data.getApplicationLinkAsync().done(function (applicationLink) {
                displayContent("ApplicationLink: ", applicationLink.rawUri, false);
            }, function (e) {
                displayError("ApplicationLink: ", "Error retrieving data: " + e);
            });
        }
        if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.storageItems)) {
            shareOperation.data.getStorageItemsAsync().done(function (storageItems) {
                var fileList = "";
                for (var i = 0; i < storageItems.size; i++) {
                    fileList += storageItems.getAt(i).name;
                    if (i < storageItems.size - 1) {
                        fileList += ", ";
                    }
                }
                displayContent("StorageItems: ", fileList, false);
            }, function (e) {
                displayError("StorageItems: ", "Error retrieving data: " + e);
            });
        }
        if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.bitmap)) {
            shareOperation.data.getBitmapAsync().done(function (bitmapStreamReference) {
                bitmapStreamReference.openReadAsync().done(function (bitmapStream) {
                    if (bitmapStream) {
                        document.getElementById("imageHolder").src = URL.createObjectURL(bitmapStream, { oneTimeOnly: true });
                        document.getElementById("imageArea").className = "unhidden";
                    }
                }, function (e) {
                    displayError("Bitmap: ", "Error reading image stream:  " + e);
                });
            }, function (e) {
                displayError("Bitmap: ", "Error retrieving data: " + e);
            });
        }

        if (shareOperation.data.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.html)) {
            shareOperation.data.getHtmlFormatAsync().done(function (htmlFormat) {
                document.getElementById("htmlContentArea").className = "unhidden";

                // Extract the HTML fragment from the HTML format
                var htmlFragment = Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.getStaticFragment(htmlFormat);

                // Set the innerHTML of the iframe to the HTML fragment
                var iFrame = document.getElementById("htmlContent");
                iFrame.style.display = "";
                iFrame.contentDocument.documentElement.innerHTML = htmlFragment;

                // Now we loop through any images and use the resourceMap to map each image element's src
                var images = iFrame.contentDocument.documentElement.getElementsByTagName("img");
                if (images.length > 0) {
                    shareOperation.data.getResourceMapAsync().done(function (resourceMap) {
                        if (resourceMap.size > 0) {
                            for (var i = 0, len = images.length; i < len; i++) {
                                var streamReference = resourceMap[images[i].getAttribute("src")];
                                if (streamReference) {
                                    // Call a helper function to map the image element's src to a corresponding blob URL generated from the streamReference
                                    setResourceMapURL(streamReference, images[i]);
                                }
                            }
                        }
                    }, function (e) {
                        displayError("Resource Map: ", "Error retrieving data:  " + e);
                    });
                }
            }, function (e) {
                displayError("HTML Format: ", "Error retrieving data: " + e);
            });
        }

        if (shareOperation.data.contains(customFormatName)) {
            shareOperation.data.getTextAsync(customFormatName).done(function (customFormatString) {
                var customFormatObject = JSON.parse(customFormatString);
                if (customFormatObject) {
                    // This sample expects the custom format to be of type http://schema.org/Book
                    if (customFormatObject.type === "http://schema.org/Book") {
                        customFormatString = "Type: " + customFormatObject.type;
                        if (customFormatObject.properties) {
                            customFormatString += "\nImage: " + customFormatObject.properties.image
                                                + "\nName: " + customFormatObject.properties.name
                                                + "\nBook Format: " + customFormatObject.properties.bookFormat
                                                + "\nAuthor: " + customFormatObject.properties.author
                                                + "\nNumber of Pages: " + customFormatObject.properties.numberOfPages
                                                + "\nPublisher: " + customFormatObject.properties.publisher
                                                + "\nDate Published: " + customFormatObject.properties.datePublished
                                                + "\nIn Language: " + customFormatObject.properties.inLanguage
                                                + "\nISBN: " + customFormatObject.properties.isbn;
                        }
                    }
                }
                displayContent("Custom data: ", customFormatString, true);
            }, function (e) {
                displayError("Custom data: ", "Error retrieving data: " + e);
            });
        }
    }

    /// <summary>
    /// Sets the blob URL for an image element based on a reference to an image stream within a resource map
    /// </summary>
    function setResourceMapURL(streamReference, imageElement) {
        if (streamReference) {
            streamReference.openReadAsync().done(function (imageStream) {
                if (imageStream) {
                    imageElement.src = URL.createObjectURL(imageStream, { oneTimeOnly: true });
                }
            }, function (e) {
                imageElement.alt = "Failed to load";
            });
        }
    }

    /// <summary>
    /// Use to simulate that an extended share operation has started
    /// </summary>
    function reportStarted() {
        shareOperation.reportStarted();
    }

    /// <summary>
    /// Use to simulate that an extended share operation has retrieved the data
    /// </summary>
    function reportDataRetrieved() {
        shareOperation.reportDataRetrieved();
    }

    /// <summary>
    /// Use to simulate that an extended share operation has reached the status "submittedToBackgroundManager"
    /// </summary>
    function reportSubmittedBackgroundTask() {
        shareOperation.reportSubmittedBackgroundTask();
    }

    /// <summary>
    /// Submit for extended share operations. Can either report success or failure, and in case of success, can add a quicklink.
    /// This does NOT take care of all the prerequisites (such as calling reportExtendedShareStatus(started)) before submitting.
    /// </summary>
    function reportError() {
        var errorText = document.getElementById("extendedShareErrorMessage").value;
        shareOperation.reportError(errorText);
    }

    /// <summary>
    /// Call the reportCompleted API with the proper quicklink (if needed)
    /// </summary>
    function reportCompleted() {
        var addQuickLink = document.getElementById("addQuickLink").checked;
        if (addQuickLink) {
            var el;
            var quickLink = new Windows.ApplicationModel.DataTransfer.ShareTarget.QuickLink();

            var quickLinkId = document.getElementById("quickLinkId").value;
            if ((typeof quickLinkId !== "string") || (quickLinkId === "")) {
                el = document.getElementById("quickLinkError");
                el.className = "unhidden";
                el.innerText = "Missing QuickLink ID";
                return;
            }
            quickLink.id = quickLinkId;

            var quickLinkTitle = document.getElementById("quickLinkTitle").value;
            if ((typeof quickLinkTitle !== "string") || (quickLinkTitle === "")) {
                el = document.getElementById("quickLinkError");
                el.className = "unhidden";
                el.innerText = "Missing QuickLink title";
                return;
            }
            quickLink.title = quickLinkTitle;

            // For quicklinks, the supported FileTypes and DataFormats are set independently from the manifest
            var dataFormats = Windows.ApplicationModel.DataTransfer.StandardDataFormats;
            quickLink.supportedFileTypes.replaceAll(["*"]);
            quickLink.supportedDataFormats.replaceAll([dataFormats.text, dataFormats.webLink, dataFormats.applicationLink, dataFormats.bitmap, dataFormats.storageItems, dataFormats.html, customFormatName]);

            // Prepare the icon for a QuickLink
            Windows.ApplicationModel.Package.current.installedLocation.getFileAsync("images\\user.png").done(function (iconFile) {
                quickLink.thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.createFromFile(iconFile);
                shareOperation.reportCompleted(quickLink);
            }, function (e) {
                // Even if the QuickLink cannot be created it is important to call ReportCompleted. Otherwise, if this is a long-running share,
                // the app will stick around in the long-running share progress list.
                shareOperation.reportCompleted();
                throw e;
            });
        } else {
            shareOperation.reportCompleted();
        }
    }

    /// <summary>
    /// Calls the share operation's dismiss UI function.
    /// </summary>
    function dismissUI() {
        shareOperation.dismissUI();
    }

    /// <summary>
    /// Expand/collapse the Extended Sharing div
    /// </summary>
    function expandoClick() {
        if (extendedSharingCollapsed) {
            document.getElementById("extendedSharing").className = "unhidden";
            // Set expando glyph to up arrow
            document.getElementById("expandoGlyph").innerHTML = "&#57360;";
            extendedSharingCollapsed = false;
        } else {
            document.getElementById("extendedSharing").className = "hidden";
            // Set expando glyph to down arrow
            document.getElementById("expandoGlyph").innerHTML = "&#57361;";
            extendedSharingCollapsed = true;
        }
    }

    /// <summary>
    /// Expand/collapse the QuickLink fields
    /// </summary>
    function addQuickLinkChanged() {
        if (document.getElementById("addQuickLink").checked) {
            quickLinkFields.className = "unhidden";
        } else {
            quickLinkFields.className = "hidden";
            document.getElementById("quickLinkError").className = "hidden";
        }
    }

    // Initialize the activation handler
    WinJS.Application.addEventListener("activated", activatedHandler, false);
    WinJS.Application.addEventListener("shareready", shareReady, false);
    WinJS.Application.start();

    function initialize() {
        document.getElementById("addQuickLink").addEventListener("change", addQuickLinkChanged, false);
        document.getElementById("reportCompleted").addEventListener("click", reportCompleted, false);
        document.getElementById("dismissUI").addEventListener("click", dismissUI, false);
        document.getElementById("expandoClick").addEventListener("click", expandoClick, false);
        document.getElementById("reportStarted").addEventListener("click", reportStarted, false);
        document.getElementById("reportDataRetrieved").addEventListener("click", reportDataRetrieved, false);
        document.getElementById("reportSubmittedBackgroundTask").addEventListener("click", reportSubmittedBackgroundTask, false);
        document.getElementById("reportError").addEventListener("click", reportError, false);
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
})();