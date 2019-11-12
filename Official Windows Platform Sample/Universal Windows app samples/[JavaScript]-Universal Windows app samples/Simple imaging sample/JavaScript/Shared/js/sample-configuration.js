(function() {
  "use strict";
  var sampleTitle = "Simple imaging JS sample";
  var scenarios = [{
    url: "/html/Scenario1_ImageProperties.html",
    title: "Image properties (FileProperties)"
}, {
    url: "/html/Scenario2_ImageTransforms.html",
    title: "Image transforms/encode (BitmapDecoder)"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();