(function() {
  "use strict";
  var sampleTitle = "Background Task";
  var scenarios = [{
    url: "/html/scenario1_JavascriptBackgroundTask.html",
    title: "Background task in JavaScript"
}, {
    url: "/html/scenario2_SampleBackgroundTask.html",
    title: "Background task in C#"
}, {
    url: "/html/scenario3_SampleBackgroundTaskWithCondition.html",
    title: "Background task in C# with a condition"
}, {
    url: "/html/scenario4_ServicingComplete.html",
    title: "Servicing complete task in C#"
}, {
    url: "/html/scenario5_TimeTriggerBackgroundTask.html",
    title: "Background task in C# with time trigger"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();
