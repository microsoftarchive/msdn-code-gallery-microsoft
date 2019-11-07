//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "WinJS - Pivot Sample";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Basic pivot" },
        { url: "/html/scenario2.html", title: "Programmatic navigation" },
        { url: "/html/scenario3.html", title: "Pivot with a multi-select ListView" }
    ];

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios)
    });
})();