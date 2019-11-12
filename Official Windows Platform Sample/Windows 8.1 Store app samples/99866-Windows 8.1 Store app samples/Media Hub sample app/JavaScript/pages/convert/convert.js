//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var transcoding = Application.Transcoding;
    var storage = Windows.Storage;

    WinJS.UI.Pages.define("/pages/convert/convert.html", {

        processed: function (element, options) {
            element.querySelector(".controls .open").onclick = this.openFile;
            element.querySelector(".controls .save").onclick = this.saveFile;

            return WinJS.Binding.processAll(element, transcoding.transcoder);
        },

        ready: function(element, options){
            transcoding.transcoder.bind("state", this._transcoderStateChanged);
            this._transcoderStateChanged();
        },

        // Opens a media file using a picker and sets it in the transcoder
        openFile: function () {
            var picker = new storage.Pickers.FileOpenPicker();
            picker.fileTypeFilter.replaceAll(["*"]);
            picker.pickSingleFileAsync().done(function (file) {
                if (file) {
                    transcoding.transcoder.source = file;
                }
            });
        },

        // Saves a media file using the transcoder
        saveFile: function () {
            // Get the container value from the proper select element depending on the source type
            var container = Application.navigator.pageElement.querySelector(".controls ." +
                (transcoding.transcoder.state === transcoding.TranscoderStates.videoSource ? "video" : "audio") +
                "containervalue").value;

            // Get the profile value from the proper select element depending on the source type
            var profile = Application.navigator.pageElement.querySelector(".controls ." +
                (transcoding.transcoder.state === transcoding.TranscoderStates.videoSource ? "video" : "audio") +
                "profilevalue").value;
            
            // Use the file save picker to get the output and start transcoding
            var picker = new storage.Pickers.FileSavePicker();
            picker.fileTypeChoices.insert(container, ["." + container.toLowerCase()]);
            picker.pickSaveFileAsync().done(function (file) {
                if (file) {
                    transcoding.transcoder.startTranscodeAsync(file, container, profile);
                }
            });
        },

        // Unload the page
        unload: function () {
            transcoding.transcoder.unbind("state", this._transcoderStateChanged);
        },

        // reloads app resources in the page
        updateResources: function (element, e) {
            // Called by _contextChanged event handler in navigator.js when a resource 
            // qualifier (language, scale, contrast, etc.) has changed. The element 
            // passed is the root of this page. 
            //
            // Since this sample app currently doesn't have any assets with variants
            // for scale/contrast/language/etc., the lines below that do the actual 
            // work are commented out. This is provided here to model how to handle 
            // scale or other resource context changes if this app were expanded to 
            // include resources with assets for such variantions.

            // Will filter for changes to specific qualifiers.
            if (e.detail.qualifier === "Scale" || e.detail.qualifier === "Contrast") {
                // if there are string resources bound to properties using data-win-res,
                // the following line will update those properties: 

                //WinJS.Resources.processAll(element);

                // Background images from the app package with variants for scale, etc. 
                // are automatically reloaded by the platform when a resource context 
                // qualifier has changed. That is not done, however, for img elements. 
                // The following will make sure those are updated:

                //var imageElements = document.getElementsByTagName("img");
                //for (var i = 0, l = imageElements.length; i < l; i++) {
                //    var previousSource = imageElements[i].src;
                //    var uri = new Windows.Foundation.Uri(document.location, previousSource);
                //    if (uri.schemeName === "ms-appx") {
                //        imageElements[i].src = "";
                //        imageElements[i].src = previousSource;
                //    }
                //}
            }

        },

        // Disable the output format controls and save button if the transcoder is not ready
        _transcoderStateChanged: function () {
            var disableOutputControls =
                transcoding.transcoder.state === transcoding.TranscoderStates.empty ||
                transcoding.transcoder.state === transcoding.TranscoderStates.unavailable;

            document.querySelector(".convertpage section[role=main] .controls .save").disabled = disableOutputControls;
            Array.prototype.forEach.call(document.querySelectorAll(".convertpage section[role=main] .controls .rows select"), function (select) {
                select.disabled = disableOutputControls;
            });
        }
    });
})();
