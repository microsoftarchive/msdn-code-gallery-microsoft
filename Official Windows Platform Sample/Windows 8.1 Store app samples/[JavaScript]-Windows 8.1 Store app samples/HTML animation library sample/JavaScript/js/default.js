//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Animation Library JS Sample";

    var scenarios = [
        { url: "/html/enterPage.html", title: "Show page" },
        { url: "/html/transitionPages.html", title: "Transition between pages" },
        { url: "/html/enterContent.html", title: "Show content" },
        { url: "/html/transitionContents.html", title: "Transition between content" },
        { url: "/html/expandAndCollapse.html", title: "Expand and collapse" },
        { url: "/html/pointerFeedback.html", title: "Tap and click feedback" },
        { url: "/html/addAndDeleteFromList.html", title: "Add and remove from list" },
        { url: "/html/filterSearchList.html", title: "Filter search list" },
        { url: "/html/fadeInAndOut.html", title: "Fade in and out" },
        { url: "/html/crossfade.html", title: "Crossfade" },
        { url: "/html/reposition.html", title: "Reposition" },
        { url: "/html/dragAndDrop.html", title: "Drag and drop" },
        { url: "/html/dragBetween.html", title: "Drag between to reorder" },
        { url: "/html/showPopupUI.html", title: "Show pop-up UI" },
        { url: "/html/showEdgeUI.html", title: "Show edge UI" },
        { url: "/html/showPanel.html", title: "Show panel" },
        { url: "/html/swipeReveal.html", title: "Reveal ability to swipe" },
        { url: "/html/swipeSelection.html", title: "Swipe select and deselect" },
        { url: "/html/updateBadge.html", title: "Update a badge" },
        { url: "/html/updateTile.html", title: "Update a tile" },
        { url: "/html/customAnimation.html", title: "Run a custom animation" },
        { url: "/html/disableAnimations.html", title: "Disable animations" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
