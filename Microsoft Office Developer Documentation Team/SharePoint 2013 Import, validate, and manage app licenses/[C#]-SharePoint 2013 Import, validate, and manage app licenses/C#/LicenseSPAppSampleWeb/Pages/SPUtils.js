var SPHostUrl;
var SPHostTitle;
var SPAppWebUrl;
var SPLayoutsFolder;
var pageTitle;


function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function loadChromeControl(pPageTitle, pSPHostUrl, pSPAppWebUrl) {
    SPHostUrl = pSPHostUrl;
    //SPHostTitle = pSPHostUrl;
    SPAppWebUrl = pSPAppWebUrl;
    SPLayoutsFolder = SPHostUrl + '/_layouts/15/';
    pageTitle = pPageTitle;
    $.getScript(SPLayoutsFolder + 'sp.ui.controls.debug.js', chromeControlLibraryLoaded);

}


function chromeControlLibraryLoaded() {

    var options = {
        siteUrl: SPHostUrl,
        //siteTitle: SPHostTitle,
        appHelpPageUrl: "../Pages/Help.html",
        appIconUrl: "AppIcon.png",
        appTitleIconUrl: "AppTitleIcon.png",
        appTitle: pageTitle,
        settingsLinks: [
            {
                linkUrl: "http://www.bing.com",
                displayName: "Dummy setting link Bing.com"
            },
            {
                linkUrl: "http://www.microsoft.com",
                displayName: "Dummy setting link Microsoft.com"
            }
        ]
    };

    /*
    * Initialize the control and make it visible.
    */
    var nav = new SP.UI.Controls.Navigation("chrome_ctrl_container", options);
    nav.setVisible(true);

}