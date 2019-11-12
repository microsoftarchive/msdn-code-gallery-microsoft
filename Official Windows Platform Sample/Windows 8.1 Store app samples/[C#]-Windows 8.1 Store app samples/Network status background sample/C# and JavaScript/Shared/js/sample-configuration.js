(function() {
  "use strict";
  var sampleTitle = "Network Status";
  var scenarios = [{
    url: "/html/scenario1_NetworkStatusWithInternetPresent.html",
    title: "Network Status with Internet Present"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();