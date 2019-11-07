(function() {
  "use strict";
  var sampleTitle = "Proximity JavaScript Sample";
  var scenarios = [{
    url: "/html/Scenario1_PeerFinder.html",
    title: "Use PeerFinder to connect to peers"
}, {
    url: "/html/Scenario2_PeerWatcher.html",
    title: "Use PeerWatcher to scan for peers"
}, {
    url: "/html/Scenario3_ProximityDevice.html",
    title: "Use ProximityDevice to publish and subscribe for messages"
}, {
    url: "/html/Scenario4_ProximityDeviceEvents.html",
    title: "Display ProximityDevice events"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();