(function () {
    "use strict";
    var sampleTitle = "AppBar control sample";
    var scenarios = [{
        url: "/html/create-appbar.html",
        title: "Create an AppBar"
    }, {
        url: "/html/custom-color.html",
        title: "Customize AppBar color"
    }, {
        url: "/html/custom-icons.html",
        title: "Customize icons"
    }, {
        url: "/html/custom-content.html",
        title: "Customize content"
    }, {
        url: "/html/appbar-commands.html",
        title: "Show and hide commands"
    }, {
        url: "/html/appbar-listview.html",
        title: "AppBar with ListView"
    }, {
        url: "/html/appbar-vertical.html",
        title: "Sticky AppBar"
    }, {
        url: "/html/localize-appbar.html",
        title: "Localize commands"
    }];
    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios)
    });
})();