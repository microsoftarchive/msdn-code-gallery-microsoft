(function () {
    "use strict";

    // URL to the feedCollection.json file that contains data of
    // all the feeds.
    var feedCollectionJSON = "ms-appx:///feedCollection.json";

    // The filename of the file containing the selected feed indices.
    var selectionIndicesJSON = "selectionIndices.json";

    // Sets up the state of the app (loads the feed collection, 
    // the selected feed indices, and sets up the list views and
    // subscribes to the selected feeds).
    function loadStateAsync() {
        return new WinJS.Promise(function (c) {

            // Load the feed collection.
            loadFeedCollectionAsync().then(function () {

                // Load the selection indices, after the feed
                // collection is loaded.
                loadSelectionIndicesAsync().then(function () {

                    // Setup the feed collection for the Subscriptions page.
                    Render.setupFeedCollectionListView();

                    // Subscribe to the selected feeds.
                    Render.selectionIndices.forEach(function (index) {
                        var feed = Render.getFeedObject(index);
                        if (feed) {
                            Data.subscribe(feed);
                        }
                    });

                    c();
                });
            });
        });
    }

    // Loads the feed collection.
    function loadFeedCollectionAsync() {
        return Windows.Storage.PathIO.readTextAsync(feedCollectionJSON).then(function (feedCollectionString) {
            var feedCollectionData = JSON.parse(feedCollectionString);
            for (var i = 0, len = feedCollectionData.length; i < len; i++) {
                Data.allFeeds.push(new Data.Feed(feedCollectionData[i]));
            }
        });
    }

    // Saves the current selection of feed indices.
    function saveSelectionIndicesAsync() {
        var local = WinJS.Application.local;
        return local.writeText(selectionIndicesJSON, JSON.stringify(Render.selectionIndices));
    }

    // Loads the selected indices.
    function loadSelectionIndicesAsync() {
        return new WinJS.Promise(function (complete) {
            try {
                WinJS.Application.local.exists(selectionIndicesJSON).then(function (exists) {
                    if (exists) {
                        WinJS.Application.local.readText(selectionIndicesJSON).then(function (selectionIndicesString) {
                            Render.selectionIndices = JSON.parse(selectionIndicesString);
                            complete();
                        });
                    } else {
                        Render.selectionIndices = [48, 54];
                        complete();
                    }
                });
            } catch (e) {
                Render.selectionIndices = [48, 54];
                complete();
            }
        });
    }

    // Public interface.
    WinJS.Namespace.define("IO", {
        loadStateAsync: loadStateAsync,
        saveStateAsync: saveSelectionIndicesAsync
    });
})();
