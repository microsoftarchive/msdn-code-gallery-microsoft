/*

    Main file, sets up some global event listeners, hooks into WinJS.Application.

*/
(function () {
    'use strict';

    function activated(e) {

        var ActivationKind = Windows.ApplicationModel.Activation.ActivationKind;
        // the app does not support portrait
        Windows.Graphics.Display.DisplayProperties.autoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.landscape;

        var launchKind = e.detail.kind;
        var query = "";
        if (launchKind === Windows.ApplicationModel.Activation.ActivationKind.search) {
            query = ConferenceApp.Util.trim(e.detail.queryText);
            if (!query) {
                launchKind = ActivationKind.launch;
            }
        }

        // the first time we are launched, we need to initalize a lot of platform hooks.
        if (ConferenceApp.firstLaunch) {
            ConferenceApp.firstLaunch = false;
            ConferenceApp.Init.initAppBar();
            ConferenceApp.Init.initShortcuts();
            ConferenceApp.Init.initSearch();
            ConferenceApp.Init.initShare();
            ConferenceApp.Init.initPlayTo();
            ConferenceApp.Init.initSettings();
            ConferenceApp.Navigation.init();

            ConferenceApp.Util.updateLoadingMessage("Verifying credentials");

            ConferenceApp.Api.signIn().then(ConferenceApp.State.load).then(ConferenceApp.Api.loadInitialData).then(
            function () {
                ConferenceApp.Api.startSync();
                setImmediate(ConferenceApp.Init.initLiveTiles);
                if (launchKind === ActivationKind.launch) {
                    // restore to previous location
                    if (WinJS.Navigation.location) {
                        ConferenceApp.Navigation.renderPage(WinJS.Navigation.location, WinJS.Navigation.state, false);
                        WinJS.Promise.wrap(WinJS.Navigation.location);
                    }
                    else {
                        ConferenceApp.Navigation.navigate('home');
                    }
                }
                // the first time we launch could be for a search result
                else if (launchKind === ActivationKind.search) {
                    ConferenceApp.Navigation.navigate('searchResults', { query: query });
                }
            },
            function () {
                // if anything fails during our initial server communication we go to this dummy page.
                ConferenceApp.Navigation.navigate('noConnection');
            });
        }
        // if we get activated again while we're still running, we only care if it was a search activation.
        else if (launchKind === ActivationKind.search) {
            ConferenceApp.Navigation.navigate('searchResults', { query: query });
        }

    }

    WinJS.Application.addEventListener('activated', activated);
    WinJS.Application.start();

    WinJS.Namespace.define("ConferenceApp", {
        firstLaunch: true
    });
})();
