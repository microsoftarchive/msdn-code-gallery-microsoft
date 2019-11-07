//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("Data", {
        samples4: new WinJS.Binding.List([
            { value: 5, description: "Task 1" },
            { value: 10, description: "Task 2" },
            { value: 25, description: "Task 3" },
            { value: 0, description: "Task 4" },
            { value: 75, description: "Task 5" },
            { value: 85, description: "Task 6" },
            { value: 100, description: "Task 7" }
        ])
    });

    function generateTask() {
        return { value: Math.floor((Math.random() * 100)), description: "Task " + (Data.samples4.length + 1) };
    }

    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            var add = element.querySelector(".addCmd");
            add.addEventListener("click", function (ev) {
                Data.samples4.push(generateTask());
            }.bind(this));

            var remove = element.querySelector(".removeCmd");
            remove.addEventListener("click", function (ev) {
                Data.samples4.pop();
            }.bind(this));

            var horizontal = element.querySelector(".horizontalCmd");
            horizontal.addEventListener("click", function (ev) {
                var graph = document.querySelector(".graphArea");
                WinJS.Utilities.removeClass(graph, "vertical");
                WinJS.Utilities.addClass(graph, "horizontal");
            }.bind(this));

            var vertical = element.querySelector(".verticalCmd");
            vertical.addEventListener("click", function (ev) {
                var graph = document.querySelector(".graphArea");
                WinJS.Utilities.removeClass(graph, "horizontal");
                WinJS.Utilities.addClass(graph, "vertical");
            }.bind(this));
        }
    });
})();