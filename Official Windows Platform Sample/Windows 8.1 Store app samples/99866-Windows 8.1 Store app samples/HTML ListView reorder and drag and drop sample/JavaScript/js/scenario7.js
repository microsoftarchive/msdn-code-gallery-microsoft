//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("Scenario7", {
        itemDataSource: null,
        groupDataSource: null
    });
    var myGroupedList = GroupedData.initData();
    Scenario7.itemDataSource = myGroupedList.dataSource;
    Scenario7.groupDataSource = myGroupedList.groups.dataSource;

    var page = WinJS.UI.Pages.define("/html/scenario7.html", {
        ready: function (element, options) {

            myGroupedList = GroupedData.initData();
            Scenario7.itemDataSource = myGroupedList.dataSource;
            Scenario7.groupDataSource = myGroupedList.groups.dataSource;

            listView.addEventListener("itemdragstart", function (eventObject) {
                // Store the array of indices in the dataTransfer Object
                var dragData = JSON.stringify({
                    indices: eventObject.detail.dragInfo.getIndices(),
                    sourceId: listView.id,
                });
                eventObject.detail.dataTransfer.setData("Text", dragData);
            });

            listView.addEventListener("itemdragdrop", function (eventObject) {
                var usingKeyboard = (eventObject.detail.dataTransfer === null);
                var dragData = eventObject.detail.dataTransfer && JSON.parse(eventObject.detail.dataTransfer.getData("Text"));


                // Only handle drops with valid data
                if (usingKeyboard || dragData && dragData.sourceId === listView.id) {
                    var indicesDragged = dragData && dragData.indices || [listView.winControl.currentItem.index];
                    var dropIndexInGroup = eventObject.detail.index;
                    var dropAfterItemIndex = eventObject.detail.insertAfterIndex;
                    var ds = listView.winControl.itemDataSource;
                    ds.itemFromIndex(indicesDragged[0]).then(function (item) {
                        return ds.itemFromIndex(dropIndexInGroup).then(function (groupItem) {
                            if (dropAfterItemIndex === -1) {
                                // Insert at the start of the List
                                return moveItemUsing("insertAtStart", item, groupItem);
                            } else {
                                // On group boundaries, eventObject.detail.insertAfterIndex is the same while dropping at the end of group A vs. the start of group B.
                                // So check if we are in the same group as eventObject.detail.insertAfterIndex:
                                if (dropAfterItemIndex >= dropIndexInGroup) {
                                    // InsertAfterIndex is valid, so use ds.insertAfter
                                    return ds.itemFromIndex(dropAfterItemIndex).then(function (insertAfterItem) {
                                        return moveItemUsing("insertAfter", item, insertAfterItem);
                                    });
                                } else {
                                    // The item was potentially dropped into a different group, so use ds.insertBefore() with the item at index: dropAfterItemIndex + 1
                                    return ds.itemFromIndex(dropAfterItemIndex + 1).then(function (insertBeforeItem) {
                                        return moveItemUsing("insertBefore", item, insertBeforeItem);
                                    });
                                }
                            }
                        }).then(function (insertedItem) {
                            return new WinJS.Promise(function (c, e, p) {
                                // Wait for ListView to finish loading reordered items before setting keyboard focus
                                listView.addEventListener("loadingstatechanged", loadingStateChangedHandler);
                                function loadingStateChangedHandler() {
                                    if (listView.winControl.loadingState === "viewPortLoaded") {
                                        listView.removeEventListener("loadingstatechanged", loadingStateChangedHandler);
                                        c(insertedItem); // Pass along the item data
                                    }
                                }
                            });
                        }).then(function (insertedItem) {
                            // Set/Clear keyboard focus after the reorder
                            if (insertedItem) {
                                listView.winControl.currentItem = { index: insertedItem.index, showFocus: usingKeyboard };
                                listView.winControl.ensureVisible(insertedItem.index);
                            }
                        }).done();

                        function moveItemUsing(method, srcItem, destinationItem) {
                            if (destinationItem && destinationItem.key !== srcItem.key) {
                                copyGroupId(srcItem, destinationItem);
                                // Note: Using move operations to move items between ListView groups is not supported
                                ds.beginEdits();
                                ds.remove(srcItem.key);
                                var insertedItemPromise = ds[method](srcItem.key, srcItem.data, destinationItem.key);
                                ds.endEdits();
                                return insertedItemPromise;
                            }
                            return WinJS.Promise.wrap();
                        }

                        // IMPORTANT NOTE: This function must modify the item such that GetGroupData() returns the correct group when it is called again
                        function copyGroupId(srcItem, destinationGroupItem) {
                            srcItem.data.kind = destinationGroupItem.data.kind;
                        }
                    });
                }
            });
        }
    });
})();