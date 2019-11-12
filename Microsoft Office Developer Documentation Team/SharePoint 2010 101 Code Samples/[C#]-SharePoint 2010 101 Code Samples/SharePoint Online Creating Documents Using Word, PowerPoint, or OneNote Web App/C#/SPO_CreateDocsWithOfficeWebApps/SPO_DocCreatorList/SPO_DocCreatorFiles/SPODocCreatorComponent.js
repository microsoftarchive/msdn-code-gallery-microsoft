SP.SOD.executeOrDelayUntilScriptLoaded(function () {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        var ctz = SP.ClientContext.get_current();
        var site = ctz.get_site();
        var web = ctz.get_web();
        ctz.load(site);
        ctz.load(web);
        ctz.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
            var pageComponentScriptUrl = SP.Utilities.UrlBuilder.urlCombine(stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()), "SPO_DocCreatorList/SPODocCreatorPageComponent.js");
            SP.SOD.registerSod('spodoccreatorpagecomponent.js', pageComponentScriptUrl);
            LoadSodByKey('spodoccreatorpagecomponent.js', function () {
                SPODocCreatorPageComponent.initialize();
            });
        }));
    }, "cui.js");
}, "sp.js");
function resolveWeb(webUrl) {
    if (webUrl === '/') {
        return ('');
    }
    else {
        return (webUrl);
    }
}
function stripSites(sUrl) {
    var returnUrl;
    var iPosSites = sUrl.indexOf('/sites/');
    if (iPosSites != -1) {
        returnUrl = sUrl.substr(0, iPosSites);
        return (returnUrl);
    }
    else {
        return (sUrl);
    }
}


