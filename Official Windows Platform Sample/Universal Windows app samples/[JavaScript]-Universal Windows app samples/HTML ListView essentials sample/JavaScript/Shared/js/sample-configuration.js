(function() {
  "use strict";
  var sampleTitle = "ListView essentials sample";
  var scenarios = [{
    url: "/html/scenario1.html",
    title: "Instantiate the ListView"
}, {
    url: "/html/scenario2.html",
    title: "Responding to Invoke Events"
}, {
    url: "/html/scenario3.html",
    title: "Switching from Grid to List"
}, {
    url: "/html/scenario4.html",
    title: "Getting & Setting the Selection"
}, {
    url: "/html/scenario5.html",
    title: "Rehydrating the ListView"
}, {
    url: "/html/scenario6.html",
    title: "Configuring the orientation of a Grid or List"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();