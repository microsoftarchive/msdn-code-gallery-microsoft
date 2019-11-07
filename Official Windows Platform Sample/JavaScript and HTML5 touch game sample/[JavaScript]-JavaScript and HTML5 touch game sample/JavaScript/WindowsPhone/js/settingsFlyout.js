//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
(function () {
    "use strict";

    function ready(element, options) {

        WinJS.UI.processAll(element)
            .done(function () {
                GameManager.game.getSettings();
            });
    }

    WinJS.UI.Pages.define("/html/settingsFlyout.html", {
        ready: ready,
    });
})();