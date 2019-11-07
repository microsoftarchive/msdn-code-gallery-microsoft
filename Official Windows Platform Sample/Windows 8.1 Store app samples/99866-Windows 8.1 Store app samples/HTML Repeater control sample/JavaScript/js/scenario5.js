//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("Data", {
        samples5: new WinJS.Binding.List([
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
        return { value: Math.floor((Math.random() * 100)), description: "Task " + (Data.samples5.length + 1) };
    }

    var animation = false;

    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        ready: function (element, options) {

            // Register for insert and removal events to trigger animations
            var repeaterElement = element.querySelector(".graphData");
            var repeater = repeaterElement.winControl;
            repeater.addEventListener("iteminserted", function (ev) {
                var a = WinJS.UI.Animation.createAddToListAnimation(ev.affectedElement);
                a.execute().then(function () {
                    animation = false;
                });;
            }.bind(this));
            repeater.addEventListener("itemremoved", function (ev) {
                var affectedElement = ev.affectedElement;
                repeaterElement.appendChild(affectedElement);
                var a = WinJS.UI.Animation.createDeleteFromListAnimation(affectedElement);

                this.affectedElement = affectedElement;
                var that = this;

                // Return a promise to the Repeater so that I can dispose the item after we have animated it
                // and removed it from the DOM
                ev.setPromise(a.execute().then(function () {
                    repeaterElement.removeChild(that.affectedElement);
                    animation = false;
                }));
            }.bind(this));


            var add = element.querySelector(".addCmd");
            add.addEventListener("click", function (ev) {
                if (!animation) {
                    animation = true;
                    Data.samples5.push(generateTask());
                }
            }.bind(this));

            var remove = element.querySelector(".removeCmd");
            remove.addEventListener("click", function (ev) {
                if (!animation && Data.samples5.length > 0) {
                    animation = true;
                    Data.samples5.pop();
                }
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