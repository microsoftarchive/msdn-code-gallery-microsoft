(function() {
  "use strict";
  var sampleTitle = "Date and Time Formatting Sample";
  var scenarios = [{
    url: "/html/scenario1_LongAndShortFormats.html",
    title: "Format date and time using long and short"
}, {
    url: "/html/scenario2_StringTemplate.html",
    title: "Format using a string template"
}, {
    url: "/html/scenario3_ParametrizedTemplate.html",
    title: "Format using a parametrized template"
}, {
    url: "/html/scenario4_OverrideSettings.html",
    title: "Override the current user's settings"
}, {
    url: "/html/scenario5_UsingUnicodeExtensions.html",
    title: "Formatters using Unicode extensions"
}, {
    url: "/html/scenario6_TimeZoneSupport.html",
    title: "Format using different timezones"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();