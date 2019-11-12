(function() {
  "use strict";
  var sampleTitle = "Syndication";
  var scenarios = [{
    url: "/html/Scenario1_GetFeed.html",
    title: "Get Feed"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();