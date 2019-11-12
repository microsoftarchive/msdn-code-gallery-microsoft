//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var data = [{ url: '/images/squareTile-sdk.png' }, { url: '/images/squareTile-sdk.png' }, { url: '/images/squareTile-sdk.png' }];

    function ready(element, options) {
        // Set up listView
        WinJS.UI.process(element).then(function () {
           var listView = element.querySelector("#listView").winControl;
           var itemTemplate = element.querySelector(".itemTemplate");
            WinJS.UI.setOptions(listView, {
                itemTemplate: itemTemplate,
                itemDataSource: new WinJS.Binding.List(data).dataSource
            });
        });
    }

    WinJS.UI.Pages.define("/html/elements.html", {
        ready: ready
    });

})();
