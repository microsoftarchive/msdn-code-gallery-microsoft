document.addEventListener("DOMContentLoaded", function () {
    // The resource files are in a URL in the form:
    // web_url/_layouts/15/Resource.ashx
    var scriptbase = Office.Samples.ListViewBasic.appWebUrl + "/_layouts/15/";

    // Dynamically create the invisible iframe.
    var blankiframe;
    var blankurl;
    var body;
    blankurl = Office.Samples.ListViewBasic.appWebUrl + "/Pages/blank.html";
    blankiframe = document.createElement("iframe");
    blankiframe.setAttribute("src", blankurl);
    blankiframe.setAttribute("style", "display: none");
    body = document.getElementsByTagName("body");
    body[0].appendChild(blankiframe);

    // Dynamically create the link element.
    var dclink;
    var head;
    dclink = document.createElement("link");
    dclink.setAttribute("rel", "stylesheet");
    dclink.setAttribute("href", scriptbase + "defaultcss.ashx?ctag=" + Office.Samples.ListViewBasic.ctag);
    head = document.getElementsByTagName("head");
    head[0].appendChild(dclink);
}, false); 
