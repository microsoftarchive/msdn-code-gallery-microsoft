(function () {
  "use strict";
  var sampleTitle = "HttpClient";
  var scenarios = [{
    url: "/html/scenario1_GetText.html",
    title: "GET Text With Cache Control"
}, {
    url: "/html/scenario2_GetStream.html",
    title: "GET Stream"
}, {
    url: "/html/scenario3_GetList.html",
    title: "GET List"
}, {
    url: "/html/scenario4_PostText.html",
    title: "POST Text"
}, {
    url: "/html/scenario5_PostStream.html",
    title: "POST Stream"
}, {
    url: "/html/scenario6_PostMultipart.html",
    title: "POST Multipart"
}, {
    url: "/html/scenario7_PostStreamWithProgress.html",
    title: "POST Stream With Progress"
}, {
    url: "/html/scenario8_GetCookies.html",
    title: "Get Cookies"
}, {
    url: "/html/scenario9_SetCookie.html",
    title: "Set Cookie"
}, {
    url: "/html/scenario10_DeleteCookie.html",
    title: "Delete Cookie"
}, {
    url: "/html/scenario11_MeteredConnectionFilter.html",
    title: "Metered Connection Filter"
}, {
    url: "/html/scenario12_RetryFilter.html",
    title: "Retry Filter"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();