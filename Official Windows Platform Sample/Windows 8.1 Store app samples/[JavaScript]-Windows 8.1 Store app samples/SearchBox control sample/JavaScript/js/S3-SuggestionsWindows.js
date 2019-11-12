//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S3-SuggestionsWindows.html", {
        ready: function (element, options) {
            // Use suggestions based on music in the music library
            var localSuggestionSettings = new Windows.ApplicationModel.Search.LocalContentSuggestionSettings();
            localSuggestionSettings.enabled = true;
            localSuggestionSettings.locations.append(Windows.Storage.KnownFolders.musicLibrary);
            localSuggestionSettings.aqsFilter = "kind:=music";

            var searchBox = document.getElementById("searchBoxId");
            searchBox.winControl.setLocalContentSuggestionSettings(localSuggestionSettings);
            searchBox.addEventListener("querysubmitted", querySubmittedHandler);
        }
    });

    function querySubmittedHandler(eventObject) {
        var queryText = eventObject.detail.queryText;
        WinJS.log && WinJS.log(queryText, "sample", "status");
    }
})();
