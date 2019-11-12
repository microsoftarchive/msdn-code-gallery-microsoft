(function () {
  "use strict";
  var sampleTitle = "WebSocket";
  var scenarios = [{
    url: "/html/scenario1_Download.html",
    title: "UTF-8 text messages"
}, {
    url: "/html/scenario2_Upload.html",
    title: "Binary data stream"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();