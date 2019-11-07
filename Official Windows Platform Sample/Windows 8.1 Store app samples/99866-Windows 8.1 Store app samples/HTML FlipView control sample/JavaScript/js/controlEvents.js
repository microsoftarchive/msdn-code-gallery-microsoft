//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var controlEventsLog, myFlipView;

    var page = WinJS.UI.Pages.define("/html/controlEvents.html", {
        processed: function (element, options) {
            // WinJS.UI.processAll() is automatically called by the Pages infrastructure by the time this
            // function gets called, and it has processed the div with data-win-control="WinJS.UI.FlipView"
            
            myFlipView = document.getElementById("controlEvents_FlipView").winControl;
            myFlipView.itemTemplate = placeholderRenderer;

            // Register for events supported by the control
            myFlipView.addEventListener("pageselected", handlePageSelected);
            myFlipView.addEventListener("pagecompleted", handlePageCompleted);
            myFlipView.addEventListener("pagevisibilitychanged", handlePageVisibilityChanged);
            myFlipView.addEventListener("datasourcecountchanged", handleDataSourceCountChanged);

            controlEventsLog = document.getElementById("controlEventsLog");
            document.getElementById("clearEventsLog").addEventListener("click", function() {
                WinJS.Utilities.empty(controlEventsLog);
            });
            document.getElementById("insertItem").addEventListener("click", function () {
                var newItem = {
                    type: "item",
                    title: "Valley inserted at " + new Date(),
                    picture: "images/Valley.jpg"
                };

                DefaultData.bindingList.push(newItem);
            });
        }
    });

    function placeholderRenderer(itemPromise) {
        // create a basic template for the item which doesn't depend on the data
        var element = document.createElement("div");
        element.className = "controlEventsItemTemplate";
        element.innerHTML = "<div class='content'>Loading...</div>";

        // return the element as the placeholder, and a callback to update it when data is available
        return {
            element: element,
            // specifies a promise that will be completed when rendering is complete
            // itemPromise will complete when the data is available. 
            renderComplete: WinJS.Promise.timeout(3000).then(function () {
                // Simulating a 3 second delay in the renderer to emphasize the difference between the pageselected
                // and the pagecompleted event.
                return itemPromise.then(function (item) {
                    // mutate the element to include the data
                    element.querySelector(".content").innerText = item.data.title;
                    element.insertAdjacentHTML("afterBegin", "<img src='" + item.data.picture + "' alt='Databound image' />");
                });
            })
        };
    }

    function logEventInfo(message) {
        var currentValue = controlEventsLog.innerHTML;
        controlEventsLog.innerHTML = new Date() + " - " + message + "<br/>" + currentValue;
    }

    function handlePageSelected(ev) {
        logEventInfo("pageselected triggered for [" + ev.target.innerText.trim() + "], currentPage = " + myFlipView.currentPage);
    }

    function handlePageCompleted(ev) {
        logEventInfo("pagecompleted triggered for [" + ev.target.innerText.trim() + "], currentPage = " + myFlipView.currentPage);
    }

    function handlePageVisibilityChanged(ev) {
        logEventInfo("pagevisibilitychanged triggered for [" + ev.target.innerText.trim() + "], visible = " + ev.detail.visible);
    }

    function handleDataSourceCountChanged(ev) {
        myFlipView.count().then(function (count) {
            logEventInfo("datasourcecountchanged triggered, count = " + count);
        });
    }
})();
