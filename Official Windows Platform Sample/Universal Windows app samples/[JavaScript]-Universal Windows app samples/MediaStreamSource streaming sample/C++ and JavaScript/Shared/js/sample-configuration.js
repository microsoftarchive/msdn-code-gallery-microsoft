(function() {
  "use strict";
  var sampleTitle = "Media Stream Source";
  var scenarios = [{
    url: "/html/Scenario1_LoadMP3FileForAudioStream.html",
    title: "Stream MP3 Audio"
}, {
    url: "/html/Scenario2_UseDirectXForVideoStream.html",
    title: "Stream DirectX3D Video"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();