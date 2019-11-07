(function() {
  "use strict";
  var sampleTitle = "Calendar Sample";
  var scenarios = [{
    url: "/html/scenario1_DisplayCalendarData.html",
    title: "Display a calendar"
}, {
    url: "/html/scenario2_DisplayCalendarStatistics.html",
    title: "Retrieve calendar statistics"
}, {
    url: "/html/scenario3_CalendarEnumerationAndMath.html",
    title: "Calendar enumeration and math"
}, {
    url: "/html/scenario4_CalendarWithUnicodeExtensions.html",
    title: "Calendar with Unicode extensions in languages"
}, {
    url: "/html/scenario5_CalendarTimeZoneSupport.html",
    title: "Calendar timezone support"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();