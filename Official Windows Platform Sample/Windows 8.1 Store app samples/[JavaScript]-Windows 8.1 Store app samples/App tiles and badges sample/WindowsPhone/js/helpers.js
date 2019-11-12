//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    window.disableUnsupportedTemplates = function disableUnsupportedTemplates_windowsPhone() {
        // Not all templates are available on WindowsPhone - disable the ones that are Windows-exclusive.
        document.getElementById("TileWide310x150Text02").disabled = "disabled";
        document.getElementById("TileWide310x150Text06").disabled = "disabled";
        document.getElementById("TileWide310x150Text07").disabled = "disabled";
        document.getElementById("TileWide310x150Text08").disabled = "disabled";
        document.getElementById("TileWide310x150Text10").disabled = "disabled";
        document.getElementById("TileWide310x150Text11").disabled = "disabled";
        document.getElementById("TileSquare310x310BlockAndText01").disabled = "disabled";
        document.getElementById("TileSquare310x310BlockAndText02").disabled = "disabled";
        document.getElementById("TileSquare310x310Image").disabled = "disabled";
        document.getElementById("TileSquare310x310ImageAndText01").disabled = "disabled";
        document.getElementById("TileSquare310x310ImageAndText02").disabled = "disabled";
        document.getElementById("TileSquare310x310ImageAndTextOverlay01").disabled = "disabled";
        document.getElementById("TileSquare310x310ImageAndTextOverlay02").disabled = "disabled";
        document.getElementById("TileSquare310x310ImageAndTextOverlay03").disabled = "disabled";
        document.getElementById("TileSquare310x310ImageCollectionAndText01").disabled = "disabled";
        document.getElementById("TileSquare310x310ImageCollectionAndText02").disabled = "disabled";
        document.getElementById("TileSquare310x310ImageCollection").disabled = "disabled";
        document.getElementById("TileSquare310x310SmallImagesAndTextList01").disabled = "disabled";
        document.getElementById("TileSquare310x310SmallImagesAndTextList02").disabled = "disabled";
        document.getElementById("TileSquare310x310SmallImagesAndTextList03").disabled = "disabled";
        document.getElementById("TileSquare310x310SmallImagesAndTextList04").disabled = "disabled";
        document.getElementById("TileSquare310x310Text01").disabled = "disabled";
        document.getElementById("TileSquare310x310Text02").disabled = "disabled";
        document.getElementById("TileSquare310x310Text03").disabled = "disabled";
        document.getElementById("TileSquare310x310Text04").disabled = "disabled";
        document.getElementById("TileSquare310x310Text05").disabled = "disabled";
        document.getElementById("TileSquare310x310Text06").disabled = "disabled";
        document.getElementById("TileSquare310x310Text07").disabled = "disabled";
        document.getElementById("TileSquare310x310Text08").disabled = "disabled";
        document.getElementById("TileSquare310x310TextList01").disabled = "disabled";
        document.getElementById("TileSquare310x310TextList02").disabled = "disabled";
        document.getElementById("TileSquare310x310TextList03").disabled = "disabled";
        document.getElementById("TileSquare310x310SmallImageAndText01").disabled = "disabled";
        document.getElementById("TileSquare310x310SmallImagesAndTextList05").disabled = "disabled";
        document.getElementById("TileSquare310x310Text09").disabled = "disabled";
    };

    window.disableUnsupportedGlyphs = function disableUnsupportedGlyphs_windowsPhone() {
        // Not all glyphs are available on WindowsPhone - disable the ones that are Windows-exclusive.
        document.getElementById("activity").disabled = "disabled";
        document.getElementById("available").disabled = "disabled";
        document.getElementById("away").disabled = "disabled";
        document.getElementById("busy").disabled = "disabled";
        document.getElementById("newMessage").disabled = "disabled";
        document.getElementById("paused").disabled = "disabled";
        document.getElementById("playing").disabled = "disabled";
        document.getElementById("unavailable").disabled = "disabled";
        document.getElementById("error").disabled = "disabled";
        document.getElementById("alarm").disabled = "disabled";
    };
})();