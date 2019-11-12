SP.SOD.executeOrDelayUntilScriptLoaded(function () {
    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        var ctx = SP.ClientContext.get_current();
        var site = ctx.get_site();
        var web = ctx.get_web();
        ctx.load(site);
        ctx.load(web);
        ctx.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
            var pageComponentScriptUrl = SP.Utilities.UrlBuilder.urlCombine(stripSites(site.get_url()) + resolveWeb(web.get_serverRelativeUrl()), "SPO_SpreadsheetCreatorList/SPODocCreatorPageComponent.js");
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


