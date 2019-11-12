(function () {
    "use strict";
    var sampleTitle = "Scrolling, panning, and zooming JS sample";
    var scenarios = [{
        url: "/html/scenario1_Panning.html",
        title: "Setting panning style"
    }, {
        url: "/html/scenario2_PanningSnap.html",
        title: "Setting Snap-Points"
    }, {
        url: "/html/scenario3_Zooming.html",
        title: "Enabling zoom in / out"
    }, {
        url: "/html/scenario4_Chaining.html",
        title: "Pan and zoom"
    }, {
        url: "/html/scenario5_ChainingToWinJS.html",
        title: "Pan and zoom with WinJS"
    }, {
        url: "/html/scenario6_touchAction.html",
        title: "Touch events in pan/zoom"
    }];
    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios)
    });
})();