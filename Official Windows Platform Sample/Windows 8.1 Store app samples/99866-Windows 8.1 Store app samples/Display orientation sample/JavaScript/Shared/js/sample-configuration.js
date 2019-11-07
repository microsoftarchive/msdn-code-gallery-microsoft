(function() {
  "use strict";
  var sampleTitle = "Display Orientation Sample";
  var scenarios = [{
    url: "/html/Scenario1_AdjustForRotation.html",
    title: "Adjust for Rotation"
}, {
    url: "/html/Scenario2_SetRotationPreference.html",
    title: "Set a Rotation Preference"
}, {
    url: "/html/Scenario3_ScreenOrientation.html",
    title: "Adjust Layout for Screen Orientation"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();