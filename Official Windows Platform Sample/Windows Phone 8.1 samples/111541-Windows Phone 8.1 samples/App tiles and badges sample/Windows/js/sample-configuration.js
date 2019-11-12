(function () {
    "use strict";
    var sampleTitle = "Tiles JS";
    var scenarios_windows = [{
        url: "/html/scenario1_sendTextTile.html",
        title: "Send tile notification with text"
    }, {
        url: "/html/scenario2_sendLocalImageTile.html",
        title: "Send tile notification with local images"
    }, {
        url: "/html/scenario3_sendWebImageTile.html",
        title: "Send tile notification with web images"
    }, {
        url: "/html/scenario4_sendBadge.html",
        title: "Send badge notification"
    }, {
        url: "/html/scenario5_usePushNotifications.html",
        title: "Send push notifications from a Windows Azure Mobile Service"
    }, {
        url: "/html/scenario6_previewAllTemplates.html",
        title: "Preview all tile notification templates"
    }, {
        url: "/html/scenario7_enableNotificationQueue.html",
        title: "Enable notification queue and tags"
    }, {
        url: "/html/scenario8_notificationExpiration.html",
        title: "Use notification expiration"
    }, {
        url: "/html/scenario9_imageProtocols.html",
        title: "Image protocols and baseUri"
    }, {
        url: "/html/scenario10_globalization.html",
        title: "Globalization, localization, scale, and accessibility"
    }, {
        url: "/html/scenario11_contentDeduplication.html",
        title: "Content Deduplication"
    }, {
        url: "/html/scenario12_imageManipulation.html",
        title: "Image Manipulation"
    }];
    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios_windows)
    });
})();