//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Template for cell spanning items
var MyCellSpanningJSTemplate = WinJS.Utilities.markSupportedForProcessing(function MyCellSpanningJSTemplate(itemPromise) {
    return itemPromise.then(function (currentItem) {
        var result = document.createElement("div");

        // Use source data to decide what size to make the
        // ListView item
        result.className = currentItem.data.type;
        result.style.overflow = "hidden";

        // Display image
        var image = document.createElement("img");
        image.className = "regularListIconTextItem-Image";
        image.src = currentItem.data.picture;
        result.appendChild(image);

        var body = document.createElement("div");
        body.className = "regularListIconTextItem-Detail";
        body.style.overflow = "hidden";
        result.appendChild(body);

        // Display title
        var title = document.createElement("h4");
        title.innerText = currentItem.data.title;
        body.appendChild(title);

        // Display text
        var fulltext = document.createElement("h6");
        fulltext.innerText = currentItem.data.text;
        body.appendChild(fulltext);

        return result;
    });
});

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            element.querySelector("#listView").winControl.forceLayout();
        }
    });
})();
