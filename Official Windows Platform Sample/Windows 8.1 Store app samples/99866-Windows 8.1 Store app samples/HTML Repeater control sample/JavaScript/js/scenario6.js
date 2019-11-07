//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("Data", {
        samples6: new WinJS.Binding.List([
            { value: 5, description: "Task 1" },
            {
                value: 50,
                description: "Task 2",
                subTasks: new WinJS.Binding.List([
                    { value: 50, description: "Task 2: Part 1" },
                    { value: 50, description: "Task 2: Part 2" }
                ])
            },
            { value: 25, description: "Task 3" },
            { value: 0, description: "Task 4" },
            { value: 75, description: "Task 5" },
            {
                value: 33,
                description: "Task 6",
                subTasks: new WinJS.Binding.List([
                    { value: 50, description: "Task 6: Part 1" },
                    { value: 25, description: "Task 6: Part 2" },
                    { value: 25, description: "Task 6: Part 3" }
                ])
            },
            { value: 100, description: "Task 7" }
        ])
    });

    function generateTask() {

        // Create just one task
        var subTasks = new WinJS.Binding.List([]);
        var taskValue = Math.floor((Math.random() * 100));
        var taskDescription = "Task " + (Data.samples6.length + 1);

        // Randomly make sub tasks
        if (Math.floor(Math.random() * 2) < 1) {
            var numberOfTasks = Math.floor(Math.random() * 4);
            var totalValue = 0;
            for (var i = 0; i < numberOfTasks; i++) {
                var subValue = Math.floor((Math.random() * 100));
                subTasks.push({
                    value: subValue,
                    description: taskDescription + ": Part " + (i + 1)
                });
                totalValue += subValue;
            }

            taskValue = totalValue / numberOfTasks;
        }

        return { value: taskValue, description: taskDescription, subTasks: subTasks };
    }

    var animation = false;

    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
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
                event.setPromise(a.execute().then(function () {
                    repeaterElement.removeChild(that.affectedElement);
                    animation = false;
                }));
            }.bind(this));


            var add = element.querySelector(".addCmd");
            add.addEventListener("click", function (ev) {
                if (!animation) {
                    animation = true;
                    Data.samples6.push(generateTask());
                }
            }.bind(this));

            var remove = element.querySelector(".removeCmd");
            remove.addEventListener("click", function (ev) {
                if (!animation && Data.samples6.length > 0) {
                    animation = true;
                    Data.samples6.pop();
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