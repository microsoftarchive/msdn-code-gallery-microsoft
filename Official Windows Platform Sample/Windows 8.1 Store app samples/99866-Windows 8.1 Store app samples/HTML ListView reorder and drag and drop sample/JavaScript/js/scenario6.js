//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
        ready: function (element, options) {

            var dragging = false;
            sourceListView.addEventListener("itemdragstart", function (eventObject) {
                // Store the array of indices (indices) in the dataTransfer Object.
                var dragData = JSON.stringify({
                    indices: eventObject.detail.dragInfo.getIndices(),
                    sourceId: sourceListView.id,
                });
                eventObject.detail.dataTransfer.setData("Text", dragData);
                dragging = true;
            });

            sourceListView.addEventListener("itemdragend", function (eventObject) {
                dragging = false;
            });

            targetListView.addEventListener("itemdragenter", function (eventObject) {
                if (dragging && eventObject.detail.dataTransfer.types.contains("Text")) {
                    // Allow drop
                    eventObject.preventDefault();
                }
            });

            targetListView.addEventListener("itemdragdrop", function (eventObject) {
                var dragData = eventObject.detail.dataTransfer && JSON.parse(eventObject.detail.dataTransfer.getData("Text"));

                // Only handle drops with valid data:
                if (dragData && dragData.sourceId === sourceListView.id) {
                    var insertAfterIndex = eventObject.detail.insertAfterIndex,
                    indicesDragged = dragData.indices;

                    sourceListView.winControl.itemDataSource.itemFromIndex(indicesDragged[0]).then(function (item) {
                        // Return an item promise if the dragged item is part of selection, otherwise pass the single dragged item.
                        if (sourceListView.winControl.selection.getIndices().indexOf(indicesDragged[0]) !== -1) {
                            return sourceListView.winControl.selection.getItems();
                        } else {
                            return [item];
                        }
                    }).done(function (items) {
                        var ds = targetListView.winControl.itemDataSource;
                        if (insertAfterIndex >= 0) {
                            ds.itemFromIndex(insertAfterIndex).done(function (insertAfterItem) {
                                ds.beginEdits();
                                // Iterate backwards, since each insertAfter() call will shift items over.
                                for (var j = items.length - 1; j >= 0; j--) {
                                    ds.insertAfter(null, items[j].data, insertAfterItem.key);
                                }
                                ds.endEdits();
                            });

                        } else {
                            // When insertAfterIndex === -1, insert "after -1" at the start of the list.
                            ds.beginEdits();
                            for (var i = items.length - 1; i >= 0; i--) {
                                ds.insertAtStart(null, items[i].data);
                            }
                            ds.endEdits();
                        }
                    });
                }
            });
        }
    });
})();
