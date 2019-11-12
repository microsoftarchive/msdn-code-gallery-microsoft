//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="..\base-sdk.js" />
/// <reference path="..\WinJS\js\base.js" />


(function () {
    "use strict";

    function localDecoderOpen() {
        // Set video element's source
        WinJS.Utilities.query("#localDecoderVideo")[0].src = "videos/video.mpg";
    }

    function localDecoderStop() {
        var vid = WinJS.Utilities.query("#localDecoderVideo")[0];
        vid.suppressErrors = true;
        vid.src = null;
    }

    var page = WinJS.UI.Pages.define("/html/LocalDecoder.html", {
        extensions: null,
        MFVideoFormat_MPG1: { value: "{3147504d-0000-0010-8000-00aa00389b71}" },
        NULL_GUID: { value: "{00000000-0000-0000-0000-000000000000}" },

        ready: function (element, options) {
            if (!this.extensions) {
                // Add any initialization code here
                this.extensions = new Windows.Media.MediaExtensionManager();
                // Register custom ByteStreamHandler and custom decoder.
                this.extensions.registerByteStreamHandler("MPEG1Source.MPEG1ByteStreamHandler", ".mpg", null);
                this.extensions.registerVideoDecoder("MPEG1Decoder.MPEG1Decoder", this.MFVideoFormat_MPG1, this.NULL_GUID);
            }

            WinJS.Utilities.query("#localDecoderOpen").listen("click", localDecoderOpen);
            WinJS.Utilities.query("#localDecoderStop").listen("click", localDecoderStop);
            WinJS.Utilities.query("#localDecoderVideo").listen("error", SdkSample.videoOnError);
        },

        unload: function () {
            localDecoderStop();
        }
    });
})();
