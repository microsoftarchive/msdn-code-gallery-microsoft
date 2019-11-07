//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    'use strict';

    var changeHitCount = 0;
    function changeHandler(event2) {
        WinJS.log && WinJS.log("fragDatePicker change fired: " + ++changeHitCount, "sample", "status");
    }

    function fragmentLoad(root) {
        // After the fragment is loaded into the target element,
        // CSS and JavaScript referenced in the fragment are loaded.
        // Then the loading code calls this method to process controls.
        WinJS.UI.processAll(root).then(function () {
            WinJS.Utilities.query('.fragDatePicker', root).control({
                onchange: changeHandler
            });
        });
    }

    WinJS.Namespace.define('FragmentScriptCSS_Fragment', {
        fragmentLoad: fragmentLoad,
    });
})();
