//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    WinJS.Namespace.define("Data", {
        items: new WinJS.Binding.List([
            { id: 1, description: "Item 1 description" },
            { id: 2, description: "Item 2 description" },
            { id: 3, description: "Item 3 description" },
            { id: 4, description: "Item 4 description" },
            { id: 5, description: "Item 5 description" },
            { id: 6, description: "Item 6 description" },
            { id: 7, description: "Item 7 description" },
            { id: 8, description: "Item 8 description" }
        ])
    });

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            
        }
    });
})();
