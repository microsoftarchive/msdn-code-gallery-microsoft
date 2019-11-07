(function() {
  "use strict";
  var sampleTitle = "Association Launching";
  var scenarios = [{
    url: "/html/scenario1_LaunchFile.html",
    title: "Launching a file"
}, {
    url: "/html/scenario2_LaunchUri.html",
    title: "Launching a URI"
}, {
    url: "/html/scenario3_ReceiveFile.html",
    title: "Receiving a file"
}, {
    url: "/html/scenario4_ReceiveUri.html",
    title: "Receiving a URI"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();