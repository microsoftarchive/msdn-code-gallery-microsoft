//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    window.disableUnsupportedTemplates = function disableUnsupportedTemplates_windows() {
        // Not all templates are available on Windows - disable the ones that are WindowsPhone-exclusive.
        document.getElementById("TileSquare71x71IconWithBadge").disabled = "disabled";
        document.getElementById("TileSquare150x150IconWithBadge").disabled = "disabled";
        document.getElementById("TileWide310x150IconWithBadgeAndText").disabled = "disabled";
        document.getElementById("TileSquare71x71Image").disabled = "disabled";
    };

    window.disableUnsupportedGlyphs = function disableUnsupportedGlyphs_windows() {
        // All glyphs are supported on Windows, so this function does nothing.
    };
})();