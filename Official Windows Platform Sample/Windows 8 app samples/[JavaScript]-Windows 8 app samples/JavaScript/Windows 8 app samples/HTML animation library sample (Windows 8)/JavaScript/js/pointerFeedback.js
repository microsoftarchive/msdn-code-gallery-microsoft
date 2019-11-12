//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/pointerFeedback.html", {
        ready: function (element, options) {
            target1.addEventListener("MSPointerDown", onPointerDown, false);
            target1.addEventListener("MSPointerUp", onPointerUp, false);
            target2.addEventListener("MSPointerDown", onPointerDown, false);
            target2.addEventListener("MSPointerUp", onPointerUp, false);
            target3.addEventListener("MSPointerDown", onPointerDown, false);
            target3.addEventListener("MSPointerUp", onPointerUp, false);
        }
    });

    function onPointerDown(evt) {
        WinJS.UI.Animation.pointerDown(evt.srcElement);
    }

    function onPointerUp(evt) {
        WinJS.UI.Animation.pointerUp(evt.srcElement);
    }
})();
