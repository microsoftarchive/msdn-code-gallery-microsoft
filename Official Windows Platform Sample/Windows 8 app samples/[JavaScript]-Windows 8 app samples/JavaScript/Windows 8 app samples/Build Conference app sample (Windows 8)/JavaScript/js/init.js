/*

    This file is where we hook into various platform components during initial activation.

*/

(function () {

    "use strict";

    var appBar = null;
    function initAppBar() {
        /// <summary>Initializes the AppBar with its complete set of items.</summary>
        
        appBar = ConferenceApp.Util.createAppBar({'home':'Home', 'schedule':'Schedule', 'sessions':'Sessions', 'people':'People', 'map':'Floorplan', 'tripReport':'Trip Report', 'news':'News'});
        document.body.appendChild(appBar.element);
        $('#save').addClass('hidden');

        ConferenceApp.Util.setupNavItem('home');
        ConferenceApp.Util.setupNavItem('schedule');
        ConferenceApp.Util.setupNavItem('sessions');
        ConferenceApp.Util.setupNavItem('people');
        ConferenceApp.Util.setupNavItem('map');
        ConferenceApp.Util.setupNavItem('tripReport');
        ConferenceApp.Util.setupNavItem('news');
        $('#backButton').click(function () {
            WinJS.Navigation.back();
        });
    }

    function initShortcuts() {
        /// <summary>Listens for Alt+Left Arrow and Alt+Right Array to navigate forward/back in the history stack.</summary>

        document.body.addEventListener('keydown', function(e) {
            if (e.altKey && (e.keyCode === WinJS.Utilities.Key.leftArrow) && !e.getModifierState('Win')) { WinJS.Navigation.back(); }
            if (e.altKey && (e.keyCode === WinJS.Utilities.Key.rightArrow) && !e.getModifierState('Win')) { WinJS.Navigation.forward(); }
        }, false);
    }

    function initSearch() {
        /// <summary>Hooks into search contract.</summary>
        var searchPane;
        var peopleImageUri = new Windows.Foundation.Uri("ms-appx:///images/search-result-people.png");
        var sessionImageUri = new Windows.Foundation.Uri("ms-appx:///images/search-result-sessions.png");
        var peopleImage = Windows.Storage.Streams.RandomAccessStreamReference.createFromUri(peopleImageUri);
        var sessionsImage = Windows.Storage.Streams.RandomAccessStreamReference.createFromUri(sessionImageUri);

        function querySubmitted(e) {
            if (e.queryText === 'showallupcoming') {
                ConferenceApp.Api.showAllUpcoming(true);
            }
            else if (e.queryText === 'shownormalupcoming') {
                ConferenceApp.Api.showAllUpcoming(false);
            }

            if (!ConferenceApp.Api.isSignedIn()) {
                return;
            }
            var query = ConferenceApp.Util.trim(e.queryText);
            if (query.length < 2) {
                return;
            }
            ConferenceApp.Navigation.navigate('searchResults', {query: query});
        }

        function suggestionRequested(e) {
            if (!ConferenceApp.Api.isSignedIn()) {
                return;
            }
            var query = ConferenceApp.Util.trim(e.queryText);
            if (query.length < 1) {
                return;
            }
            var results = ConferenceApp.Api.search(query);
            if (results.length < 4) {
                for (var i = 0; i < 3; i++) {
                    if (results[i]) {
                        var item = results[i].item;
                        var tag = '';
                        if (results[i].type === 'session') {
                            tag = 'session/' + item.SessionCode;
                            e.request.searchSuggestionCollection.appendResultSuggestion(item.Title, item.Abstract, tag, sessionsImage, 'session');
                        }
                        else if (results[i].type === 'person') {
                            tag = 'person/' + item.AttendeeId;
                            e.request.searchSuggestionCollection.appendResultSuggestion(item.Name, item.Company, tag, peopleImage, 'person');
                        }
                    }
                }
            }
        }

        function suggestionChosen(e) {
            if (!ConferenceApp.Api.isSignedIn()) {
                return;
            }
            var parts = e.tag.split('/');
            var type = parts[0];
            if (type === 'session') {
                var session = ConferenceApp.Db.all('sessions').filterBy('SessionCode', parts[1])[0];
                ConferenceApp.Navigation.navigate('sessionDetail', { item: session });
            }
            else if (type === 'person') {
                var person = ConferenceApp.Db.all('attendees').filterBy('AttendeeId', parseInt(parts[1], 10))[0];
                ConferenceApp.Navigation.navigate('peopleDetail', { item: person });
            }
        }

        try {
            searchPane = Windows.ApplicationModel.Search.SearchPane.getForCurrentView();
            searchPane.searchHistoryEnabled = false;
            searchPane.addEventListener("querysubmitted", querySubmitted);
            searchPane.addEventListener("suggestionsrequested", suggestionRequested);
            searchPane.addEventListener("resultsuggestionchosen", suggestionChosen);
        } catch (e) { }
    }

    function initShare() {
        /// <summary>Hooks into share contract.</summary>
        var manager = Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView();

        function handleDataRequested(evt) {
            var data = ConferenceApp.Navigation.share();
            if (data) {
                evt.request.data = data;
            }
        }

        manager.addEventListener("datarequested", handleDataRequested);
    }

    function initPlayTo() {
        /// <summary>Hooks into PlayTo contract.</summary>
        var ptm = Windows.Media.PlayTo.PlayToManager.getForCurrentView();
        ptm.addEventListener("sourcerequested", function (e) {
            var data = ConferenceApp.Navigation.playTo();
            if (data) {
                e.sourceRequest.setSource(data);
            }
        });

    }

    function initSettings() {
        /// <summary>Hooks into Settings contract.</summary>
        function handleSignOut() {
            if (ConferenceApp.Api.isSignedIn()) {
                ConferenceApp.Api.stopSync();
                ConferenceApp.Api.clearUserData();
                ConferenceApp.Api.signOut().then(function () {
                    ConferenceApp.Navigation.navigate('noConnection');
                });
            }
        }
        function handlePrivacy() {
            ConferenceApp.Util.launchUrl('http://go.microsoft.com/fwlink/?LinkId=226414');
        }

        function addCustomCommands(e) {
            var commands = e.request.applicationCommands;
            if (ConferenceApp.Api.canSignOut()) {
                commands.append(new ApplicationSettings.SettingsCommand("signOut", "Sign Out", handleSignOut));
            }
            commands.append(new ApplicationSettings.SettingsCommand("privacyStatement", "Privacy Statement", handlePrivacy));
        }

        var ApplicationSettings = Windows.UI.ApplicationSettings;
        var pane = ApplicationSettings.SettingsPane.getForCurrentView();
        pane.addEventListener("commandsrequested", addCustomCommands);
    }

    function initLiveTiles() {
        /// <summary>Pushes Live Tile updates</summary>
        var updater = Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication();
        updater.enableNotificationQueue(true);
        updateLiveTiles();
        // update tiles every ten minutes
        setInterval(updateLiveTiles, 600000);
    }

    function updateLiveTiles() {
        /// <summary>Show the top 5 most popular sessions as a LiveTile.</summary>
        var top5 = ConferenceApp.Api.getPopularSessions().slice(0, 5);
        top5.forEach(function (item) {
            ConferenceApp.Util.updateTile(item.Title, item.Abstract);
        });
    }

    WinJS.Namespace.define("ConferenceApp.Init", {
        initAppBar: initAppBar,
        initShortcuts: initShortcuts,
        initSearch: initSearch,
        initShare: initShare,
        initPlayTo: initPlayTo,
        initSettings: initSettings,
        initLiveTiles: initLiveTiles
    });

})();
