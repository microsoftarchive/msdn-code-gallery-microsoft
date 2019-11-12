(function() {
  "use strict";
  var sampleTitle = "Device Capabilities JS sample";
  var scenarios = [{
    url: "/html/scenario2_Mouse.html",
    title: "Mouse Capabilities"
}, {
    url: "/html/scenario1_Keyboard.html",
    title: "Keyboard Capabilities"
}, {
    url: "/html/scenario3_Touch.html",
    title: "Touch  Capabilities"
}, {
    url: "/html/scenario4_Pointer.html",
    title: "Pointer  Capabilities"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();