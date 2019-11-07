(function() {
  "use strict";
  var sampleTitle = "Device Enumeration Sample";
  var scenarios = [{
    url: "/html/interfaces.html",
    title: "Device Interfaces"
}, {
    url: "/html/containers.html",
    title: "Device Containers"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();