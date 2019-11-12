//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var feed = [];
    function rssFeedItem() { };
    rssFeedItem.prototype = {
        title: "",
        link: ""
    };
    var page = WinJS.UI.Pages.define("/html/postMessage.html", {
        ready: function (element, options) {
            makeXhrCall("feed.xml", xhrParseFeedXml);
            document.getElementById("postMessage").addEventListener("click", postMessage, false);
        }
    });

    function postMessage() {
        document.frames["postMessageIframe"].postMessage(JSON.stringify(feed), "ms-appx-web://" + document.location.host);
    }

    function makeXhrCall(url, callback) {
        WinJS.log && WinJS.log("", "sample", "error");
        WinJS.xhr({ url: url }).done(
            function (result) {
                callback(result.responseXML, result.status);
            },
            function (result) {
                callback(null, result.status);
            });
    }

    function xhrParseFeedXml(xml, statusCode) {
        document.getElementById("postMessageLocalData").innerHTML = "";
        if (xml) {
            var items = xml.querySelectorAll("rss > channel > item");
            for (var i = 0; i < 5; i++) {
                var feedItem = new rssFeedItem();
                feedItem.title = items[i].querySelector("title").textContent;
                feedItem.link = items[i].querySelector("link").textContent;
                feed.push(feedItem);

                var link = document.createElement("a");
                var newLine = document.createElement("br");
                link.setAttribute("href", feedItem.link);
                link.innerText = (i + 1) + ") " + feedItem.title;
                document.getElementById("postMessageLocalData").appendChild(link);
                document.getElementById("postMessageLocalData").appendChild(newLine);
            }
        } else {
            WinJS.log && WinJS.log("Unable to retrieve data at this time. Status code: " + statusCode, "sample", "error");
        }
    }
})();
