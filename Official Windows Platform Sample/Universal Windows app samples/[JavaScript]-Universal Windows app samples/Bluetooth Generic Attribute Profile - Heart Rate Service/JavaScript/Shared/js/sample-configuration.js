(function() {
  "use strict";
  var sampleTitle = "Bluetooth Generic Attribute Profile - Heart Rate Service";
  var scenarios = [{
    url: "/html/scenario1_DeviceEvents.html",
    title: "Device Events"
}, {
    url: "/html/scenario2_ReadCharacteristicValue.html",
    title: "Read Characteristic Value"
}, {
    url: "/html/scenario3_WriteCharacteristicValue.html",
    title: "Write Characteristic Value"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();