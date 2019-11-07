//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Constants specific to the Windows Store version.
    WinJS.Namespace.define("Constants", {
        // On Windows the accelerometer returns values relative to Landscape orientation,
        // even if the device has a native orientation of Portrait.
        // This sample always treats rotation as relative to Landscape regardless of platform.
        "UIAngleOffset": 270
    });
})();