//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/BuildNewRss.html", {
        ready: function (element, options) {
            document.getElementById("scenario1BtnDefault").addEventListener("click", scenario1BuildRss, false);

            scenario1Inialize();
        }
    });

    function scenario1Inialize() {
        loadXmlFile("buildRss", "rssTemplate.xml", "scenario1");
    }

    function scenario1BuildRss() {
        var data = document.getElementById("scenario1RssInput").value;
        if (data === "" || data === null) {
            document.getElementById("scenario1Result").value = "Please type in RSS content in the [RSS Content] box.";
            document.getElementById("scenario1Result").style.color = "red";
        }
        else {
            try {
                var doc = new Windows.Data.Xml.Dom.XmlDocument;
                doc.loadXml(document.getElementById("scenario1OriginalData").value);

                // create a rss CDataSection and insert into DOM tree
                var cdata = doc.createCDataSection(data);
                var element = doc.getElementsByTagName("content").item(0);
                element.appendChild(cdata);

                document.getElementById("scenario1Result").value = doc.getXml();
                document.getElementById("scenario1Result").style.color = "black";
            }
            catch (error) {
                document.getElementById("scenario1Result").value = error.description;
                document.getElementById("scenario1Result").style.color = "red";
            }
        }
    }
})();
