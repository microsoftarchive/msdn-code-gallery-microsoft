//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {

            var dragging = false;
            myDragContent.addEventListener("dragstart", function (eventObject) {
                var dragData = { sourceId: myDragContent.id, data: myItemTitle.innerText, imgSrc: myImg.src };
                eventObject.dataTransfer.setData("Text", JSON.stringify(dragData));
                dragging = true;
            });
            myDragContent.addEventListener("dragend", function (eventObject) {
                dragging = false;
            });

            listView.addEventListener("itemdragenter", function (eventObject) {
                if (dragging && eventObject.detail.dataTransfer.types.contains("Text")) {
                    // Allow drop
                    eventObject.preventDefault();
                }
            });

            listView.addEventListener("itemdragdrop", function (eventObject) {
                var dragData = eventObject.detail.dataTransfer && JSON.parse(eventObject.detail.dataTransfer.getData("Text"));
                if (dragData && dragData.sourceId === myDragContent.id) {
                    var newItemData = { title: dragData.data, text: ("Source id: " + dragData.sourceId), picture: dragData.imgSrc };
                    // insertAfterIndex tells us where in the list to add the new item.
                    // If we're inserting at the start, insertAfterIndex is -1. 
                    // Adding 1 to insertAfterIndex gives us the nominal index in the array to insert the new item.
                    myData.splice(eventObject.detail.insertAfterIndex + 1, 0, newItemData);
                }
            });
        }
    });
})();
