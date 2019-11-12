(function () {
  "use strict";
  var sampleTitle = "Background Transfer Download";
  var scenarios = [{
    url: "/html/scenario1_Download.html",
    title: "File Download"
}, {
    url: "/html/scenario2_Upload.html",
    title: "File Upload"
}, {
    url: "/html/scenario3_Notifications.html",
    title: "Completion Notifications"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();