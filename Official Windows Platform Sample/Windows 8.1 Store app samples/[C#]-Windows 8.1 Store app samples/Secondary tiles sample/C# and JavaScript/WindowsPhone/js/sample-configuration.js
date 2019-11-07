(function() {
  "use strict";
  var sampleTitle = "Secondary Tile JS";
  var scenarios = [{
    url: "/html/scenario1_PinTile.html",
    title: "Pin Tile"
}, {
    url: "/html/scenario2_UnpinTile.html",
    title: "Unpin Tile"
}, {
    url: "/html/scenario3_EnumerateTiles.html",
    title: "Enumerate Tiles"
}, {
    url: "/html/scenario4_TilePinned.html",
    title: "Is Tile Pinned?"
}, {
    url: "/html/scenario5_LaunchedFromSecondaryTile.html",
    title: "Show Activation Arguments"
}, {
    url: "/html/scenario6_SecondaryTileNotification.html",
    title: "Secondary Tile notifications"
}, {
    url: "/html/scenario7_PinFromAppbar.html",
    title: "Pin/Unpin Through Appbar"
}, {
    url: "/html/scenario8_UpdateAsync.html",
    title: "Update Secondary Tile Default logo"
}];

  function getScenarioIndex(args) {
      return 4;
  }

  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios),
    getScenarioIndex: getScenarioIndex
  });

})();