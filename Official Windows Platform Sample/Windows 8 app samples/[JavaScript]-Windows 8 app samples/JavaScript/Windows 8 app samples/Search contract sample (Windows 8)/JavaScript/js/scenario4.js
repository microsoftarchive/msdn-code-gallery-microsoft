//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            WinJS.log && WinJS.log("Use the search pane to submit a query", "sample", "status");

            // Have Windows provide suggestions from local files.
            // Scenarios 2-6 introduce different methods of providing suggestions. The local suggestion feature is enabled in this reduced scope
            //  for this sample's purpose, but in the common case, you should place this code in the global scope, e.g. default.js, to run as
            // soon as your app is launched. This way your app can provide suggestions anytime the user brings up the search pane.
            var localSuggestionSettings = new Windows.ApplicationModel.Search.LocalContentSuggestionSettings();
            localSuggestionSettings.enabled = true;
            localSuggestionSettings.locations.append(Windows.Storage.KnownFolders.musicLibrary);
            localSuggestionSettings.aqsFilter = "kind:=music";

            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().setLocalContentSuggestionSettings(localSuggestionSettings);
        },
        unload: function () {
            // Scenarios 2-6 introduce different methods of providing suggestions. For the purposes of this sample,
            // remove suggestion handling when unloading this page so that it does not conflict with other scenarios.
            // This should not be added to your app.
            var localSuggestionSettings = new Windows.ApplicationModel.Search.LocalContentSuggestionSettings();
            localSuggestionSettings.enabled = false;
            Windows.ApplicationModel.Search.SearchPane.getForCurrentView().setLocalContentSuggestionSettings(localSuggestionSettings);
        }
    });
})();
