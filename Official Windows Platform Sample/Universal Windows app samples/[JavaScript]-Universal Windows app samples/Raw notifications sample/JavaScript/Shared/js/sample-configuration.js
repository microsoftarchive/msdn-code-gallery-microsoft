//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Raw Notifications";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Creating a channel and background task" },
        { url: "/html/scenario2.html", title: "Raw notification events" },
    ];

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios)
    });
})();