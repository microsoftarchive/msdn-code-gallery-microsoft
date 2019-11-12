//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/soundrecorder/soundrecorder.html", {

        processed: function (element, options) {
            this.soundRecorder = new Application.Capture.SoundRecorder();

            element.querySelector(".controls .record").onclick = this.toggleRecording;
            element.querySelector(".controls .play").onclick = this.togglePlayback;
            element.querySelector(".controls .save").onclick = this.save;

            // Bind the UI to the recorder and initialize the device
            return WinJS.Binding.processAll(element, this.soundRecorder);
        },

        ready: function (element, options) {
            this.soundRecorder.initialize(element.querySelector(".controls audio"));
        },

        // Start or stop recording
        toggleRecording: function () {
            if (Application.navigator.pageControl.soundRecorder.state === Application.Capture.SoundRecorderStates.recording) {
                Application.navigator.pageControl.soundRecorder.stop();
            } else {
                Application.navigator.pageControl.soundRecorder.start();
            }
        },

        // Toggle playback of the captured audio 
        togglePlayback: function () {
            if (Application.navigator.pageControl.soundRecorder.state === Application.Capture.SoundRecorderStates.playing) {
                Application.navigator.pageControl.soundRecorder.pause();
            } else {
                Application.navigator.pageControl.soundRecorder.play();
            }
        },

        // Save the captured audio
        save: function () {
            Application.navigator.pageControl.soundRecorder.saveAsync().done(function () {
                (new Application.Data.Folder({ folder: Windows.Storage.KnownFolders.musicLibrary })).invoke();
            });
        },

        // Dispose the sound recorder when unloading
        unload: function () {
            this.soundRecorder.dispose();
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

        soundRecorder: null

    });
})();
