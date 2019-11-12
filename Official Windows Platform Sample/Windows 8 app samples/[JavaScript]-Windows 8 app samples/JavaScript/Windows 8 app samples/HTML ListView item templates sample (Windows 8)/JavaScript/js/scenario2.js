//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Custom templating function
var MyJSItemTemplate = WinJS.Utilities.markSupportedForProcessing(function MyJSItemTemplate(itemPromise) {
    return itemPromise.then(function (currentItem) {

        // Build ListView Item Container div
        var result = document.createElement("div");
        result.className = "regularListIconTextItem";
        result.style.overflow = "hidden";

        // Build icon div and insert into ListView Item
        var image = document.createElement("img");
        image.className = "regularListIconTextItem-Image";
        image.src = currentItem.data.picture;
        result.appendChild(image);

        // Build content body
        var body = document.createElement("div");
        body.className = "regularListIconTextItem-Detail";
        body.style.overflow = "hidden";

        // Display title
        var title = document.createElement("h4");
        title.innerText = currentItem.data.title;
        body.appendChild(title);

        // Display text
        var fulltext = document.createElement("h6");
        fulltext.innerText = currentItem.data.text;
        body.appendChild(fulltext);

        //put the body into the ListView Item
        result.appendChild(body);

        return result;
    });
});

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            element.querySelector("#listView").winControl.forceLayout();
        }
    });
})();
