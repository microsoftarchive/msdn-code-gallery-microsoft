(function() {
  "use strict";
  var sampleTitle = "ApplicationData";
  var scenarios = [{
    url: "/html/scenario1_Files.html",
    title: "Files"
}, {
    url: "/html/scenario2_Settings.html",
    title: "Settings"
}, {
    url: "/html/scenario3_SettingContainer.html",
    title: "Setting Containers"
}, {
    url: "/html/scenario4_CompositeSettings.html",
    title: "Composite Settings"
}, {
    url: "/html/scenario5_DataChangedEvent.html",
    title: "DataChanged Event"
}, {
    url: "/html/scenario6_HighPriority.html",
    title: "HighPriority"
}, {
    url: "/html/scenario7_Msappdata.html",
    title: "ms-appdata:// Protocol"
}, {
    url: "/html/scenario8_ClearScenario.html",
    title: "Clear"
}, {
    url: "/html/scenario9_SetVersion.html",
    title: "SetVersion"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();