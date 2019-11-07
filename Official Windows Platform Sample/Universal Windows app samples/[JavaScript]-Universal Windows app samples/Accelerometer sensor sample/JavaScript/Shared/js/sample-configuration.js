(function() {
  "use strict";
  var sampleTitle = "Accelerometer";
  var scenarios = [{
    url: "/html/scenario1_DataEvents.html",
    title: "Data Events"
}, {
    url: "/html/scenario2_ShakeEvents.html",
    title: "Shake Events"
}, {
    url: "/html/scenario3_Polling.html",
    title: "Polling"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();