//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Scheduled Notifications";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Scheduling a toast or tile update" },
        { url: "/html/scenario2.html", title: "Querying and removing notifications" }
    ];

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios)
    });
})();