(function() {
  "use strict";
  var sampleTitle = "Bluetooth Rfcomm Chat (Client)";
  var scenarios = [{
    url: "/html/scenario1_ChatClient.html",
    title: "Run Chat client"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();