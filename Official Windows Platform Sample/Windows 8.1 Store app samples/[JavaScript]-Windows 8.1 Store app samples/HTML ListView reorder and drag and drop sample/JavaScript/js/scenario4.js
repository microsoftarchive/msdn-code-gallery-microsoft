//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {

            var dragging = false;
            myDragContent.addEventListener("dragstart", function (eventObject) {
                var dragData = { sourceElement: myDragContent.id, data: myItemTitle.innerText, imgSrc: myImg.src };
                eventObject.dataTransfer.setData("Text", JSON.stringify(dragData));
                dragging = true;
            });
            myDragContent.addEventListener("dragend", function (eventObject) {
                dragging = false;
            });

            var dropTarget = element.querySelector("#listView");
            listView.addEventListener("dragover", function (eventObject) {
                if (dragging) {
                    // Allow HTML5 drops
                    eventObject.preventDefault();
                }
            });

            listView.addEventListener("itemdragenter", function (eventObject) {
                if (dragging && eventObject.detail.dataTransfer.types.contains("Text")) {
                    WinJS.Utilities.addClass(dropTarget, "drop-ready");
                }
            });

            listView.addEventListener("itemdragleave", function (eventObject) {
                WinJS.Utilities.removeClass(dropTarget, "drop-ready");
            });

            listView.addEventListener("drop", function (eventObject) {
                WinJS.Utilities.removeClass(dropTarget, "drop-ready");
                var dragData = JSON.parse(eventObject.dataTransfer.getData("Text"));

                if (dragData && dragData.sourceElement === myDragContent.id) {
                    var newItemData = { title: dragData.data, text: ("id: " + dragData.sourceElement), picture: dragData.imgSrc };
                    var dropIndex = 0;
                    myData.splice(dropIndex, 0, newItemData);
                }
            });
        }
    });
})();
