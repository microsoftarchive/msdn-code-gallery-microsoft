//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario11.html", {
        ready: function (element, options) {
            document.getElementById("scenario11Show").addEventListener("click", show, false);
        }
    });

    function show() {
        var resourceNS = Windows.ApplicationModel.Resources.Core;
        var resourceMap = resourceNS.ResourceManager.current.mainResourceMap.getSubtree('Resources');

        var context = new resourceNS.ResourceContext();
        var languagesVector = new Array(document.getElementById('scenario11Select').value); // Context values can be a list
        context.languages = languagesVector;

        var str = resourceMap.getValue('scenario3Message', context).valueAsString;

        WinJS.log && WinJS.log(str, "sample", "status");
    }
})();
