//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("Data", {
        samples3: new WinJS.Binding.List([
            { value: 5, description: "Task 1" },
            { value: 10, description: "Task 2" },
            { value: 25, description: "Task 3" },
            { value: 0, description: "Task 4" },
            { value: 75, description: "Task 5" },
            { value: 85, description: "Task 6" },
            { value: 100, description: "Task 7" }
        ])
    });


    /* Template used for the Repeater Control */
    function template(data) {
        var e = document.createElement("div");
        WinJS.Utilities.addClass(e, "bar");
        var label = document.createElement("label");
        WinJS.Utilities.addClass(label, "label");
        e.appendChild(label);
        label.textContent = data.description;

        var progress = document.createElement("progress");
        e.appendChild(progress);
        progress.max = 100;
        progress.value = data.value;

        if (data.value <= 0) {
            WinJS.Utilities.addClass(e, "disabled");
        }

        return e;
    }

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            var repeaterElement = element.querySelector(".graph");
            var repeater = new WinJS.UI.Repeater(repeaterElement, {
                data: Data.samples3,
                template: template
            });
        }
    });
})();
