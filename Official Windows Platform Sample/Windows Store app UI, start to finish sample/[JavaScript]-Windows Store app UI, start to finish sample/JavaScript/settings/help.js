//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// See Adding app settings at http://go.microsoft.com/fwlink/?LinkID=288799.

(function () {
    "use strict";

    WinJS.UI.Pages.define("/settings/help.html", {
        // Initialize the page.
        // This function is called whenever a user navigates to this page. 
        // It populates the page elements with the app's data.
        ready: function (element, options) {
            var helpDiv = document.getElementById('helpDiv');
            // Register the handler for keyboard dismissal.
            this._eventHandlerRemover = [];
            var that = this;
            function addRemovableEventListener(e, eventName, handler, capture) {
                e.addEventListener(eventName, handler, capture);
                that._eventHandlerRemover.push(function () {
                    e.removeEventListener(eventName, handler);
                });
            };

            addRemovableEventListener(helpDiv, 'keydown', handleKeys, false);

            // Process app resources.            
            // Replace text-only strings bound to properties through data-win-res.
            WinJS.Resources.processAll(helpDiv);

            // HTML content using getString in JavaScript.
            var resSettingsHelpContent = WinJS.Resources.getString('/help/settingsHelpContent');
            var helpContent = document.getElementById("helpcontent");
            helpContent.innerHTML = resSettingsHelpContent.value;
        },

        unload: function () {
            // Remove the handlers for dismissal.
            document.getElementById("helpDiv").removeEventListener("keydown", handleKeys);
        },

        // This function is called by _contextChanged event handler in navigator.js when 
        // a resource qualifier (language, scale, contrast, and so on) changes. 
        // The element passed is the root of this page. 
        updateResources: function (element, e) {
            // Will filter for changes to specific qualifiers.
            if (e.detail.qualifier === "Language" || e.detail.qualifier === "Scale" || e.detail.qualifier === "Contrast") {
                // Process app resources.            
                // Replace text-only strings bound to properties through data-win-res.
                WinJS.Resources.processAll(element);

                // Images with variants from the app package are automatically reloaded 
                // by the platform when a resource context qualifier has changed. 
                // However, this is not done for img elements on a page. 
                // Here, we ensure they are updated.
                var imageElements = document.getElementsByTagName("img");
                for (var i = 0, l = imageElements.length; i < l; i++) {
                    var previousSource = imageElements[i].src;
                    var uri = new Windows.Foundation.Uri(document.location, previousSource);
                    if (uri.schemeName === "ms-appx") {
                        imageElements[i].src = "";
                        imageElements[i].src = previousSource;
                    }
                }
            }
        }
    });

    function handleKeys(evt) {
        // Handles Alt+Left and backspace key to dismiss the control.
        if ((evt.altKey && evt.key === 'Left') || (evt.key === 'Backspace')) {
            WinJS.UI.SettingsFlyout.show();
        }
    };

})();
