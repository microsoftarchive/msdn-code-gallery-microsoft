//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("Scenario5", {
        currDropIndex: -1,
        dragging: false,
        listItemDragOverHandler: function (eventObject) {
            if (Scenario5.dragging) {
                eventObject.preventDefault();

                var targetIndex = listView.winControl.indexOfElement(eventObject.srcElement);
                var dropTarget = listView.winControl.elementFromIndex(targetIndex);

                if (!WinJS.Utilities.hasClass(dropTarget, "drop-ready")) {
                    WinJS.Utilities.addClass(dropTarget, "drop-ready");
                }
            }
        },

        listItemDropHandler: function (eventObject) {
            var dragData = JSON.parse(eventObject.dataTransfer.getData("Text"));
            if (dragData && dragData.sourceElement === myDragContent.id) {

                var newItemData = { title: dragData.data, text: ("id: " + dragData.sourceElement), picture: dragData.imgSrc };
                var targetItemIndex = listView.winControl.indexOfElement(eventObject.srcElement);
                WinJS.log && WinJS.log("You dropped \"" + newItemData.title + "\" on the item at index " + targetItemIndex, "sample", "status");

                var dropTarget = listView.winControl.elementFromIndex(targetItemIndex);
                if (WinJS.Utilities.hasClass(dropTarget, "drop-ready")) {
                    WinJS.Utilities.removeClass(dropTarget, "drop-ready");
                }
            }
        },

        listItemDragLeaveHandler: function (eventObject) {
            var targetIndex = listView.winControl.indexOfElement(eventObject.srcElement);
            var dropTarget = listView.winControl.elementFromIndex(targetIndex);

            WinJS.Utilities.removeClass(dropTarget, "drop-ready");
        }
    });

    WinJS.Utilities.markSupportedForProcessing(Scenario5.listItemDragOverHandler);
    WinJS.Utilities.markSupportedForProcessing(Scenario5.listItemDropHandler);
    WinJS.Utilities.markSupportedForProcessing(Scenario5.listItemDragLeaveHandler);

    var page = WinJS.UI.Pages.define("/html/scenario5.html", {

        ready: function (element, options) {
            Scenario5.myProperty = 5;
            myDragContent.addEventListener("dragstart", function (eventObject) {
                var dragData = { sourceElement: myDragContent.id, data: myItemTitle.innerText, imgSrc: myImg.src };
                eventObject.dataTransfer.setData("Text", JSON.stringify(dragData));
                Scenario5.dragging = true;
            });

            myDragContent.addEventListener("dragend", function (eventObject) {
                Scenario5.dragging = false;
            });
        }
    });

})();
