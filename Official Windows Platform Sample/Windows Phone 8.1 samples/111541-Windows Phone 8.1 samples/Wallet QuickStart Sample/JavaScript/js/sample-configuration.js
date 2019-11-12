//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Wallet Quickstart";

    var scenarios = [
        { url: "/html/scenario1Launch.html", title: "launch Wallet" },
        { url: "/html/scenario2Create.html", title: "add an item" },
        { url: "/html/scenario3Update.html", title: "update an item" },
        { url: "/html/scenario4Transaction.html", title: "add a transaction" },
        { url: "/html/scenario5CustomVerb.html", title: "add a custom verb" },
        { url: "/html/scenario6Relevancy.html", title: "set item relevant date and location" },
        { url: "/html/scenario7Delete.html", title: "delete an item" },
        { url: "/html/scenario8Import.html", title: "import a wallet item package" }
    ];

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios)
    });
})();
