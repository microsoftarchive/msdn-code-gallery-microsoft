//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        ready: function (element, options) {
            var listView = element.querySelector('#listView').winControl;

            restoreListView();

            // Helper function to restore the selection from the app's persistent sessionState
            function restoreListView() {
                var storedSelection = WinJS.Application.sessionState.selectedItems;
                var storedFirstVisible = WinJS.Application.sessionState.firstVisible;
                if (listView && storedSelection && typeof (storedFirstVisible) === 'number') {
                    listView.selection.set(storedSelection);
                    listView.indexOfFirstVisible = storedFirstVisible;
                }
            }

            // Store the selection and indexOfFirstVisible in the app's persistent sessionState
            function storeListViewState(eventObject) {

                // If the handler was called via the "loadingstatechanged" event, only update
                // sessionState if the listView is in the "complete" loadingState. This is useful
                // for continuously updating the indexOfFirstVisible value. Otherwise, this handler
                // was called from the "selectionchanged" or "checkpoint" events.
                if (!eventObject.type === "loadingstatechanged" || listView.loadingState === "complete") {
                    WinJS.Application.sessionState.selectedItems = listView.selection.getRanges();
                    WinJS.Application.sessionState.firstVisible = listView.indexOfFirstVisible;
                }
            }

            listView.addEventListener("selectionchanged", storeListViewState, false);
            listView.addEventListener("loadingstatechanged", storeListViewState, false);
            WinJS.Application.addEventListener("checkpoint", storeListViewState, false);
        }
    });
})();
