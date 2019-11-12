(function () {
  "use strict";
  var sampleTitle = "DatagramSocket";
  var scenarios = [{
    url: "/html/scenario1_Start.html",
    title: "Start DatagramSocket Listener"
}, {
    url: "/html/scenario2_Connect.html",
    title: "Connect to Listener"
}, {
    url: "/html/scenario3_Send.html",
    title: "Send Data"
}, {
    url: "/html/scenario4_Close.html",
    title: "Close Socket"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();