//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var sampleTitle = "Push and Periodic Notifications";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Registering a notification channel" },
        { url: "/html/scenario2.html", title: "Renewing channels" },
        { url: "/html/scenario3.html", title: "Listening for push notifications" },
        { url: "/html/scenario4.html", title: "Polling for tile updates" },
        { url: "/html/scenario5.html", title: "Polling for badge updates" }
    ];

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios)
    });
})();