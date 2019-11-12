//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {

            for (var i = 1; i <= 6; i++) {
                element.querySelector("#scen2-item" + i).addEventListener("dragstart", function (e) {
                    e.dataTransfer.effectAllowed = 'copy';
                    e.dataTransfer.setData('Text', this.id);
                });
            }

            var dropTarget = element.querySelector("#cone");
            dropTarget.addEventListener("dragover", function (eventObject) {
                //Allow drop
                eventObject.preventDefault();
            });

            dropTarget.addEventListener("dragenter", function (eventObject) {
                //Add Border
                WinJS.Utilities.addClass(dropTarget, "drop-ready");
            });

            dropTarget.addEventListener("dragleave", function (eventObject) {
                //Remove Border
                WinJS.Utilities.removeClass(dropTarget, "drop-ready");
            });

            dropTarget.addEventListener("drop", function (eventObject) {
                //Remove Border
                WinJS.Utilities.removeClass(dropTarget, "drop-ready");

                var itemId = eventObject.dataTransfer.getData('Text').charAt(10);

                var itemText = Scenario2.scen2Switch(itemId);
                var message = "You added " + itemText;

                WinJS.log && WinJS.log(message, "sample", "status");

            });

        }
    });

    var invokedHandler = WinJS.Utilities.markSupportedForProcessing(function (e) {

        var itemId = (e.srcElement.id).charAt(10);

        var itemText = Scenario2.scen2Switch(itemId);

        var message = "This is " + itemText;

        WinJS.log && WinJS.log(message, "sample", "status");
    });

    WinJS.Namespace.define("Scenario2", {
        invokedHandler: invokedHandler,

        scen2Switch: function (itemId) {

            var itemText;
            switch (parseInt(itemId)) {
                case 1:
                    itemText = "Caramel Sauce";
                    break;
                case 2:
                    itemText = "Chocolate Sauce";
                    break;
                case 3:
                    itemText = "Strawberry Sauce";
                    break;
                case 4:
                    itemText = "Chocolate Sprinkles";
                    break;
                case 5:
                    itemText = "Rainbow Sprinkles";
                    break;
                case 6:
                    itemText = "Vanilla Sprinkles";
                    break;
                default:
                    itemText = "No item, error";
                    break;
            }
            return itemText;
        }

    });

})();


