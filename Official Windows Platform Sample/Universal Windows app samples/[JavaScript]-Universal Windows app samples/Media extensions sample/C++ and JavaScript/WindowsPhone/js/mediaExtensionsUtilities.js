//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    function pickMediaFile(filters, videos, oneTimeOnly) {
        if (filters[0] === ".mp4") {
            videos.forEach(function (video, i) {
                video.src = "http://ie.microsoft.com/testdrive/Graphics/VideoFormatSupport/big_buck_bunny_trailer_480p_high.mp4";
            });
        } else {
            SdkSample.displayError("Unsupported filter: " + filters[0]);
        }
    }

    WinJS.Namespace.define("SdkSample", {
        pickMediaFile: pickMediaFile
    });
})();
