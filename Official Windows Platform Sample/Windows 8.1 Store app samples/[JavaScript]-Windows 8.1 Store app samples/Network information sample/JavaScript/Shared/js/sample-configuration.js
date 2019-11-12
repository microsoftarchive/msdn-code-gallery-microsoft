(function() {
  "use strict";
  var sampleTitle = "Network Information";
  var scenarios = [{
    url: "/html/scenario1_InternetConnectionProfile.html",
    title: "Get Internet Connection Profile Information"
}, {
    url: "/html/scenario2_ConnectionProfileList.html",
    title: "Get Connection Profile List"
}, {
    url: "/html/scenario3_FindConnectionProfiles.html",
    title: "Find Connection Profiles"
}, {
    url: "/html/scenario4_LanId.html",
    title: "Get Lan Identifiers"
}, {
    url: "/html/scenario5_NetworkStatusChange.html",
    title: "Register for Network Status Change Notifications"
}, {
    url: "/html/scenario6_NetworkUsage.html",
    title: "Get Network Usage"
}, {
    url: "/html/scenario7_ConnectivityIntervals.html",
    title: "Get Connectivity Intervals"
}, {
    url: "/html/scenario8_AttributedNetworkUsage.html",
    title: "Get Attributed Network Usage"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();