//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var output;

    var page = WinJS.UI.Pages.define("/html/scenario9.html", {
        ready: function (element, options) {
            output = document.getElementById('output');
            document.getElementById("scenario9Show").addEventListener("click", show, false);
        }
    });

    function show() {
        WinJS.Resources.processAll(output);
        output.querySelector('button').style.display = '';
    }
})();
