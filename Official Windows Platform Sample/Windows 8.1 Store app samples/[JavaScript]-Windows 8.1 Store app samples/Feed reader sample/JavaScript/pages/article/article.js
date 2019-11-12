//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Helper variables.
    var ui = WinJS.UI;

    ui.Pages.define("/pages/article/article.html", {
        ready: function (element, options) {
            /// <summary>
            /// This function is called whenever a user navigates to this page. It populates the
            /// page elements with the app's data.
            /// </summary>
            /// <param name="element">
            /// The DOM element that contains all the content for the page.
            /// </param>
            /// <param name="options">
            /// An object that contains one or more property/value pairs to apply to the PageControl.
            /// </param>
            var appbar = document.getElementById("appbar").winControl;
            appbar.disabled = true;

            var article = options.article;
            element.querySelector(".titlearea .pagetitle").innerText = article.title;
            element.querySelector("#article #content").innerHTML = article.body;
            element.querySelector("#articleArea").focus();
        },

        unload: function () {
            /// <summary>This function is called when the user navigates away from the page.</summary>
            var appbar = document.getElementById("appbar").winControl;
            appbar.disabled = false;
        },

        updateResources: function (element, e) {
            /// <summary>
            /// Called by _contextChanged event handler in navigator.js when a resource 
            /// qualifier (language, scale, contrast, etc.) has changed. The element 
            /// passed is the root of this page. 
            ///
            /// Since this sample app currently doesn't have any assets with variants
            /// for scale/contrast/language/etc., the lines below that do the actual 
            /// work are commented out. This is provided here to model how to handle 
            /// scale or other resource context changes if this app were expanded to 
            /// include resources with assets for such variantions.
            /// </summary>

            // Will filter for changes to specific qualifiers.
            if (e.detail.qualifier === "Scale" || e.detail.qualifier === "Contrast") {
                // if there are string resources bound to properties using data-win-res,
                // the following line will update those properties: 

                //WinJS.Resources.processAll(element);

                // Background images from the app package with variants for scale, etc. 
                // are automatically reloaded by the platform when a resource context 
                // qualifier has changed. That is not done, however, for img elements. 
                // The following will make sure those are updated:

                //var imageElements = document.getElementsByTagName("img");
                //for (var i = 0, l = imageElements.length; i < l; i++) {
                //    var previousSource = imageElements[i].src;
                //    var uri = new Windows.Foundation.Uri(document.location, previousSource);
                //    if (uri.schemeName === "ms-appx") {
                //        imageElements[i].src = "";
                //        imageElements[i].src = previousSource;
                //    }
                //}
            }

        }
    });
})();