/*
    
    This file lets us persist arbitrary state to the disk in the app's data directory.
    Mostly this is used to store the view state when the app gets terminated by PLM events.

    WinJS.Application's "checkpoint" event does most of the work here.

*/

(function () {

    "use strict";

    // save to disk every time we get a checkpoint.
    function handleCheckpoint(evt) {
        ConferenceApp.Navigation.saveState();
        var checkpointPromise = WinJS.Application.local.writeText("state.json", JSON.stringify(ConferenceApp.State.local)).then(ConferenceApp.Db.saveAll).then(null, function (e) { ConferenceApp.Debug.log("Failed to save state: " + e); });
        evt.setPromise(checkpointPromise);
    }

    WinJS.Application.addEventListener('checkpoint', handleCheckpoint);

    function load() {
        /// <summary>Load state JSON from disk.</summary>
        ConferenceApp.Util.updateLoadingMessage("Loading initial state");
        return WinJS.Application.local.readText('state.json', '{}').then(
            function (data) {
                var state = {};
                try {
                    state = ConferenceApp.Util.parseJSON(data);
                }
                catch(e) { }
                ConferenceApp.State.local = state;
                if (ConferenceApp.State.local.navigationState) {
                    WinJS.Navigation.history = ConferenceApp.State.local.navigationState;
                }
            },
            function (error) {
                ConferenceApp.Debug.log("Error while loading state: " + error);
            }
        ).then(ConferenceApp.Db.init);
    }

    function saveAll() {
        /// <summary>Fire a checkpoint event to trigger a save to the disk.</summary>
        WinJS.Application.checkpoint();
    }

    WinJS.Namespace.define('ConferenceApp.State', {
        local: {},
        saveAll: saveAll,
        load: load
    });

})();
