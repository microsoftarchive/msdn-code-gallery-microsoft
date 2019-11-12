//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "WinJS - ListView Drag and Drop and Reorder Sample";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Enabling Reorder Within the ListView" },
        { url: "/html/scenario2.html", title: "Enabling the ListView as the source for a drag operation" },
        { url: "/html/scenario3.html", title: "Enabling the ListView as a Drop Target (specific location)" },
        { url: "/html/scenario4.html", title: "Enabling a Sorted ListView as a Drop Target" },
        { url: "/html/scenario5.html", title: "Enabling ListView Items as Drop Targets" },
        { url: "/html/scenario6.html", title: "Dragging From a ListView into Another ListView (specific location)" },
        { url: "/html/scenario7.html", title: "Enabling Reorder Within a Grouped Listview" },
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

        myData = new WinJS.Binding.List(myDataStore);
        myTargetData = new WinJS.Binding.List(myTargetDataStore);

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

var myDataStore = [
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" }
];
var myTargetDataStore = [
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" },
        { title: "Banana Blast", text: "Low-fat frozen yogurt", picture: "/images/60Banana.png" },
        { title: "Lavish Lemon Ice", text: "Sorbet", picture: "/images/60Lemon.png" },
        { title: "Marvelous Mint", text: "Gelato", picture: "/images/60Mint.png" },
        { title: "Creamy Orange", text: "Sorbet", picture: "/images/60Orange.png" },
        { title: "Succulent Strawberry", text: "Sorbet", picture: "/images/60Strawberry.png" },
        { title: "Very Vanilla", text: "Ice Cream", picture: "/images/60Vanilla.png" }
];


var myData = new WinJS.Binding.List(myDataStore);
var myTargetData = new WinJS.Binding.List(myTargetDataStore);