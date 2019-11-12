//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("Data", {
        samples2: new WinJS.Binding.List([
            { value: 5, description: "Task 1" },
            { value: 10, description: "Task 2" },
            { value: 25, description: "Task 3" },
            { value: 50, description: "Task 4" },
            { value: 75, description: "Task 5" },
            { value: 85, description: "Task 6" },
            { value: 100, description: "Task 7" }
        ])
    });

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
           
        }
    });

})();
