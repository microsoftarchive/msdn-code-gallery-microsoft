(function() {
  "use strict";
  var sampleTitle = "Hotspot Authentication";
  var scenarios = [{
    url: "/html/initialization.html",
    title: "Initialization"
}, {
    url: "/html/auth-by-backgroundtask.html",
    title: "Authentication by background task"
}, {
    url: "/html/auth-by-foregroundapp.html",
    title: "Authentication by foreground app"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();