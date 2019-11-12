//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/webContent.html", {
        ready: function (element, options) {
            showLinks();
        }
    });

    function showLinks() {
        var pages = [
            {
                id: "link1",
                name: "Microsoft Press",
                link: "http://www.microsoft.com/presspass/press/NewsArchive.mspx?cmbContentType=PressRelease"
            },
            {
                id: "link2",
                name: "Windows Phone Press",
                link: "http://www.microsoft.com/presspass/presskits/windowsphone/"

            },
            {
                id: "link3",
                name: "Windows Live Press",
                link: "http://www.microsoft.com/presspass/presskits/windowslive/"
            },
            {
                id: "link4",
                name: "Xbox Press",
                link: "http://www.microsoft.com/presspass/presskits/xbox/"
            }
        ];

        document.getElementById("sitesList").innerHTML = "";
        var list = document.createElement("ul");
        var instructions = document.createElement("div");
        instructions.innerText = "Select a page to navigate the x-ms-webview to:";
        for (var i = 0, len = pages.length; i < len; i++) {
            var listItem = document.createElement("li");
            listItem.id = pages[i].id;
            listItem.innerText = pages[i].name;
            listItem.setAttribute("dataUri", pages[i].link);
            listItem.addEventListener("click", function () {
                document.getElementById("webContentHolder").setAttribute("src", this.getAttribute("dataUri"));
            }, false);
            list.appendChild(listItem);
            if (i === 0) {
                listItem.click();
            }
        }
        document.getElementById("sitesList").appendChild(instructions);
        document.getElementById("sitesList").appendChild(list);
    }
})();
