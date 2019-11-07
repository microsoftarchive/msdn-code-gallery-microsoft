"use strict";

(function () {
    var ctag;
    var appweburl;

    //Get the URI decoded app web URL.
    appweburl =
        decodeURIComponent(
            getQueryStringParameter("SPAppWebUrl")
    );
    //Get the ctag from the SPClientTag token.
    ctag =
        decodeURIComponent(
            getQueryStringParameter("SPClientTag")
    );

    // The resource files are in a URL in the form:
    // web_url/_layouts/15/Resource.ashx
    var scriptbase = appweburl + "/_layouts/15/";

    // Dynamically create the invisible iframe
    var blankiframe;
    var blankurl;
    var body;
    blankurl = appweburl + "/Pages/blank.html";
    blankiframe = document.createElement("iframe");
    blankiframe.setAttribute("src", blankurl);
    blankiframe.setAttribute("style", "display: none");
    body = document.getElementsByTagName("body");
    body[0].appendChild(blankiframe);

    // Dynamically create the link element
    var dclink;
    var head;
    dclink = document.createElement("link");
    dclink.setAttribute("rel", "stylesheet");
    dclink.setAttribute("href", scriptbase + "defaultcss.ashx?ctag=" + ctag);
    dclink.onload = styleLoaded;
    head = document.getElementsByTagName("head");
    head[0].appendChild(dclink);
})();

// Callback for the onLoad event of the 
//  dynamically created link element
function styleLoaded() {
    // When the page has loaded the stylesheet
    //  display the page body.
    $("body").show();
}