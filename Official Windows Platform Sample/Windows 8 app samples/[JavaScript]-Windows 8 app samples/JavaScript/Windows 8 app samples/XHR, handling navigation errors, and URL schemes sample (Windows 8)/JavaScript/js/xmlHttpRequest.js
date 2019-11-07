//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/xmlHttpRequest.html", {
        ready: function (element, options) {
            document.getElementById("xhrRemote").addEventListener("click", xhrRemote, false);
            document.getElementById("xhrLocal").addEventListener("click", xhrLocal, false);
        }
    });

    function xhrRemote() {
        makeXhrCall("http://rss.msnbc.msn.com/id/3032127/device/rss/rss.xml", xhrParseXml);
    }

    function xhrLocal() {
        makeXhrCall("feed.xml", xhrParseXml);
    }

    function makeXhrCall(url, callback) {
        WinJS.log && WinJS.log("", "sample", "status");
        document.getElementById("response").innerHTML = "";

        WinJS.xhr({ url: url }).done(
            function (result) {
                callback(result.responseXML, result.status);
            },
            function (result) {
                callback(null, result.status);
            });
    }

    function xhrParseXml(xml, statusCode) {
        if (xml) {
            var items = xml.querySelectorAll("rss > channel > item");
            if (items) {
                var /*@override*/ length = Math.min(10, items.length);
                for (var i = 0; i < length; i++) {
                    var link = document.createElement("a");
                    var newLine = document.createElement("br");
                    link.setAttribute("href", items[i].querySelector("link").textContent);
                    link.innerText = (i + 1) + ") " + items[i].querySelector("title").textContent;
                    document.getElementById("response").appendChild(link);
                    document.getElementById("response").appendChild(newLine);
                }
            } else {
                WinJS.log && WinJS.log("There are no items available at this time", "sample", "status");
            }
        } else {
            WinJS.log && WinJS.log("Unable to retrieve data at this time. Status code: " + statusCode, "sample", "error");
        }
    }
})();
