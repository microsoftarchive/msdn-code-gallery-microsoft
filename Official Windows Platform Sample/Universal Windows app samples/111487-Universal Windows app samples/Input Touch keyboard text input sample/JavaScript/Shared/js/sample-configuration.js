(function() {
  "use strict";
  var sampleTitle = "TouchKeyboard";
  var scenarios = [{
    url: "/html/scenario1_SpellingTextSuggestions.html",
    title: "Spelling & Text Suggestions"
}, {
    url: "/html/scenario2_ScopedViews.html",
    title: "Scoped Views"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();