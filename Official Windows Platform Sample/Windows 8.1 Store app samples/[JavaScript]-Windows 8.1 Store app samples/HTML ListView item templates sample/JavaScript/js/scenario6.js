//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var MyInteractiveTemplate = WinJS.Utilities.markSupportedForProcessing(function MyInteractiveTemplate(itemPromise) {
    return itemPromise.then(function (currentItem) {
        var result = document.createElement("div");

        // ListView item
        result.className = "regularListIconTextItem";
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

        // Create a new rating control with the .win-interactive class
        // so ListView ignores touch events
        var ratingsControl = new WinJS.UI.Rating();
        WinJS.Utilities.addClass(ratingsControl.element, "win-interactive");
        body.appendChild(ratingsControl.element);

        // Update the rating with a userRating if one exists, or else
        // use the averageRating property. This will produce a different
        // visual in the ratings control for user ratings.
        var currentRating = currentItem.data.rating;
        if (typeof (currentItem.data.userRating) === "number") {

            // currentItem.data.userRating is undefined if the value
            // has not been set in the "change" handler.
            ratingsControl.userRating = currentRating;
        } else {
            ratingsControl.averageRating = currentRating;
        }

        // Attach an event listener on the rating control
        ratingsControl.addEventListener("change", function (eventObject) {

            // Update the rating in the data for this item
            var newRating = eventObject.detail.tentativeRating;
            currentItem.data.rating = newRating;
            currentItem.data.userRating = newRating;
            WinJS.log && WinJS.log("Rating for " + currentItem.data.title + " changed to " + newRating, "sample", "status");
        });

        return result;
    });
});

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
        ready: function (element, options) {
            element.querySelector("#listView").winControl.forceLayout();
        }
    });
})();
