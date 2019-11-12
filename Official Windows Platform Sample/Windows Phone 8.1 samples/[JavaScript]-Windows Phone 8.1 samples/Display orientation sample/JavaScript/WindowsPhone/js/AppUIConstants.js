//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Constants specific to the Windows Phone version.
    WinJS.Namespace.define("Constants", {
        // On Windows the accelerometer returns values relative to Portrait orientation.
        // This sample always treats rotation as relative to Landscape regardless of platform.
        "UIAngleOffset": 180
    });
})();