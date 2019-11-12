(function() {
  "use strict";
  var sampleTitle = "Web Authentication SDK Sample";
  var scenarios = [{
    url: "/html/oAuthFacebook.html",
    title: "Authenticate to Facebook"
}, {
    url: "/html/oAuthTwitter.html",
    title: "Sign in with Twitter"
}, {
    url: "/html/oAuthFlickr.html",
    title: "Using OAuth with Flickr"
}, {
    url: "/html/oAuthGoogle.html",
    title: "Obtain Access Token from Google"
}, {
    url: "/html/oAuthAnyService.html",
    title: "Connect to any service"
}, {
    url: "/html/manageAccounts.html",
    title: "Account Management"
}, {
    url: "/html/oAuthFilters.html",
    title: "OAuth2 using Http Filters"
}];
  WinJS.Namespace.define("SdkSample", {
    sampleTitle: sampleTitle,
    scenarios: new WinJS.Binding.List(scenarios)
});
})();